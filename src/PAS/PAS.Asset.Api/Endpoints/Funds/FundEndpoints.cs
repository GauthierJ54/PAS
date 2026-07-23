using MediatR;
using PAS.Asset.Application.Funds.Commands.AddFundNav;
using PAS.Asset.Application.Funds.Commands.CreateFund;
using PAS.Asset.Application.Funds.Commands.SoftDeleteFund;
using PAS.Asset.Application.Funds.Commands.SoftDeleteFundNav;
using PAS.Asset.Application.Funds.Queries.GetAllFunds;
using PAS.Asset.Application.Funds.Queries.GetFundById;

namespace PAS.Asset.Api.Endpoints.Funds;

public static class FundEndpoints {
    public static IEndpointRouteBuilder MapFundEndpoints(this IEndpointRouteBuilder endpoints) {

        endpoints.MapGet("/fund/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) => {
            var fund = await sender.Send(new GetFundByIdQuery(id), cancellationToken);

            return Results.Ok(fund);
        }).WithName("GetFundByID");

        endpoints.MapGet("/funds", async (ISender sender, CancellationToken cancellationToken) => {
            var funds = await sender.Send(new GetAllFundsQuery(), cancellationToken);

            return Results.Ok(funds);
        });

        endpoints.MapPost("/fund", async (CreateFundRequest request, ISender sender, CancellationToken cancellationToken) => {
            var fund = new CreateFundCommand(request.name, request.isin, request.currency);

            var id = await sender.Send(fund, cancellationToken);

            return Results.CreatedAtRoute(routeName: "GetFundById", routeValues: new { id }, value: new { id });
        });

        endpoints.MapPut("/fundNav/{id:guid}", async (Guid id, AddFundNavRequest request, ISender sender, CancellationToken cancellationToken) => {
                var fundNav = new AddFundNavCommand(id, request.value, request.date);

                var fund = await sender.Send(fundNav, cancellationToken);

                return Results.CreatedAtRoute(routeName: "GetFundById", routeValues: new { id }, value: new { id });
        });

        endpoints.MapPatch("/delete/fund/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) => {
            var fund = new SoftDeleteFundCommand(id);

            await sender.Send(fund, cancellationToken);

            return Results.Ok(new { id });
        });

        endpoints.MapPatch("/delete/fundNav/{id:guid}/{date:datetime}", async (Guid id, DateTime date, ISender sender, CancellationToken cancellationToken) => {
            var fundNav = new SoftDeleteFundNavCommand(id, date);

            await sender.Send(fundNav, cancellationToken);

            return Results.CreatedAtRoute(routeName: "GetFundById", routeValues: new { id }, value: new { id });
        });

        return endpoints;
    }
}