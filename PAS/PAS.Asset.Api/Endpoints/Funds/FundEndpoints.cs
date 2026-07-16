using MediatR;
using PAS.Asset.Application.Funds.Commands.AddFundNav;
using PAS.Asset.Application.Funds.Commands.CreateFund;
using PAS.Asset.Application.Funds.Queries.GetAllFunds;
using PAS.Asset.Application.Funds.Queries.GetFundById;

namespace PAS.Asset.Api.Endpoints.Funds;

public static class FundEndpoints {
    public static IEndpointRouteBuilder MapFundEndpoints(
        this IEndpointRouteBuilder endpoints) {

        endpoints.MapGet("/funds/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) => {
            var fund = await sender.Send(new GetFundByIdQuery(id), cancellationToken);

            return Results.Ok(fund);
        });

        endpoints.MapGet("/funds", async (ISender sender, CancellationToken cancellationToken) => {
            var funds = await sender.Send(new GetAllFundsQuery(), cancellationToken);

            return Results.Ok(funds);
        });

        endpoints.MapPost("/funds", async (
            CreateFundRequest request,
            ISender sender,
            CancellationToken cancellationToken) => {
                var fund = new CreateFundCommand(request.name, request.isin, request.currency);

                var id = await sender.Send(fund, cancellationToken);

                return Results.Created($"/funds/{id}", new { id });
            });

        endpoints.MapPut("/fundNav", async (
            AddFundNavRequest request,
            ISender sender,
            CancellationToken cancellationToken) => {
                var fundNav = new AddFundNavCommand(request.fundId, request.value, request.date);

                var fund = await sender.Send(fundNav, cancellationToken);

                return Results.Ok(fund);
            });

        return endpoints;
    }
}