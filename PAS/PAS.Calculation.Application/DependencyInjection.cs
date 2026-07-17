using Microsoft.Extensions.DependencyInjection;

namespace PAS.Calculation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>());

        return services;
    }
}

public sealed class ApplicationAssemblyMarker;
