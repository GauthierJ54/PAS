using PAS.Asset.Api.Endpoints.Funds;
using PAS.Asset.Api.ExceptionHandling;
using PAS.Asset.Application;
using PAS.Asset.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Exceptions
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// OpenAPI
builder.Services.AddOpenApi();

// Application
builder.Services.AddApplication();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

builder.AddRabbitMQClient("messaging");
 
var app = builder.Build();

app.UseExceptionHandler();
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