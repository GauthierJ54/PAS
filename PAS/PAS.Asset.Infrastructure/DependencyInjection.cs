using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAS.Asset.Application.Abstractions;
using PAS.Asset.Domain.Funds;
using PAS.Asset.Infrastructure.Persistence;
using PAS.Asset.Infrastructure.Persistence.Repositories;

namespace PAS.Asset.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {

        // Enregistrement du DbContext
        services.AddDbContext<AssetDbContext>(options => {
            options.UseSqlServer(
                configuration.GetConnectionString("PAS"));
        });

        // Enregistrement du Repository
        services.AddScoped<IFundRepository, FundRepository>();
        services.AddScoped<IQueryRepository, QueryRepository>();

        return services;
    }
}