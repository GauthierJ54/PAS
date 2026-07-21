namespace PAS.Asset.Infrastructure.Messaging.Publishers;

public interface IRabbitMqPublisher {
    Task PublishAsync(Guid eventId, string eventType, string routingKey, string payload, CancellationToken cancellationToken);
}