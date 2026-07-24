using MediatR;
using PAS.Asset.Domain.Funds;
using PAS.Common.Exceptions;

namespace PAS.Asset.Application.Funds.Commands.UpdateFundSatus {
    public sealed class UpdateFundStatusCommandHandler : IRequestHandler<UpdateFundStatusCommand, Guid> {

        private readonly IFundRepository _fundRepository;

        public UpdateFundStatusCommandHandler(IFundRepository fundRepository) {
            _fundRepository = fundRepository;
        }

        public async Task<Guid> Handle(UpdateFundStatusCommand request, CancellationToken cancellationToken) {
            var fund = await _fundRepository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

            if (fund == null) throw new NotFoundException($"Fund with Id {request.Id} not found.");
            if (Enum.TryParse<FundStatus>(request.Status, true, out var status) == false) {
                throw new BadRequestException($"Invalid status value: {request.Status}");
            }

            fund.UpdateStatus(status);
            await _fundRepository.SaveChangesAsync(cancellationToken);
            return fund.Id;
        }
    }
}
