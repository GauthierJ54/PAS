using PAS.Asset.Domain.Common;

namespace PAS.Asset.Domain.Funds.Events;

public sealed class FundSoftDeleteDomainEvent : IDomainEvent {
    public Guid EventId { get; }
    public Guid FundId { get; }
    public DateTime OccurredAtUtc { get; }

    public FundSoftDeleteDomainEvent(Guid fundId) {
        EventId = Guid.NewGuid();
        FundId = fundId;
        OccurredAtUtc = DateTime.UtcNow;
    }
}
