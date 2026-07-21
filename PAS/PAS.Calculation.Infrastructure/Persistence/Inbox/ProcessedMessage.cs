namespace PAS.Calculation.Infrastructure.Persistence.Inbox;

public sealed class ProcessedMessage {
    public Guid EventId { get; private set; }

    public DateTimeOffset ProcessedAtUtc { get; private set; }

    private ProcessedMessage() {
    }

    private ProcessedMessage(Guid eventId, DateTimeOffset processedAtUtc) {
        if (eventId == Guid.Empty) {
            throw new ArgumentException(
                "L'identifiant de l'événement ne peut pas être vide.",
                nameof(eventId));
        }

        EventId = eventId;
        ProcessedAtUtc = processedAtUtc;
    }

    public static ProcessedMessage Create(Guid eventId) {
        return new ProcessedMessage(eventId, DateTimeOffset.UtcNow);
    }
}