using MediatR;
using PAS.Asset.Application.Abstractions;
using PAS.Asset.Application.Funds.Models;

namespace PAS.Asset.Application.Funds.Queries.GetFundById {
    public sealed class GetFundByIdQueryHandler : IRequestHandler<GetFundByIdQuery, FundDto?> {

        private readonly IQueryRepository _queryRepository;

        public GetFundByIdQueryHandler(IQueryRepository queryRepository) {
            _queryRepository = queryRepository;
        }

        public async Task<FundDto?> Handle(GetFundByIdQuery request, CancellationToken cancellationToken) {
            var fund = await _queryRepository.GetByIdAsync(request.id, cancellationToken).ConfigureAwait(false);
            return fund;
        }
    }
}
