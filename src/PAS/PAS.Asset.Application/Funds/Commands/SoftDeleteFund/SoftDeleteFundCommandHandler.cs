using MediatR;
using PAS.Asset.Domain.Funds;
using PAS.Common.Exceptions;

namespace PAS.Asset.Application.Funds.Commands.SoftDeleteFund {
    public sealed class SoftDeleteFundCommandHandler : IRequestHandler<SoftDeleteFundCommand> {

        private readonly IFundRepository _fundRepository;

        public SoftDeleteFundCommandHandler(IFundRepository fundRepository) {
            _fundRepository = fundRepository;
        }

        public async Task Handle(SoftDeleteFundCommand request, CancellationToken cancellationToken) {
            var fund = await _fundRepository.GetByIdAsync(
                request.FundId,
                cancellationToken);

            if (fund is null) {
                throw new NotFoundException(
                    $"Fund with ID '{request.FundId}' does not exist.");
            }

            fund.SoftDelete();

            await _fundRepository.SaveChangesAsync(cancellationToken);
        }
    }
}