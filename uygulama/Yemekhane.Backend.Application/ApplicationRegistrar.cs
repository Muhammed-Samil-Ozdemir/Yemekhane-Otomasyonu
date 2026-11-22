using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Yemekhane.Backend.Application.Menus;

namespace Yemekhane.Backend.Application;

public static class ApplicationRegistrar
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ApplicationRegistrar).Assembly;

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(applicationAssembly));

        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        mapsterConfig.Scan(applicationAssembly);
        services.AddSingleton(mapsterConfig);

        services.AddScoped<MenuReader>();

        return services;
    }
}
