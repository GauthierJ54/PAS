using PAS.Calculation.Application.FundPerformances.Commands.ProcessFundNavAdded;
using PAS.Calculation.Application.Tests.Fakes;
using PAS.Calculation.Domain.FundPerformances;

namespace PAS.Calculation.Application.Tests.FundPerformances;

public sealed class ProcessFundNavAddedCommandHandlerTests {
    [Fact]
    public async Task Handle_WhenFundDoesNotExist_CreatesFundAndAddsNav() {
        var repository = new FakeFundPerformanceRepository();
        var handler = new ProcessFundNavAddedCommandHandler(repository);
        var fundId = Guid.NewGuid();
        var date = new DateOnly(2026, 7, 17);

        await handler.Handle(CreateCommand(fundId, date, 100m), CancellationToken.None);

        var storedFund = await repository.GetByIdAsync(fundId, CancellationToken.None);
        Assert.NotNull(storedFund);
        var nav = Assert.Single(storedFund.Navs);
        Assert.Equal(date, nav.Date);
        Assert.Equal(100m, nav.Value);
        Assert.Equal(1, repository.AddCallCount);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenFundExists_AddsNavWithoutAddingAggregateAgain() {
        var repository = new FakeFundPerformanceRepository();
        var fund = FundPerformance.Create(Guid.NewGuid());
        repository.Seed(fund);
        var handler = new ProcessFundNavAddedCommandHandler(repository);

        await handler.Handle(
            CreateCommand(fund.Id, new DateOnly(2026, 7, 17), 100m),
            CancellationToken.None);

        Assert.Single(fund.Navs);
        Assert.Equal(0, repository.AddCallCount);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WithDuplicateDate_ThrowsAndDoesNotSave() {
        var repository = new FakeFundPerformanceRepository();
        var fund = FundPerformance.Create(Guid.NewGuid());
        var date = new DateOnly(2026, 7, 17);
        fund.AddNav(100m, date);
        repository.Seed(fund);
        var handler = new ProcessFundNavAddedCommandHandler(repository);

        Task Action() => handler.Handle(
            CreateCommand(fund.Id, date, 105m),
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(Action);
        Assert.Equal(0, repository.SaveChangesCallCount);
    }

    private static ProcessFundNavAddedCommand CreateCommand(
        Guid fundId,
        DateOnly date,
        decimal value) {
        return new ProcessFundNavAddedCommand(
            Guid.NewGuid(),
            fundId,
            date,
            value,
            DateTimeOffset.UtcNow,
            1);
    }
}
