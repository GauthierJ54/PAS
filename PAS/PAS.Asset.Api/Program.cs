using PAS.Asset.Api.Endpoints.Funds;
using PAS.Asset.Application.Funds.Commands.CreateFund;
using PAS.Asset.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI
builder.Services.AddOpenApi();

// MediatR - register handlers from the application assembly that contains CreateFundCommand
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateFundCommand>());

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapFundEndpoints();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();

    app.MapScalarApiReference(options => {
        options.Title = "PAS - Asset API";
        options.Theme = ScalarTheme.Mars;
    });
}

app.UseHttpsRedirection();

app.Run();