using MediatR;

namespace PAS.Asset.Application.Funds.Commands.SoftDeleteFundNav {
    public sealed record SoftDeleteFundNavCommand(Guid FundId, DateTime DateTime) : IRequest;
}
