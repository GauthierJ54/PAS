using PAS.Asset.Domain.Funds;

namespace PAS.Asset.Application.Funds.Models {
    public sealed record FundDto(
        Guid Id,
        string Name,
        string Isin,
        string Currency,
        FundStatus Status,
        IReadOnlyCollection<FundNavDto> Navs);
}
