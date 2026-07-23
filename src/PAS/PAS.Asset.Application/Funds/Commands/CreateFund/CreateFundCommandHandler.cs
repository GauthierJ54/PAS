using MediatR;
using PAS.Asset.Domain.Funds;
using PAS.Common.Exceptions;

namespace PAS.Asset.Application.Funds.Commands.CreateFund {
    public sealed class CreateFundCommandHandler : IRequestHandler<CreateFundCommand, Guid> {

        private readonly IFundRepository _fundRepository;

        public CreateFundCommandHandler(IFundRepository fundRepository) {
            _fundRepository = fundRepository;
        }

        public async Task<Guid> Handle(CreateFundCommand request, CancellationToken cancellationToken) {
            bool isinExists = await _fundRepository.ExistsByIsinAsync(request.Isin, cancellationToken);
            if (isinExists) {
                throw new NotFoundException($"Fund with ISIN '{request.Isin}' already exists.");
            }
            Fund fund = Fund.Create(request.Name, request.Isin, request.Currency);
            await _fundRepository.AddAsync(fund, cancellationToken);
            await _fundRepository.SaveChangesAsync(cancellationToken);
            return fund.Id;
        }
    }
}
