namespace PAS.Calculation.Application.FundPerformances.Models;

public sealed record DailyPerformanceDto(
    Guid FundId,
    DateOnly Date,
    decimal PreviousValue,
    decimal CurrentValue,
    decimal Rate);
