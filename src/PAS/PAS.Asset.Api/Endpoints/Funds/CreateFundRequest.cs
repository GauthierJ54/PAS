namespace PAS.Asset.Api.Endpoints.Funds {
    public sealed record CreateFundRequest(
        string name,
        string isin,
        string currency);
}
