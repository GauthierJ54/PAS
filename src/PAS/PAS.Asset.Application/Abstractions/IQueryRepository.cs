using PAS.Asset.Application.Funds.Models;

namespace PAS.Asset.Application.Abstractions {
    public interface IQueryRepository {

        Task<FundDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<FundDto>> GetAllAsync(CancellationToken cancellationToken);
    }
}
