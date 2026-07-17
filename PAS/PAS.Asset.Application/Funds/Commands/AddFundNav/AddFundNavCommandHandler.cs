using MediatR;
using PAS.Asset.Domain.Funds;
using PAS.Common.Exceptions;

namespace PAS.Asset.Application.Funds.Commands.AddFundNav {
    public sealed class AddFundNavCommandHandler : IRequestHandler<AddFundNavCommand, Fund?> {

        private readonly IFundRepository _fundRepository;

        public AddFundNavCommandHandler(IFundRepository fundRepository) {
            _fundRepository = fundRepository;
        }

        public async Task<Fund?> Handle(AddFundNavCommand request, CancellationToken cancellationToken) {
            Fund? fund = await _fundRepository.GetByIdWithNavOfDayAsync(request.fundId, request.date, cancellationToken);
            if (fund == null) {
                throw new NotFoundException($"Fund with ID '{request.fundId}' is not exists.");
            }
            fund.AddNav(request.value, request.date);
            await _fundRepository.UpdateAsync(fund, cancellationToken);
            await _fundRepository.SaveChangesAsync(cancellationToken);
            return fund;
        }
    }
}
