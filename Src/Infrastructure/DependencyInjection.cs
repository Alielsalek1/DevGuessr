using Infrastructure.Persistance;
using Infrastructure.Repositories.Implementations;
using Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.Extensions.Options;
using Infrastructure.Common.Options;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDatabase();
        services.AddRepositories();
        services.AddCaching();
        services.AddMessageBroker();
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(dbOptions.ConnectionString)
                   .UseSnakeCaseNamingConvention()
                   .UseNpgsql(dbOptions.ConnectionString, npgsqlOptions =>
                   {
                       npgsqlOptions.EnableRetryOnFailure(
                           maxRetryCount: 5,
                           maxRetryDelay: TimeSpan.FromSeconds(10),
                           errorCodesToAdd: null);
                   });
        });
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserDevicesRepository, UserDevicesRepository>();
        services.AddScoped<IUserRefreshTokensRepository, UserRefreshTokensRepository>();
        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddStackExchangeRedisCache(options => { });
        services.AddOptions<RedisCacheOptions>()
            .PostConfigure<IOptions<RedisOptions>>((options, redisOptions) =>
            {
                options.Configuration = redisOptions.Value.ConnectionString;
                options.InstanceName = redisOptions.Value.InstanceName;
            });

        return services;
    }

    private static IServiceCollection AddMessageBroker(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                cfg.Host(rabbitOptions.Host, ushort.TryParse(rabbitOptions.Port, out var port) ? port : (ushort)5672, "/", h =>
                {
                    h.Username(rabbitOptions.Username);
                    h.Password(rabbitOptions.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
