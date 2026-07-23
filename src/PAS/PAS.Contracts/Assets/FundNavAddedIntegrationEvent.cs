namespace PAS.Contracts.Assets {
    public sealed record FundNavAddedIntegrationEvent(
        Guid EventId,
        Guid FundId,
        DateOnly Date,
        decimal Value,
        DateTimeOffset OccurredAtUtc,
        int Version
    );
}
