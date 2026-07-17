namespace PAS.Calculation.Domain.FundPerformances {
    public sealed record PerformanceResult(
        DateOnly Date,
        decimal PreviousValue,
        decimal CurrentValue,
        decimal Rate);
}
