using PAS.Calculation.Application.FundPerformances.Queries.GetDailyPerformance;
using PAS.Calculation.Application.Tests.Fakes;
using PAS.Calculation.Domain.FundPerformances;

namespace PAS.Calculation.Application.Tests.FundPerformances;

public sealed class GetDailyPerformanceQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithTwoNavs_ReturnsDailyPerformance()
    {
        var repository = new FakeFundPerformanceRepository();
        var fund = FundPerformance.Create(Guid.NewGuid());
        fund.AddNav(100m, new DateOnly(2026, 7, 16));
        fund.AddNav(105m, new DateOnly(2026, 7, 17));
        repository.Seed(fund);
        var handler = new GetDailyPerformanceQueryHandler(repository);

        var result = await handler.Handle(
            new GetDailyPerformanceQuery(fund.Id, new DateOnly(2026, 7, 17)),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(fund.Id, result.FundId);
        Assert.Equal(100m, result.PreviousValue);
        Assert.Equal(105m, result.CurrentValue);
        Assert.Equal(0.05m, result.Rate);
    }

    [Fact]
    public async Task Handle_WhenPerformanceCannotBeCalculated_ReturnsNull()
    {
        var repository = new FakeFundPerformanceRepository();
        var handler = new GetDailyPerformanceQueryHandler(repository);

        var result = await handler.Handle(
            new GetDailyPerformanceQuery(Guid.NewGuid(), new DateOnly(2026, 7, 17)),
            CancellationToken.None);

        Assert.Null(result);
    }
}
