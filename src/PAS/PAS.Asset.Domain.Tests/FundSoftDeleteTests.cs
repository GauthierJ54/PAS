using PAS.Asset.Domain.Funds;
using PAS.Asset.Domain.Funds.Exceptions;

namespace PAS.Asset.Domain.Tests;

public sealed class FundSoftDeleteTests {

    [Fact]
    public void SoftDelete_MarksFundAsDeleted() {
        var fund = Fund.Create("Test Fund", "LU1234567890", "EUR");
        var beforeDeletion = DateTime.UtcNow;

        fund.SoftDelete();

        Assert.NotNull(fund.DeletedAtUtc);
        Assert.InRange(
            fund.DeletedAtUtc.Value,
            beforeDeletion,
            DateTime.UtcNow);
    }

    [Fact]
    public void SoftDelete_WhenFundIsAlreadyDeleted_Throws() {
        var fund = Fund.Create("Test Fund", "LU1234567890", "EUR");
        fund.SoftDelete();

        var exception = Assert.Throws<InvalidOperationException>(
            fund.SoftDelete);

        Assert.Equal("Le fonds est déjà supprimé.", exception.Message);
    }

    [Fact]
    public void SoftDeleteNav_MarksNavOfRequestedDayAsDeleted() {
        var fund = Fund.Create("Test Fund", "LU1234567890", "EUR");
        var navDate = new DateTime(2026, 7, 23, 8, 30, 0, DateTimeKind.Utc);
        fund.AddNav(100m, navDate);

        fund.SoftDeleteNav(navDate.Date.AddHours(18));

        var nav = Assert.Single(fund.Navs);
        Assert.NotNull(nav.DeletedAtUtc);
    }

    [Fact]
    public void SoftDeleteNav_WhenNavDoesNotExist_Throws() {
        var fund = Fund.Create("Test Fund", "LU1234567890", "EUR");
        var missingDate = new DateTime(2026, 7, 23);

        Assert.Throws<FundNavNotFoundException>(
            () => fund.SoftDeleteNav(missingDate));
    }

    [Fact]
    public void SoftDeleteNav_WhenNavIsAlreadyDeleted_Throws() {
        var fund = Fund.Create("Test Fund", "LU1234567890", "EUR");
        var navDate = new DateTime(2026, 7, 23);
        fund.AddNav(100m, navDate);
        fund.SoftDeleteNav(navDate);

        Assert.Throws<FundNavNotFoundException>(
            () => fund.SoftDeleteNav(navDate));
    }
}