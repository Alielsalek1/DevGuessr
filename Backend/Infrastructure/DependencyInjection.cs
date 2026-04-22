using Infrastructure.Persistance;
using Npgsql;
using Infrastructure.Repositories.Implementations;
using Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.Extensions.Options;
using Infrastructure.Common.Options;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Infrastructure.Common.Services;
using Application.Services.Interfaces.Misc;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDatabase();
        services.AddRepositories();
        services.AddFileStorage();
        services.AddCaching();
        services.AddMessageBroker();
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddSingleton<NpgsqlDataSource>(sp => 
        {
            var dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var builder = new NpgsqlDataSourceBuilder(dbOptions.ConnectionString);
            builder.EnableDynamicJson();
            return builder.Build();
        });

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var dataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();
            options.UseNpgsql(dataSource, npgsqlOptions =>
                   {
                       npgsqlOptions.EnableRetryOnFailure(
                           maxRetryCount: 5,
                           maxRetryDelay: TimeSpan.FromSeconds(10),
                           errorCodesToAdd: null);
                   })
                   .UseSnakeCaseNamingConvention();
        });

        services.AddHostedService<DatabaseMigrationHostedService>();
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserDevicesRepository, UserDevicesRepository>();
        services.AddScoped<IUserRefreshTokensRepository, UserRefreshTokensRepository>();
        services.AddScoped<ILangdleRepository, LangdleRepository>();
        services.AddScoped<ILogodleTargetRepository, LogodleTargetRepository>();
        services.AddScoped<IClusterdleRepository, ClusterdleRepository>();
        services.AddScoped<ILangdleGameRepository, LangdleGameRepository>();
        services.AddScoped<ILogodleGameRepository, LogodleGameRepository>();
        services.AddScoped<IMythdleGameRepository, MythdleGameRepository>();
        services.AddScoped<IMythdleTargetRepository, MythdleTargetRepository>();
        return services;
    }

    private static IServiceCollection AddFileStorage(this IServiceCollection services)
    {
        services.AddScoped<IFileStorageService, FileStorageService>();
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
