using MediatR;
using PAS.Asset.Application.Funds.Commands.AddFundNav;
using PAS.Asset.Application.Funds.Commands.CreateFund;
using PAS.Asset.Application.Funds.Commands.SoftDeleteFund;
using PAS.Asset.Application.Funds.Commands.SoftDeleteFundNav;
using PAS.Asset.Application.Funds.Commands.UpdateFundSatus;
using PAS.Asset.Application.Funds.Models;
using PAS.Asset.Application.Funds.Queries.GetAllFunds;
using PAS.Asset.Application.Funds.Queries.GetFundById;

namespace PAS.Asset.Api.Endpoints.Funds;

public static class FundEndpoints {
    public static IEndpointRouteBuilder MapFundEndpoints(this IEndpointRouteBuilder endpoints) {

        var fundGroup = endpoints.MapGroup("")
            .WithTags("Funds")
            .RequireAuthorization();

        fundGroup.MapGet("/fund/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) => {
            var fund = await sender.Send(new GetFundByIdQuery(id), cancellationToken);

            return Results.Ok(fund);
        }).WithName("GetFundById")
          .Produces<FundDto>();

        fundGroup.MapGet("/funds", async (ISender sender, CancellationToken cancellationToken) => {
            var funds = await sender.Send(new GetAllFundsQuery(), cancellationToken);

            return Results.Ok(funds);
        }).WithName("GetAllFunds")
          .Produces<IReadOnlyCollection<FundDto>>();

        fundGroup.MapPost("/fund", async (CreateFundRequest request, ISender sender, CancellationToken cancellationToken) => {
            var fund = new CreateFundCommand(request.name, request.isin, request.currency);

            var id = await sender.Send(fund, cancellationToken);

            return Results.CreatedAtRoute(routeName: "GetFundById", routeValues: new { id }, value: new { id });
        }).WithName("CreateFund")
          .Produces<FundIdResponse>(StatusCodes.Status201Created);

        fundGroup.MapPut("/fundNav/{id:guid}", async (Guid id, AddFundNavRequest request, ISender sender, CancellationToken cancellationToken) => {
                var fundNav = new AddFundNavCommand(id, request.value, request.date);

                var fund = await sender.Send(fundNav, cancellationToken);

                return Results.CreatedAtRoute(routeName: "GetFundById", routeValues: new { id }, value: new { id });
        }).WithName("AddFundNav")
          .Produces<FundIdResponse>(StatusCodes.Status201Created);

        fundGroup.MapPatch("/delete/fund/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) => {
            var fund = new SoftDeleteFundCommand(id);

            await sender.Send(fund, cancellationToken);

            return Results.Ok(new { id });
        }).WithName("SoftDeleteFund")
          .Produces<FundIdResponse>();

        fundGroup.MapPatch("/delete/fundNav/{id:guid}/{date:datetime}", async (Guid id, DateTime date, ISender sender, CancellationToken cancellationToken) => {
            var fundNav = new SoftDeleteFundNavCommand(id, date);

            await sender.Send(fundNav, cancellationToken);

            return Results.CreatedAtRoute(routeName: "GetFundById", routeValues: new { id }, value: new { id });
        }).WithName("SoftDeleteFundNav")
          .Produces<FundIdResponse>(StatusCodes.Status201Created);

        fundGroup.MapPatch("/status/{id:guid}", async (Guid id, string status, ISender sender, CancellationToken cancellationToken) => {
            var fund = new UpdateFundStatusCommand(id, status);
            await sender.Send(fund, cancellationToken);
            return Results.Ok(new { id });
        }).WithName("UpdateFundStatus")
          .Produces<FundIdResponse>();

        return endpoints;
    }
}


