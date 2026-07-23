using PAS.Asset.Domain.Funds;
using PAS.Asset.Domain.Funds.Events;

namespace PAS.Asset.Domain.Tests;

public sealed class FundDomainEventTests
{
    [Fact]
    public void AddNav_TriggerFundNavAddedDomainEvent()
    {
        // Arrange
        var fund = Fund.Create("Test Fund", "LU1234567890", "EUR");
        var navValue = 150.50m;
        var navDate = DateTime.Parse("2025-01-15");

        // Act
        fund.AddNav(navValue, navDate);

        // Assert
        var events = fund.GetDomainEvents();
        Assert.Single(events);
        Assert.IsType<FundNavAddedDomainEvent>(events[0]);

        var domainEvent = Assert.IsType<FundNavAddedDomainEvent>(events[0]);
        Assert.Equal(fund.Id, domainEvent.FundId);
        Assert.Equal(navDate, domainEvent.Date);
        Assert.Equal(navValue, domainEvent.Value);
        Assert.NotEqual(Guid.Empty, domainEvent.EventId);
        Assert.True(domainEvent.OccurredAtUtc <= DateTime.UtcNow);
    }

    [Fact]
    public void UpdateNav_TriggerFundNavUpdatedDomainEvent()
    {
        // Arrange
        var fund = Fund.Create("Test Fund", "LU1234567890", "EUR");
        var navValue = 150.50m;
        var navDate = DateTime.Parse("2025-01-15");

        fund.AddNav(navValue, navDate);

        // Clear events from AddNav to test only UpdateNav
        fund.ClearDomainEvents();

        var newValue = 155.00m;

        // Act
        fund.UpdateNav(newValue, navDate);

        // Assert
        var events = fund.GetDomainEvents();
        Assert.Single(events);
        Assert.IsType<FundNavUpdatedDomainEvent>(events[0]);

        var domainEvent = Assert.IsType<FundNavUpdatedDomainEvent>(events[0]);
        Assert.Equal(fund.Id, domainEvent.FundId);
        Assert.Equal(navDate, domainEvent.Date);
        Assert.Equal(newValue, domainEvent.Value);
        Assert.NotEqual(Guid.Empty, domainEvent.EventId);
        Assert.True(domainEvent.OccurredAtUtc <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_DoesNotTriggerDomainEvent()
    {
        // Arrange
        var fund = Fund.Create("Test Fund", "LU1234567890", "EUR");

        // Assert
        var events = fund.GetDomainEvents();
        Assert.Empty(events);
    }

    [Fact]
    public void ClearDomainEvents_ClearsAllEvents()
    {
        // Arrange
        var fund = Fund.Create("Test Fund", "LU1234567890", "EUR");
        var navDate = DateTime.Parse("2025-01-15");

        fund.AddNav(100.0m, navDate);
        fund.UpdateNav(110.0m, navDate);

        Assert.Equal(2, fund.GetDomainEvents().Count);

        // Act
        fund.ClearDomainEvents();

        // Assert
        Assert.Empty(fund.GetDomainEvents());
    }
}
