using MediatR;
using PAS.Asset.Application.Abstractions.Messaging;

namespace PAS.Asset.Application.Funds.Commands.SoftDeleteFund {
    public sealed record SoftDeleteFundCommand(Guid FundId) : IRequest, IOutboxCommand;
}
