using PAS.Calculation.Domain.FundPerformances;

namespace PAS.Calculation.Domain.Tests;

public sealed class FundPerformanceTests {
    // ─────────────────────────────────────────────
    // 1. Création avec FundId valide
    // ─────────────────────────────────────────────
    [Fact]
    public void Create_WithValidFundId_CreatesFundPerformance() {
        // Arrange
        var fundId = Guid.NewGuid();

        // Act
        var result = FundPerformance.Create(fundId);

        // Assert
        Assert.Equal(fundId, result.Id);
        Assert.Empty(result.Navs);
    }

    // ─────────────────────────────────────────────
    // 2. Création avec Guid.Empty doit échouer
    // ─────────────────────────────────────────────
    [Fact]
    public void Create_WithGuidEmpty_ThrowsArgumentException() {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => FundPerformance.Create(Guid.Empty));
    }

    // ─────────────────────────────────────────────
    // 3. Ajout valide d'une NAV
    // ─────────────────────────────────────────────
    [Fact]
    public void AddNav_ValidValueAndDate_AddsNavSuccessfully() {
        // Arrange
        var fundId = Guid.NewGuid();
        var fundPerformance = FundPerformance.Create(fundId);
        var navValue = 150.50m;
        var navDate = DateOnly.FromDateTime(DateTime.Parse("2025-01-15"));

        // Act
        fundPerformance.AddNav(navValue, navDate);

        // Assert
        Assert.Single(fundPerformance.Navs);
        var nav = Assert.Single(fundPerformance.Navs);
        Assert.Equal(navValue, nav.Value);
        Assert.Equal(navDate, nav.Date);
    }

    // ─────────────────────────────────────────────
    // 4. Ajout d'une NAV avec valeur nulle ou négative
    // ─────────────────────────────────────────────
    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void AddNav_ZeroOrNegativeValue_ThrowsArgumentOutOfRangeException(decimal value) {
        // Arrange
        var fundPerformance = FundPerformance.Create(Guid.NewGuid());
        var navDate = DateOnly.FromDateTime(DateTime.Parse("2025-01-15"));

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => fundPerformance.AddNav(value, navDate));
    }

    [Fact]
    public void AddNav_NegativeFractionalValue_ThrowsArgumentOutOfRangeException() {
        // Arrange
        var fundPerformance = FundPerformance.Create(Guid.NewGuid());
        var navDate = DateOnly.FromDateTime(DateTime.Parse("2025-01-15"));

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => fundPerformance.AddNav(-0.01m, navDate));
    }

    // ─────────────────────────────────────────────
    // 5. Ajout d'une NAV avec date doublée
    // ─────────────────────────────────────────────
    [Fact]
    public void AddNav_DuplicateDate_ThrowsInvalidOperationException() {
        // Arrange
        var fundPerformance = FundPerformance.Create(Guid.NewGuid());
        var navDate = DateOnly.FromDateTime(DateTime.Parse("2025-01-15"));

        fundPerformance.AddNav(100.0m, navDate);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => fundPerformance.AddNav(110.0m, navDate));
    }

    // ─────────────────────────────────────────────
    // 6. Mise à jour d'une NAV existante
    // ─────────────────────────────────────────────
    [Fact]
    public void UpdateNav_ExistingDate_UpdatesNavSuccessfully() {
        // Arrange
        var fundPerformance = FundPerformance.Create(Guid.NewGuid());
        var navDate = DateOnly.FromDateTime(DateTime.Parse("2025-01-15"));

        fundPerformance.AddNav(100.0m, navDate);

        // Act
        fundPerformance.CorrectNav(105.0m, navDate);

        // Assert
        var nav = Assert.Single(fundPerformance.Navs);
        Assert.Equal(105.0m, nav.Value);
        Assert.Equal(navDate, nav.Date);
    }

    // ─────────────────────────────────────────────
    // 7. Mise à jour d'une NAV avec date inexistante
    // ─────────────────────────────────────────────
    [Fact]
    public void CorrectNav_NonExistingDate_ThrowsInvalidOperationException() {
        var fundPerformance = FundPerformance.Create(Guid.NewGuid());
        var navDate = new DateOnly(2025, 1, 15);

        Action action = () =>
            fundPerformance.CorrectNav(105m, navDate);

        Assert.Throws<InvalidOperationException>(action);
        Assert.Empty(fundPerformance.Navs);
    }

    // ─────────────────────────────────────────────
    // 8. Première NAV — pas de performance calculable
    // ─────────────────────────────────────────────
    [Fact]
    public void GetDailyPerformance_FirstNav_ReturnsNull() {
        // Arrange
        var fundPerformance = FundPerformance.Create(Guid.NewGuid());
        var navDate = DateOnly.FromDateTime(DateTime.Parse("2025-01-15"));

        fundPerformance.AddNav(100.0m, navDate);

        // Act
        var result = fundPerformance.GetDailyPerformance(navDate);

        // Assert
        Assert.Null(result);
    }

    // ─────────────────────────────────────────────
    // 9. Performance positive
    // ─────────────────────────────────────────────
    [Fact]
    public void GetDailyPerformance_Positive_ReturnsCorrectRate() {
        // Arrange
        var fundPerformance = FundPerformance.Create(Guid.NewGuid());
        var date1 = DateOnly.FromDateTime(DateTime.Parse("2025-01-01"));
        var date2 = DateOnly.FromDateTime(DateTime.Parse("2025-01-02"));

        fundPerformance.AddNav(100.0m, date1);
        fundPerformance.AddNav(110.0m, date2);

        // Act
        var result = fundPerformance.GetDailyPerformance(date2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100.0m, result.PreviousValue);
        Assert.Equal(110.0m, result.CurrentValue);
        Assert.Equal(0.10m, result.Rate); // 10%
    }

    // ─────────────────────────────────────────────
    // 10. Performance négative
    // ─────────────────────────────────────────────
    [Fact]
    public void GetDailyPerformance_Negative_ReturnsCorrectRate() {
        // Arrange
        var fundPerformance = FundPerformance.Create(Guid.NewGuid());
        var date1 = DateOnly.FromDateTime(DateTime.Parse("2025-01-01"));
        var date2 = DateOnly.FromDateTime(DateTime.Parse("2025-01-02"));

        fundPerformance.AddNav(100.0m, date1);
        fundPerformance.AddNav(80.0m, date2);

        // Act
        var result = fundPerformance.GetDailyPerformance(date2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100.0m, result.PreviousValue);
        Assert.Equal(80.0m, result.CurrentValue);
        Assert.Equal(-0.20m, result.Rate); // -20%
    }

    // ─────────────────────────────────────────────
    // 11. Dates non consécutives (week-end)
    // ─────────────────────────────────────────────
    [Fact]
    public void GetDailyPerformance_NonConsecutiveDates_ReturnsCorrectRate() {
        // Arrange
        var fundPerformance = FundPerformance.Create(Guid.NewGuid());
        // 2025-01-03 (vendredi) et 2025-01-06 (lundi) — week-end entre les deux
        var date1 = DateOnly.FromDateTime(DateTime.Parse("2025-01-03"));
        var date2 = DateOnly.FromDateTime(DateTime.Parse("2025-01-06"));

        fundPerformance.AddNav(100.0m, date1);
        fundPerformance.AddNav(105.0m, date2);

        // Act
        var result = fundPerformance.GetDailyPerformance(date2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100.0m, result.PreviousValue); // vendredi, pas lundi précédent
        Assert.Equal(105.0m, result.CurrentValue);
        Assert.Equal(0.05m, result.Rate); // 5%
    }

    // ─────────────────────────────────────────────
    // 12. NAV reçues dans le désordre
    // ─────────────────────────────────────────────
    [Fact]
    public void GetDailyPerformance_OutOfOrderNavs_ReturnsCorrectRate() {
        // Arrange
        var fundPerformance = FundPerformance.Create(Guid.NewGuid());
        var date1 = DateOnly.FromDateTime(DateTime.Parse("2025-01-01"));
        var date2 = DateOnly.FromDateTime(DateTime.Parse("2025-01-02"));
        var date3 = DateOnly.FromDateTime(DateTime.Parse("2025-01-03"));

        // Ajout dans le désordre : d'abord le milieu, puis le début, puis la fin
        fundPerformance.AddNav(100.0m, date2);  // milieu d'abord
        fundPerformance.AddNav(90.0m, date1);   // début ensuite
        fundPerformance.AddNav(110.0m, date3);  // fin en dernier

        // Act — performance de date3 (la plus récente)
        var result3 = fundPerformance.GetDailyPerformance(date3);

        // Act — performance de date2
        var result2 = fundPerformance.GetDailyPerformance(date2);

        // Assert
        Assert.NotNull(result3);
        Assert.Equal(100.0m, result3.PreviousValue); // date2 est la NAV précédente de date3
        Assert.Equal(110.0m, result3.CurrentValue);
        Assert.Equal(0.10m, result3.Rate);

        Assert.NotNull(result2);
        Assert.Equal(90.0m, result2.PreviousValue); // date1 est la NAV précédente de date2
        Assert.Equal(100.0m, result2.CurrentValue);
        Assert.Equal(0.111111111111111m, result2.Rate, precision: 12); // ~11.11%
    }
}
