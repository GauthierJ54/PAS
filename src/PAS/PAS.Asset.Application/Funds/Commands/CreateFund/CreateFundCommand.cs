using MediatR;

namespace PAS.Asset.Application.Funds.Commands.CreateFund {
    public sealed record CreateFundCommand(
        string Name,
        string Isin,
        string Currency) : IRequest<Guid>;
}
