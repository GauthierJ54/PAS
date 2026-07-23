using PAS.Calculation.Domain.FundPerformances;

namespace PAS.Calculation.Application.Tests.Fakes;

internal sealed class FakeFundPerformanceRepository : IFundPerformanceRepository {
    private readonly Dictionary<Guid, FundPerformance> _funds = [];

    public int AddCallCount { get; private set; }
    public int SaveChangesCallCount { get; private set; }

    public Task<FundPerformance?> GetByIdAsync(
        Guid fundId,
        CancellationToken cancellationToken) {
        _funds.TryGetValue(fundId, out var fundPerformance);
        return Task.FromResult(fundPerformance);
    }

    public Task AddAsync(
        FundPerformance fundPerformance,
        CancellationToken cancellationToken) {
        _funds.Add(fundPerformance.Id, fundPerformance);
        AddCallCount++;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) {
        SaveChangesCallCount++;
        return Task.CompletedTask;
    }

    public void Seed(FundPerformance fundPerformance) {
        _funds.Add(fundPerformance.Id, fundPerformance);
    }
}
