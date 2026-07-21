namespace PAS.Asset.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage {
    public Guid EventId { get; private set; }

    public string EventType { get; private set; } = string.Empty;

    public string RoutingKey { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public DateTimeOffset? ProcessedAtUtc { get; private set; }

    public int RetryCount { get; private set; }

    public string? LastError { get; private set; }

    private OutboxMessage() {
    }

    private OutboxMessage(
        Guid eventId,
        string eventType,
        string routingKey,
        string payload,
        DateTimeOffset occurredAtUtc) {
        EventId = eventId;
        EventType = eventType;
        RoutingKey = routingKey;
        Payload = payload;
        OccurredAtUtc = occurredAtUtc;
    }

    public static OutboxMessage Create(Guid eventId, string eventType, string routingKey, string payload, DateTimeOffset occurredAtUtc) {
        if (eventId == Guid.Empty)
            throw new ArgumentException("EventId ne peut pas être vide.");

        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("EventType est obligatoire.");

        if (string.IsNullOrWhiteSpace(routingKey))
            throw new ArgumentException("RoutingKey est obligatoire.");

        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException("Payload est obligatoire.");

        return new OutboxMessage(eventId, eventType, routingKey, payload, occurredAtUtc);
    }

    public void MarkAsProcessed(DateTimeOffset processedAtUtc) {
        ProcessedAtUtc = processedAtUtc;
        LastError = null;
    }

    public void MarkAsFailed(string error) {
        RetryCount++;
        LastError = error;
    }
}