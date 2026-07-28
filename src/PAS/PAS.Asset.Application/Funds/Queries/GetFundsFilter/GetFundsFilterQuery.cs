using MediatR;
using PAS.Asset.Application.Funds.Models;

namespace PAS.Asset.Application.Funds.Queries.GetFundsFilter {
    public sealed record GetFundsFilterQuery(
        string? Name,
        string? Isin,
        string? Currency,
        string? Status) : IRequest<IEnumerable<FundDto>>;
}
