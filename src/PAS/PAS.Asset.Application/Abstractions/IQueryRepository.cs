using PAS.Asset.Application.Funds.Models;
using PAS.Asset.Domain.Funds;

namespace PAS.Asset.Application.Abstractions {
    public interface IQueryRepository {

        Task<FundDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<FundDto>> GetAllAsync(CancellationToken cancellationToken);

        Task<IEnumerable<FundDto>> GetAllFilterAsync(string? name, string? isin, string? currency, FundStatus? fundStatus, CancellationToken cancellationToken);
    }
}
