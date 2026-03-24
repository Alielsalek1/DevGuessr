using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistance;
using Npgsql;

namespace Tests.Common.TestContainerDependencies;

public class DatabaseProvider
{
    private readonly string _connectionString;

    public DatabaseProvider(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public static async Task EnsureDatabaseMigratedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public void ReplaceDbContext(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        services.AddSingleton(dataSource);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(dataSource)
                   .UseSnakeCaseNamingConvention()
                   .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });
    }

    public static void ReplaceDbContextWithEnvironment(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
        var conn = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(conn!);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        services.AddSingleton(dataSource);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(dataSource)
                   .UseSnakeCaseNamingConvention()
                   .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });
    }
}
