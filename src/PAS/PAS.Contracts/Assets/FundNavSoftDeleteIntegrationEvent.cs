namespace PAS.Contracts.Assets {
    public sealed record FundNavSoftDeleteIntegrationEvent(
        Guid EventId,
        Guid FundId,
        DateOnly Date,
        DateTimeOffset OccurredAtUtc,
        int Version
    );
}
