using PAS.Asset.Domain.Common;

namespace PAS.Asset.Domain.Funds.Events;

public sealed class FundNavUpdatedDomainEvent : IDomainEvent {
    public Guid EventId { get; }
    public Guid FundId { get; }
    public DateTime Date { get; }
    public decimal Value { get; }
    public DateTime OccurredAtUtc { get; }

    public FundNavUpdatedDomainEvent(Guid fundId, DateTime date, decimal value) {
        EventId = Guid.NewGuid();
        FundId = fundId;
        Date = date;
        Value = value;
        OccurredAtUtc = DateTime.UtcNow;
    }
}
