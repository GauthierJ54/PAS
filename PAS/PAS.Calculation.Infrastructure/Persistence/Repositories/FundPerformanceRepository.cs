using Microsoft.EntityFrameworkCore;
using PAS.Calculation.Domain.FundPerformances;

namespace PAS.Calculation.Infrastructure.Persistence.Repositories;

public sealed class FundPerformanceRepository
    : IFundPerformanceRepository {
    private readonly CalculationDbContext _context;

    public FundPerformanceRepository(CalculationDbContext context) {
        _context = context;
    }

    public Task<FundPerformance?> GetByIdAsync(Guid fundId, CancellationToken cancellationToken) {
        return _context.FundPerformances
            .SingleOrDefaultAsync(
                f => f.Id == fundId,
                cancellationToken);
    }

    public Task AddAsync(FundPerformance fundPerformance, CancellationToken cancellationToken) {
        return _context.FundPerformances.AddAsync(
            fundPerformance,
            cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) {
        return _context.SaveChangesAsync(cancellationToken);
    }
}