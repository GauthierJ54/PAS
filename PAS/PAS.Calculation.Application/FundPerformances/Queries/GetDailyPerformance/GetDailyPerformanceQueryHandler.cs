using MediatR;
using PAS.Calculation.Application.FundPerformances.Models;
using PAS.Calculation.Domain.FundPerformances;

namespace PAS.Calculation.Application.FundPerformances.Queries.GetDailyPerformance;

public sealed class GetDailyPerformanceQueryHandler
    : IRequestHandler<GetDailyPerformanceQuery, DailyPerformanceDto?>
{
    private readonly IFundPerformanceRepository _repository;

    public GetDailyPerformanceQueryHandler(IFundPerformanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<DailyPerformanceDto?> Handle(
        GetDailyPerformanceQuery request,
        CancellationToken cancellationToken)
    {
        var fundPerformance = await _repository.GetByIdAsync(
            request.FundId,
            cancellationToken);

        var result = fundPerformance?.GetDailyPerformance(request.Date);

        return result is null
            ? null
            : new DailyPerformanceDto(
                request.FundId,
                result.Date,
                result.PreviousValue,
                result.CurrentValue,
                result.Rate);
    }
}
