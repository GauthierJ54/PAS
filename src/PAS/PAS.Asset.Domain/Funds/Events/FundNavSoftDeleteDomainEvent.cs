using PAS.Asset.Domain.Common;

namespace PAS.Asset.Domain.Funds.Events;

public sealed class FundNavSoftDeleteDomainEvent : IDomainEvent {
    public Guid EventId { get; }
    public Guid FundId { get; }
    public DateTime Date { get; }
    public DateTime OccurredAtUtc { get; }

    public FundNavSoftDeleteDomainEvent(Guid fundId, DateTime date) {
        EventId = Guid.NewGuid();
        FundId = fundId;
        Date = date;
        OccurredAtUtc = DateTime.UtcNow;
    }
}
