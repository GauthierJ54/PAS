using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PAS.Calculation.Application.FundPerformances.Commands.ProcessFundNavAdded;
using PAS.Contracts.Assets;
using PAS.Contracts.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PAS.Calculation.Infrastructure.Messaging.Consumers;

public sealed class FundNavAddedConsumer : BackgroundService {
    private const string QueueName = "pas.calculation.fund-nav-added.v1";

    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FundNavAddedConsumer> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public FundNavAddedConsumer(IConnection connection, IServiceScopeFactory scopeFactory, ILogger<FundNavAddedConsumer> logger) {
        _connection = connection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        await using var channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await DeclareTopologyAsync(channel, stoppingToken);

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, delivery) => {
            try {
                var body = delivery.Body.ToArray();

                var integrationEvent = JsonSerializer.Deserialize<FundNavAddedIntegrationEvent>(body, JsonOptions);

                if (integrationEvent is null) {
                    throw new JsonException("Le message FundNavAdded est vide ou invalide.");
                }

                using var scope = _scopeFactory.CreateScope();

                var sender = scope.ServiceProvider
                    .GetRequiredService<ISender>();

                var command = new ProcessFundNavAddedCommand(
                    integrationEvent.EventId,
                    integrationEvent.FundId,
                    integrationEvent.Date,
                    integrationEvent.Value,
                    integrationEvent.OccurredAtUtc,
                    integrationEvent.Version);

                await sender.Send(command, stoppingToken);

                await channel.BasicAckAsync(
                    deliveryTag: delivery.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

                _logger.LogInformation(
                    "NAV reçue pour le fonds {FundId}, date {Date}, événement {EventId}.",
                    integrationEvent.FundId,
                    integrationEvent.Date,
                    integrationEvent.EventId);
            } catch (Exception exception) {
                _logger.LogError(
                    exception,
                    "Échec du traitement du message RabbitMQ {DeliveryTag}.",
                    delivery.DeliveryTag);

                await channel.BasicNackAsync(
                    deliveryTag: delivery.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation(
            "Consumer RabbitMQ démarré sur la queue {QueueName}.",
            QueueName);

        try {
            await Task.Delay(
                Timeout.InfiniteTimeSpan,
                stoppingToken);
        } catch (OperationCanceledException)
              when (stoppingToken.IsCancellationRequested) {
            _logger.LogInformation(
                "Arrêt du consumer RabbitMQ {QueueName}.",
                QueueName);
        }
    }

    private static async Task DeclareTopologyAsync(
        IChannel channel,
        CancellationToken cancellationToken) {
        await channel.ExchangeDeclareAsync(
            exchange: AssetMessagingTopology.Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: QueueName,
            exchange: AssetMessagingTopology.Exchange,
            routingKey:
                AssetMessagingTopology.FundNavAddedRoutingKey,
            cancellationToken: cancellationToken);
    }
}