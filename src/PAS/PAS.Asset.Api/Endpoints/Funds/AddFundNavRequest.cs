namespace PAS.Asset.Api.Endpoints.Funds {
    public sealed record AddFundNavRequest(
        decimal value,
        DateTime date);
}
