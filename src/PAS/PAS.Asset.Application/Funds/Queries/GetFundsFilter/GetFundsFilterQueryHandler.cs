using MediatR;
using PAS.Asset.Application.Abstractions;
using PAS.Asset.Application.Funds.Models;
using PAS.Asset.Domain.Funds;
using PAS.Common.Exceptions;

namespace PAS.Asset.Application.Funds.Queries.GetFundsFilter {
    public sealed class GetFundsFilterQueryHandler : IRequestHandler<GetFundsFilterQuery, IEnumerable<FundDto>> {

        private readonly IQueryRepository _queryRepository;

        public GetFundsFilterQueryHandler(IQueryRepository queryRepository) {
            _queryRepository = queryRepository;
        }

        public async Task<IEnumerable<FundDto>> Handle(GetFundsFilterQuery request, CancellationToken cancellationToken) {

            FundStatus? status = null;

            if (!string.IsNullOrWhiteSpace(request.Status)) {
                if (!Enum.TryParse<FundStatus>(request.Status, ignoreCase: true, out var parsedStatus)) {

                    throw new BadRequestException($"Invalid fund status: {request.Status}");
                }

                status = parsedStatus;
            }

            return await _queryRepository.GetAllFilterAsync(request.Name, request.Isin, request.Currency, status, cancellationToken);
        }
    }
}
