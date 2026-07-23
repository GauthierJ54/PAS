using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PAS.Asset.Infrastructure.Messaging.Publishers;
using PAS.Asset.Infrastructure.Persistence;

namespace PAS.Asset.Infrastructure.Messaging.Outbox;

public sealed class OutboxPublisherWorker : BackgroundService {

    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(2);
    private const int BatchSize = 20;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMqPublisher _publisher;
    private readonly ILogger<OutboxPublisherWorker> _logger;

    public OutboxPublisherWorker(IServiceScopeFactory scopeFactory, IRabbitMqPublisher publisher, ILogger<OutboxPublisherWorker> logger) {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Worker Outbox démarré.");

        while (!stoppingToken.IsCancellationRequested) {
            try {
                await PublishPendingMessagesAsync(stoppingToken);
            } catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested) {
                break;
            } catch (Exception exception) {
                _logger.LogError(exception, "Erreur pendant le traitement de l'Outbox.");
            }

            try {
                await Task.Delay(PollingInterval, stoppingToken);
            } catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested) {
                break;
            }
        }

        _logger.LogInformation("Worker Outbox arrêté.");
    }

    private async Task PublishPendingMessagesAsync(CancellationToken cancellationToken) {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AssetDbContext>();

        var messages = await dbContext.OutboxMessages
            .Where(message => message.ProcessedAtUtc == null)
            .OrderBy(message => message.OccurredAtUtc)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in messages) {
            try {
                await _publisher.PublishAsync(eventId: message.EventId, eventType: message.EventType,  routingKey: message.RoutingKey, payload: message.Payload, cancellationToken: cancellationToken);

                message.MarkAsProcessed(DateTimeOffset.UtcNow);

                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Message Outbox {EventId} marqué comme traité.",  message.EventId);
            } catch (OperationCanceledException)
                  when (cancellationToken.IsCancellationRequested) {
                        throw;
                    } catch (Exception exception) {
                        var error = exception.ToString();

                        if (error.Length > 2000) {
                            error = error[..2000];
                    }

                message.MarkAsFailed(error);

                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogError(exception, "Échec de publication du message Outbox {EventId}. Tentative {RetryCount}.", message.EventId, message.RetryCount);

                break;
            }
        }
    }
}