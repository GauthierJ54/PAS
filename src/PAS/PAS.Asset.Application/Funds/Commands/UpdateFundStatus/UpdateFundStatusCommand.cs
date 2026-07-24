using MediatR;

namespace PAS.Asset.Application.Funds.Commands.UpdateFundSatus {
    public sealed record UpdateFundStatusCommand(Guid Id, string Status) : IRequest<Guid>;
}
