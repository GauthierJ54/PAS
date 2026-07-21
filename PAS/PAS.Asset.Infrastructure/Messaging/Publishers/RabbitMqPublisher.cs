using System.Text;
using Microsoft.Extensions.Logging;
using PAS.Contracts.Messaging;
using RabbitMQ.Client;

namespace PAS.Asset.Infrastructure.Messaging.Publishers;

public sealed class RabbitMqPublisher : IRabbitMqPublisher {
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(IConnection connection, ILogger<RabbitMqPublisher> logger) {
        _connection = connection;
        _logger = logger;
    }

    public async Task PublishAsync(Guid eventId, string eventType, string routingKey, string payload, CancellationToken cancellationToken) {
        var channelOptions = new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true);

        await using var channel = await _connection.CreateChannelAsync(options: channelOptions, cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(exchange: AssetMessagingTopology.Exchange, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null, cancellationToken: cancellationToken);

        var properties = new BasicProperties {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            MessageId = eventId.ToString(),
            Type = eventType
        };

        var body = Encoding.UTF8.GetBytes(payload);

        await channel.BasicPublishAsync(exchange: AssetMessagingTopology.Exchange, routingKey: routingKey, mandatory: true, basicProperties: properties, body: body, cancellationToken: cancellationToken);

        _logger.LogInformation("Événement Outbox {EventId} publié avec la routing key {RoutingKey}.", eventId, routingKey);
    }
}