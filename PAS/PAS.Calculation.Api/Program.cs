using PAS.Calculation.Api.Endpoints;
using PAS.Calculation.Application;
using PAS.Calculation.Infrastructure;
using PAS.Common.Exceptions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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
app.MapFundPerformanceEndpoints();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();

    app.MapScalarApiReference(options => {
        options.Title = "PAS - Calculation API";
        options.Theme = ScalarTheme.Mars;
    });
}

app.UseHttpsRedirection();
app.Run();