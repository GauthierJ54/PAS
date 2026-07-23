using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAS.Asset.Application.Abstractions;
using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds;
using PAS.Asset.Infrastructure.Messaging.Outbox;
using PAS.Asset.Infrastructure.Messaging.Publishers;
using PAS.Asset.Infrastructure.Persistence;
using PAS.Asset.Infrastructure.Persistence.Outbox;
using PAS.Asset.Infrastructure.Persistence.Repositories;

namespace PAS.Asset.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {

        // Enregistrement du DbContext
        services.AddDbContext<AssetDbContext>(options =>
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
                        "asset");
                });
        });

        services.AddHostedService<OutboxPublisherWorker>();

        // Enregistrement des Repository
        services.AddScoped<IFundRepository, FundRepository>();
        services.AddScoped<IQueryRepository, QueryRepository>();
        services.AddScoped<IFundNavAddedOutbox, FundNavAddedOutbox>();

        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

        return services;
    }
}