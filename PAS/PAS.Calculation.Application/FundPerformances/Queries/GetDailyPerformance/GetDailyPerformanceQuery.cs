using MediatR;
using PAS.Calculation.Application.FundPerformances.Models;

namespace PAS.Calculation.Application.FundPerformances.Queries.GetDailyPerformance;

public sealed record GetDailyPerformanceQuery(Guid FundId, DateOnly Date)
    : IRequest<DailyPerformanceDto?>;
