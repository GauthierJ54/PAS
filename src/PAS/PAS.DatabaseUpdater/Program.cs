using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PAS.Asset.Infrastructure.Persistence;
using PAS.Calculation.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("pas") ?? throw new InvalidOperationException("La chaîne de connexion 'pas' est introuvable.");

builder.Services.AddDbContext<AssetDbContext>(options => {
    options.UseSqlServer(
        connectionString,
        sqlOptions => {
            sqlOptions.MigrationsAssembly(
                typeof(AssetDbContext)
                    .Assembly
                    .GetName()
                    .Name);

            sqlOptions.MigrationsHistoryTable(
                "__EFMigrationsHistory",
                "asset");
        });
});

builder.Services.AddDbContext<CalculationDbContext>(options => {
    options.UseSqlServer(
        connectionString,
        sqlOptions => {
            sqlOptions.MigrationsAssembly(
                typeof(CalculationDbContext)
                    .Assembly
                    .GetName()
                    .Name);

            sqlOptions.MigrationsHistoryTable(
                "__EFMigrationsHistory",
                "calculation");
        });
});

using var host = builder.Build();

await using var scope = host.Services.CreateAsyncScope();

var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseUpdater");

try {
    logger.LogInformation("Application des migrations Asset...");

    var assetDbContext = scope.ServiceProvider.GetRequiredService<AssetDbContext>();

    await assetDbContext.Database.MigrateAsync();

    logger.LogInformation("Migrations Asset appliquées.");

    logger.LogInformation("Application des migrations Calculation...");

    var calculationDbContext = scope.ServiceProvider.GetRequiredService<CalculationDbContext>();

    await calculationDbContext.Database.MigrateAsync();

    logger.LogInformation("Migrations Calculation appliquées.");

    logger.LogInformation("Toutes les migrations ont été appliquées.");
} catch (Exception exception) {
    logger.LogCritical(exception, "Échec de la mise à jour de la base de données.");

    throw;
}