namespace PAS.Calculation.Domain.FundPerformances;

public interface IFundPerformanceRepository {
    Task<FundPerformance?> GetByIdAsync(Guid fundId, CancellationToken cancellationToken);

    Task AddAsync(FundPerformance fundPerformance, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
