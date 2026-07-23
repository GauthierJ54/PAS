using MediatR;
using PAS.Asset.Domain.Funds;

namespace PAS.Asset.Application.Funds.Commands.AddFundNav {
    public sealed record AddFundNavCommand(
        Guid fundId,
        decimal value,
        DateTime date) : IRequest<Fund?>;
}
