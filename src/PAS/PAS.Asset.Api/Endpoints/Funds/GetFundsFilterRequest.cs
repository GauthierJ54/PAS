namespace PAS.Asset.Api.Endpoints.Funds {
    public sealed record GetFundsFilterRequest(
        string? Name,
        string? Isin,
        string? Currency,
        string? Status);
}
