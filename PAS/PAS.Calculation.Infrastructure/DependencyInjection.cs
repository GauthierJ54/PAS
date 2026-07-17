using Microsoft.Extensions.DependencyInjection;
using PAS.Calculation.Domain.FundPerformances;
using PAS.Calculation.Infrastructure.Messaging.Consumers;
using PAS.Calculation.Infrastructure.Persistence;

namespace PAS.Calculation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFundPerformanceRepository, InMemoryFundPerformanceRepository>();

        services.AddHostedService<FundNavAddedConsumer>();

        return services;
    }
}
