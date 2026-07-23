using MediatR;
using PAS.Asset.Application.Funds.Models;

namespace PAS.Asset.Application.Funds.Queries.GetFundById {
    public sealed record GetFundByIdQuery(Guid id) : IRequest<FundDto?>;
}
