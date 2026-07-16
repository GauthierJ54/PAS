using MediatR;
using PAS.Asset.Application.Abstractions;
using PAS.Asset.Application.Funds.Models;

namespace PAS.Asset.Application.Funds.Queries.GetAllFunds {
    public sealed class GetAllFundsQueryHandler : IRequestHandler<GetAllFundsQuery, IEnumerable<FundDto>> {

        private readonly IQueryRepository _queryRepository;

        public GetAllFundsQueryHandler(IQueryRepository queryRepository) {
            _queryRepository = queryRepository;
        }

        public async Task<IEnumerable<FundDto>> Handle(GetAllFundsQuery request, CancellationToken cancellationToken) {
            var funds = await _queryRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
            return funds;
        }
    }
}
