using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Cache;
using Yemekhane.Backend.Domain.Balances;
using Yemekhane.Backend.Domain.Cafeterias;
using Yemekhane.Backend.Domain.Evaluations;
using Yemekhane.Backend.Domain.Foods;
using Yemekhane.Backend.Domain.Meals;
using Yemekhane.Backend.Domain.Menus;
using Yemekhane.Backend.Domain.Reservations;
using Yemekhane.Backend.Domain.Users;
using Yemekhane.Backend.Infrastructure.Auth;
using Yemekhane.Backend.Infrastructure.Balances;
using Yemekhane.Backend.Infrastructure.Cache;
using Yemekhane.Backend.Infrastructure.Cafeterias;
using Yemekhane.Backend.Infrastructure.Data;
using Yemekhane.Backend.Infrastructure.Evaluations;
using Yemekhane.Backend.Infrastructure.Foods;
using Yemekhane.Backend.Infrastructure.Meals;
using Yemekhane.Backend.Infrastructure.Menus;
using Yemekhane.Backend.Infrastructure.Reservations;
using Yemekhane.Backend.Infrastructure.Users;

namespace Yemekhane.Backend.Infrastructure;

public static class InfrastructureRegistrar
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DatabaseOption>(configuration);
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddSingleton<ReplicaSynchronizer>();

        var redisOption = configuration.Get<RedisOption>() ?? new RedisOption();
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisOption.ConnectionString));

        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<ISessionService, RedisSessionService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICafeteriaRepository, CafeteriaRepository>();
        services.AddScoped<IMealRepository, MealRepository>();
        services.AddScoped<IFoodRepository, FoodRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IBalanceRepository, BalanceRepository>();
        services.AddScoped<IEvaluationRepository, EvaluationRepository>();

        return services;
    }
}
