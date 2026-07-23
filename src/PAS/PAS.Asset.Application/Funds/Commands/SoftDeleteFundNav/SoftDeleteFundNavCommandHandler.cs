using MediatR;
using PAS.Asset.Domain.Funds;
using PAS.Common.Exceptions;

namespace PAS.Asset.Application.Funds.Commands.SoftDeleteFundNav {
    public sealed class SoftDeleteFundNavCommandHandler : IRequestHandler<SoftDeleteFundNavCommand> {

        private readonly IFundRepository _fundRepository;

        public SoftDeleteFundNavCommandHandler(IFundRepository fundRepository) {
            _fundRepository = fundRepository;
        }

        public async Task Handle(SoftDeleteFundNavCommand request, CancellationToken cancellationToken) {
            var fund = await _fundRepository.GetByIdWithNavOfDayAsync(
                request.FundId,
                request.DateTime,
                cancellationToken);

            if (fund is null) {
                throw new NotFoundException(
                    $"Fund with ID '{request.FundId}' does not exist.");
            }

            fund.SoftDeleteNav(request.DateTime);

            await _fundRepository.SaveChangesAsync(cancellationToken);
        }
    }
}