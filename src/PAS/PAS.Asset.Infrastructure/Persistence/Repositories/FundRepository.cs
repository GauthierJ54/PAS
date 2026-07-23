using Microsoft.EntityFrameworkCore;
using PAS.Asset.Domain.Funds;

namespace PAS.Asset.Infrastructure.Persistence.Repositories {
    public sealed class FundRepository : IFundRepository {

        private readonly AssetDbContext _context;

        public FundRepository(AssetDbContext assetDbContext) {
            _context = assetDbContext;
        }

        public async Task AddAsync(Fund fund, CancellationToken cancellationToken) {
            await _context.Funds.AddAsync(fund, cancellationToken);
        }

        public Task<bool> ExistsByIsinAsync(string isin, CancellationToken cancellationToken) {
            return _context.Funds.AnyAsync(f => f.Isin == isin && f.DeletedAtUtc == null, cancellationToken);
        }

        public Task<Fund?> GetByIdAsync(Guid id, CancellationToken cancellationToken) {
            return _context.Funds
                .FirstOrDefaultAsync(
                    f => f.Id == id && f.DeletedAtUtc == null,
                    cancellationToken);
        }

        public Task<Fund?> GetByIdWithNavOfDayAsync(Guid id, DateTime date, CancellationToken cancellationToken) {
            var start = date.Date;
            var end = start.AddDays(1);

            return _context.Funds
                .Include(f => f.Navs.Where(
                    n => n.Date >= start &&
                         n.Date < end))
                .FirstOrDefaultAsync(
                    f => f.Id == id && f.DeletedAtUtc == null,
                    cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken) {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}