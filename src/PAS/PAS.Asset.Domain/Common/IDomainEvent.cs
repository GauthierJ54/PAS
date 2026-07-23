namespace PAS.Asset.Domain.Common;

public interface IDomainEvent {
    Guid EventId { get; }
    DateTime OccurredAtUtc { get; }
}