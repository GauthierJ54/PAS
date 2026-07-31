using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAS.Asset.Application.Abstractions;
using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds;
using PAS.Asset.Infrastructure.Messaging.Outbox;
using PAS.Asset.Infrastructure.Messaging.Publishers;
using PAS.Asset.Infrastructure.Persistence;
using PAS.Asset.Infrastructure.Persistence.Repositories;
using Rebus.Config;
using Rebus.Config.Outbox;
using Rebus.Serialization.Json;
using Rebus.SqlServer;

namespace PAS.Asset.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {

        // Enregistrement du DbContext
        services.AddDbContext<AssetDbContext>(options => {
            var connectionString = configuration.GetConnectionString("pas") ?? throw new InvalidOperationException("La chaîne de connexion 'pas' est introuvable.");

            options.UseSqlServer(connectionString, sqlOptions => {
                sqlOptions.MigrationsHistoryTable(
                    "__EFMigrationsHistory",
                    "asset");
            });
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RebusOutboxTransactionBehavior<,>));

        services.AddRebus((configure, serviceProvider) => {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var sqlConnectionString = configuration.GetConnectionString("pas") ?? throw new InvalidOperationException("La chaîne de connexion SQL 'pas' est introuvable.");

            var outboxTable = new TableName(schema: "asset", tableName: "RebusOutbox");

            var rabbitMqConnectionString = configuration.GetConnectionString("messaging") ?? throw new InvalidOperationException("La chaîne de connexion RabbitMQ 'messaging' est introuvable.");

            return configure.Transport(transport => transport.UseRabbitMqAsOneWayClient(rabbitMqConnectionString)).Serialization(serializer => serializer.UseSystemTextJson()).Outbox(outbox => outbox.StoreInSqlServer(sqlConnectionString, outboxTable));
        },
        isDefaultBus: true);

        // Enregistrement des Repository
        services.AddScoped<IFundRepository, FundRepository>();
        services.AddScoped<IQueryRepository, QueryRepository>();
        services.AddScoped<IFundNavAddedEventPublisher, RebusFundNavAddedEventPublisher>();
        services.AddScoped<IFundNavSoftDeleteEventPublisher, RebusFundNavSoftDeleteEventPublisher>();
        services.AddScoped<IFundSoftDeleteEventPublisher, RebusFundSoftDeleteEventPublisher>();

        return services;
    }
}