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

// Authentication
builder.Services
    .AddAuthentication()
    .AddKeycloakJwtBearer(
        serviceName: "keycloak",
        realm: "pas",
        options => {
            options.Audience = "pas-api";
            options.MapInboundClaims = false;
            options.TokenValidationParameters.RoleClaimType = "roles";

            if (builder.Environment.IsDevelopment()) {
                options.Authority =
                    "https://localhost:8080/realms/pas";

                options.RequireHttpsMetadata = false;

                options.BackchannelHttpHandler =
                    new HttpClientHandler {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler
                                .DangerousAcceptAnyServerCertificateValidator
                    };
            }
        });

// Authorization
builder.Services.AddAuthorization(options => {
    options.AddPolicy(
        "FundsRead",
        policy => policy.RequireRole("funds.read"));
});
// Application
builder.Services.AddApplication();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

builder.AddRabbitMQClient("messaging");

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapFundPerformanceEndpoints();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();

    app.MapScalarApiReference(options => {
        options.Title = "PAS - Calculation API";
        options.Theme = ScalarTheme.Mars;
    });
}

if (!app.Environment.IsDevelopment()) {
    app.UseHttpsRedirection();
}
app.Run();
