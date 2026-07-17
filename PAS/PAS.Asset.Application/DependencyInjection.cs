using Microsoft.Extensions.DependencyInjection;

namespace PAS.Asset.Application;

public static class DependencyInjection {
    public static IServiceCollection AddApplication(this IServiceCollection services) {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>());
        return services;
    }
}

public sealed class ApplicationAssemblyMarker;