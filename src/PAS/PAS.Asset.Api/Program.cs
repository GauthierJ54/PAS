using PAS.Asset.Api.Endpoints.Funds;
using PAS.Asset.Application;
using PAS.Asset.Infrastructure;
using PAS.Common.Exceptions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Exceptions
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
                options.Authority = "https://localhost:8080/realms/pas";

                options.RequireHttpsMetadata = false;

                options.BackchannelHttpHandler = new HttpClientHandler {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            }
        });

// Authorization
builder.Services.AddAuthorization(options => {
    options.AddPolicy(
        "FundsRead",
        policy => policy.RequireRole("funds.read"));

    options.AddPolicy(
        "FundsWrite",
        policy => policy.RequireRole("funds.write"));

    options.AddPolicy(
        "FundsDelete",
        policy => policy.RequireRole("funds.delete"));
});

// Application
builder.Services.AddApplication();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapFundEndpoints();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();

    app.MapScalarApiReference(options => {
        options.Title = "PAS - Asset API";
        options.Theme = ScalarTheme.Mars;
    });
}

if (!app.Environment.IsDevelopment()) {
    app.UseHttpsRedirection();
}
app.Run();
