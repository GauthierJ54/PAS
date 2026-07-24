namespace PAS.Contracts.Assets {
    public sealed record FundSoftDeleteIntegrationEvent(
        Guid EventId,
        Guid FundId,
        DateTimeOffset OccurredAtUtc,
        int Version
    );
}
