namespace PAS.Asset.Api.Endpoints.Funds {
    public sealed record AddFundNavRequest(
        Guid fundId,
        decimal value,
        DateTime date);
}
