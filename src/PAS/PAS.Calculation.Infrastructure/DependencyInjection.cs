using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAS.Calculation.Domain.FundPerformances;
using PAS.Calculation.Infrastructure.Messaging.Consumers;
using PAS.Calculation.Infrastructure.Persistence;
using PAS.Calculation.Infrastructure.Persistence.Repositories;

namespace PAS.Calculation.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {

        services.AddDbContext<CalculationDbContext>(options =>
        {
            var connectionString =
                configuration.GetConnectionString("pas")
                ?? throw new InvalidOperationException(
                    "La chaîne de connexion 'pas' est introuvable.");

            options.UseSqlServer(
                connectionString,
                sqlOptions => {
                    sqlOptions.MigrationsHistoryTable(
                        "__EFMigrationsHistory",
                        "calculation");
                });
        });

        services.AddScoped<IFundPerformanceRepository, FundPerformanceRepository>();

        services.AddHostedService<FundNavAddedConsumer>();

        return services;
    }
}
