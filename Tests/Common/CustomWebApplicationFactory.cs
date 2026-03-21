using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Tests.Common.TestContainerDependencies;
using Tests.MailHog;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Builder;

namespace Tests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private ContainerOrchestrator? _orchestrator;

    /// <summary>
    /// Initializes the test environment and starts the required containers.
    /// </summary>
    public async Task InitializeAsync()
    {
        // hardcode SEQ_URL for testing purposes
        TestEnvironment.SetSeqUrl();
        // hardcode JWT for testing purposes
        TestEnvironment.SetJwtEnvironmentVariables();

        // Set up test environment and start orchestrator (owns containers + providers)
        TestEnvironment.Configure();
        _orchestrator = new ContainerOrchestrator();
        await _orchestrator.StartAsync();

        // Create a minimal service provider for migrations
        var services = new ServiceCollection();
        _orchestrator.DatabaseProvider?.ReplaceDbContext(services);
        // Add any other required services for migration here if needed
        using var serviceProvider = services.BuildServiceProvider();
        // Initialize respawner after migrations
        await _orchestrator.InitializeRespawnerAsync(serviceProvider);
    }

    // Note: Database and Redis operations should be performed via the exposed providers

    // Expose providers so tests and helpers can operate on containers/resources
    public RedisProvider? RedisProvider => _orchestrator?.RedisProvider;
    public DatabaseProvider? DatabaseProvider => _orchestrator?.DatabaseProvider;
    public RespawnerProvider? RespawnerProvider => _orchestrator?.RespawnerProvider;
    public MailhogProvider? MailhogProvider => _orchestrator?.MailhogProvider;
    public RabbitMqProvider? RabbitMqProvider => _orchestrator?.RabbitMqProvider;
    public MailhogClient? MailhogClient => _orchestrator?.MailhogProvider?.CreateClient();

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync(); // Gracefully stops the Web Host and Hangfire first
        if (_orchestrator != null) await _orchestrator.DisposeAsync(); // Then tear down the Postgres container safely
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", "1");
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            // Replace IGoogleAuthValidator with test implementation
            var googleAuthValidatorDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IGoogleAuthValidator));
            if (googleAuthValidatorDescriptor != null)
            {
                services.Remove(googleAuthValidatorDescriptor);
            }
            services.AddScoped<IGoogleAuthValidator, TestGoogleAuthValidator>();

            // allow ContainerOrchestrator's DatabaseProvider to replace DbContext registrations
            if (_orchestrator?.DatabaseProvider != null)
            {
                _orchestrator.DatabaseProvider.ReplaceDbContext(services);
            }
            else
            {
                DatabaseProvider.ReplaceDbContextWithEnvironment(services);
                // Add hosted service to migrate the database for fallback case
                services.AddHostedService<FallbackMigrationHostedService>();
            }

            // Register the orchestrator instance and a hosted service that will run
            // after the app's service provider is available to run migrations and init respawner.
            if (_orchestrator != null)
            {
                services.AddSingleton(_orchestrator);
                services.AddHostedService<OrchestratorHostedService>();
            }
        });

        // Configure TestServer to inject fake RemoteIpAddress for all requests
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IStartupFilter, FakeRemoteIpAddressStartupFilter>();
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        // Add a header that the middleware will use to set the IP
        client.DefaultRequestHeaders.Add("X-Test-Remote-IP", FakeRemoteIpAddressMiddleware.DefaultTestIpAddress);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        
        // Mute Serilog globally for the test environment AFTER Program.cs configures it
        Serilog.Log.Logger = new Serilog.LoggerConfiguration()
            .MinimumLevel.Error()
            .CreateLogger();
            
        return host;
    }
}

/// <summary>
/// Startup filter that injects the FakeRemoteIpAddressMiddleware at the very beginning of the pipeline
/// </summary>
public class FakeRemoteIpAddressStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.UseMiddleware<FakeRemoteIpAddressMiddleware>();
            next(app);
        };
    }
}
