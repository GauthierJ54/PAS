using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PAS.Calculation.Application.FundPerformances.Commands.ProcessFundDelete;
using PAS.Calculation.Infrastructure.Persistence;
using PAS.Calculation.Infrastructure.Persistence.Inbox;
using PAS.Contracts.Assets;
using PAS.Contracts.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace PAS.Calculation.Infrastructure.Messaging.Consumers;

public sealed class FundSoftDeleteConsumer : BackgroundService {
    private const string QueueName = $"pas.calculation.{AssetMessagingTopology.FundSoftDeleteRoutingKey}";

    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FundSoftDeleteConsumer> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public FundSoftDeleteConsumer(IConnection connection, IServiceScopeFactory scopeFactory, ILogger<FundSoftDeleteConsumer> logger) {
        _connection = connection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        await using var channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await DeclareTopologyAsync(channel, stoppingToken);

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, delivery) => {
            try {
                var body = delivery.Body.ToArray();

                var integrationEvent = JsonSerializer.Deserialize<FundSoftDeleteIntegrationEvent>(body, JsonOptions);

                if (integrationEvent is null ||
                    integrationEvent.EventId == Guid.Empty ||
                    integrationEvent.FundId == Guid.Empty ||
                    integrationEvent.OccurredAtUtc == default ||
                    integrationEvent.Version != 1) {
                    throw new JsonException("Le message FundSoftDelete est incomplet ou invalide.");
                }

                await using var scope = _scopeFactory.CreateAsyncScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<CalculationDbContext>();

                var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                var alreadyProcessed = await dbContext.ProcessedMessages.AsNoTracking().AnyAsync(message => message.EventId == integrationEvent.EventId, stoppingToken);

                if (alreadyProcessed) {
                    _logger.LogInformation("L'événement {EventId} a déjà été traité. Il sera acquitté sans être rejoué.", integrationEvent.EventId);

                    await channel.BasicAckAsync(deliveryTag: delivery.DeliveryTag, multiple: false, cancellationToken: stoppingToken);

                    return;
                }

                await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);

                try {
                    var command = new ProcessFundDeleteCommand(integrationEvent.EventId, integrationEvent.FundId, integrationEvent.OccurredAtUtc, integrationEvent.Version);

                    await sender.Send(command, stoppingToken);

                    var processedMessage = ProcessedMessage.Create(integrationEvent.EventId);

                    dbContext.ProcessedMessages.Add(processedMessage);

                    await dbContext.SaveChangesAsync(stoppingToken);

                    await transaction.CommitAsync(stoppingToken);
                } catch {
                    await transaction.RollbackAsync(stoppingToken);
                    throw;
                }

                await channel.BasicAckAsync(deliveryTag: delivery.DeliveryTag, multiple: false, cancellationToken: stoppingToken);

                _logger.LogInformation("Fund supprimé pour le fonds {FundId}, événement {EventId}.", integrationEvent.FundId, integrationEvent.EventId);

            } catch (Exception exception) {
                _logger.LogError(exception, "Échec du traitement du message RabbitMQ {DeliveryTag}.", delivery.DeliveryTag);

                await channel.BasicNackAsync(deliveryTag: delivery.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

        _logger.LogInformation("Consumer RabbitMQ démarré sur la queue {QueueName}.", QueueName);

        try {
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        } catch (OperationCanceledException)
              when (stoppingToken.IsCancellationRequested) {
            _logger.LogInformation("Arrêt du consumer RabbitMQ {QueueName}.", QueueName);
        }
    }

    private static async Task DeclareTopologyAsync(IChannel channel, CancellationToken cancellationToken) {
        await channel.ExchangeDeclareAsync(exchange: AssetMessagingTopology.Exchange, type: ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);

        await channel.QueueBindAsync(queue: QueueName, exchange: AssetMessagingTopology.Exchange, routingKey: AssetMessagingTopology.FundSoftDeleteRoutingKey, cancellationToken: cancellationToken);
    }
}