using System.Collections.Concurrent;
using PAS.Calculation.Domain.FundPerformances;

namespace PAS.Calculation.Infrastructure.Persistence;

public sealed class InMemoryFundPerformanceRepository : IFundPerformanceRepository
{
    private readonly ConcurrentDictionary<Guid, FundPerformance> _funds = new();

    public Task<FundPerformance?> GetByIdAsync(
        Guid fundId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _funds.TryGetValue(fundId, out var fundPerformance);
        return Task.FromResult(fundPerformance);
    }

    public Task AddAsync(
        FundPerformance fundPerformance,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_funds.TryAdd(fundPerformance.Id, fundPerformance))
        {
            throw new InvalidOperationException(
                $"Les calculs du fonds '{fundPerformance.Id}' existent déjà.");
        }

        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
