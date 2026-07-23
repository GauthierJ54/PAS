using MediatR;

namespace PAS.Asset.Application.Funds.Commands.SoftDeleteFund {
    public sealed record SoftDeleteFundCommand(Guid FundId) : IRequest;
}
