using MediatR;
using PAS.Calculation.Application.FundPerformances.Commands.ProcessFundNavAdded;
using PAS.Calculation.Application.FundPerformances.Queries.GetDailyPerformance;
using PAS.Contracts.Assets;

namespace PAS.Calculation.Api.Endpoints;

public static class FundPerformanceEndpoints
{
    public static IEndpointRouteBuilder MapFundPerformanceEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/internal/events/fund-nav-added",
            async (
                FundNavAddedIntegrationEvent integrationEvent,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new ProcessFundNavAddedCommand(
                    integrationEvent.EventId,
                    integrationEvent.FundId,
                    integrationEvent.Date,
                    integrationEvent.Value,
                    integrationEvent.OccurredAtUtc,
                    integrationEvent.Version);

                await sender.Send(command, cancellationToken);

                return Results.Accepted(value: new
                {
                    integrationEvent.EventId
                });
            });

        endpoints.MapGet(
            "/funds/{fundId:guid}/performances/{date}",
            async (
                Guid fundId,
                DateOnly date,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var performance = await sender.Send(
                    new GetDailyPerformanceQuery(fundId, date),
                    cancellationToken);

                return performance is null
                    ? Results.NotFound(new
                    {
                        error = $"Aucune performance calculable pour le fonds '{fundId}' à la date {date}."
                    })
                    : Results.Ok(performance);
            });

        return endpoints;
    }
}
