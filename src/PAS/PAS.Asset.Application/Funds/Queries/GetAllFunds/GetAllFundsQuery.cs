using MediatR;
using PAS.Asset.Application.Funds.Models;

namespace PAS.Asset.Application.Funds.Queries.GetAllFunds {
    public sealed record GetAllFundsQuery : IRequest<IEnumerable<FundDto>>;
}
