using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAS.Calculation.Domain.FundPerformances;
using PAS.Calculation.Infrastructure.Messaging.Handlers;
using PAS.Calculation.Infrastructure.Persistence;
using PAS.Calculation.Infrastructure.Persistence.Repositories;
using PAS.Contracts.Assets;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Serialization.Json;

namespace PAS.Calculation.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {

        services.AddDbContext<CalculationDbContext>(options => {
            var connectionString = configuration.GetConnectionString("pas") ?? throw new InvalidOperationException("La chaîne de connexion 'pas' est introuvable.");

            options.UseSqlServer(connectionString, sqlOptions => {
                sqlOptions.MigrationsHistoryTable(
                    "__EFMigrationsHistory",
                    "calculation");
            });
        });

        services.AddRebus((configure, serviceProvider) => {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var rabbitMqConnectionString = configuration.GetConnectionString("messaging") ?? throw new InvalidOperationException("La chaîne de connexion RabbitMQ 'messaging' est introuvable.");

            return configure.Transport(transport => transport.UseRabbitMq(rabbitMqConnectionString, "pas.calculation.rebus.v1")).Serialization(serializer => serializer.UseSystemTextJson()).Options(options => {
                options.SetNumberOfWorkers(1);
                options.SetMaxParallelism(1);
                options.RetryStrategy(maxDeliveryAttempts: 5, errorQueueName: "pas.calculation.rebus.error.v1");
            });
        },
        onCreated: async bus => {
            await bus.Subscribe<FundNavAddedIntegrationEvent>();
            await bus.Subscribe<FundNavSoftDeleteIntegrationEvent>();
            await bus.Subscribe<FundSoftDeleteIntegrationEvent>();
        });

        services.AddRebusHandler<FundNavAddedIntegrationEventHandler>();
        services.AddRebusHandler<FundNavSoftDeleteIntegrationEventHandler>();
        services.AddRebusHandler<FundSoftDeleteIntegrationEventHandler>();

        services.AddScoped<IFundPerformanceRepository, FundPerformanceRepository>();

        return services;
    }
}
