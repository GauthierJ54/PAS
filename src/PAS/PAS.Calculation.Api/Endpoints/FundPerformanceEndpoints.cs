using MediatR;
using PAS.Calculation.Application.FundPerformances.Queries.GetDailyPerformance;

namespace PAS.Calculation.Api.Endpoints;

public static class FundPerformanceEndpoints {
    public static IEndpointRouteBuilder MapFundPerformanceEndpoints(this IEndpointRouteBuilder endpoints) {

        endpoints.MapGet("/funds/{fundId:guid}/performances/{date}",async (Guid fundId, DateOnly date, ISender sender, CancellationToken cancellationToken) => {
                    var performance = await sender.Send(new GetDailyPerformanceQuery(fundId, date), cancellationToken);

                    return performance is null
                        ? Results.NotFound(new {error = $"Aucune performance calculable pour le fonds '{fundId}' à la date {date}."})
                        : Results.Ok(performance);
                });

        return endpoints;
    }
}
