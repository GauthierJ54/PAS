using Microsoft.EntityFrameworkCore;
using PAS.Asset.Application.Abstractions;
using PAS.Asset.Application.Funds.Models;

namespace PAS.Asset.Infrastructure.Persistence.Repositories {
    public sealed class QueryRepository : IQueryRepository {

        private readonly AssetDbContext _context;

        public QueryRepository(AssetDbContext assetDbContext) {
            _context = assetDbContext;
        }

        public async Task<IEnumerable<FundDto>> GetAllAsync(CancellationToken cancellationToken) {
            return await _context.Funds.Where(f => f.DeletedAtUtc == null).AsNoTracking().Select(f => new FundDto(
                f.Id,
                f.Name,
                f.Isin,
                f.Currency,
                f.Status,
                f.Navs.Where(n => n.DeletedAtUtc == null).OrderByDescending(n => n.Date).Select(n => new FundNavDto(
                    n.Value,
                    DateOnly.FromDateTime(n.Date)
                )).ToList()
            )).ToListAsync(cancellationToken);
        }

        public async Task<FundDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken) {
            return await _context.Funds.AsNoTracking().Where(f => f.Id == id && f.DeletedAtUtc == null).Select(f => new FundDto(
                f.Id,
                f.Name,
                f.Isin,
                f.Currency,
                f.Status,
                f.Navs.Where(n => n.DeletedAtUtc == null).OrderByDescending(n => n.Date).Select(n => new FundNavDto(n.Value, DateOnly.FromDateTime(n.Date))).ToList()))
            .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
