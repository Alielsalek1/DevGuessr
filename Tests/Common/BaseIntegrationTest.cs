using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common.TestContainerDependencies;
using TestsReusables.Auth;
using Xunit;

namespace Tests.Common;

[Collection("Integration Tests")]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        // Add default test device ID cookie to all requests
        Client.DefaultRequestHeaders.Add("Cookie", $"deviceId={AuthBackdoor.TestDeviceId}");
    }

    // Facades for providers to avoid Law of Demeter violations
    protected RedisProvider Redis => Factory.RedisProvider ?? throw new InvalidOperationException("Redis provider not available");
    protected DatabaseProvider Database => Factory.DatabaseProvider ?? throw new InvalidOperationException("Database provider not available");
    protected MailhogProvider Mailhog => Factory.MailhogProvider ?? throw new InvalidOperationException("Mailhog provider not available");
    protected IDistributedCache Cache => Factory.Services.GetRequiredService<IDistributedCache>();
    protected RespawnerProvider Respawner => Factory.RespawnerProvider ?? throw new InvalidOperationException("Respawner provider not available");
    protected RabbitMqProvider RabbitMq => Factory.RabbitMqProvider ?? throw new InvalidOperationException("RabbitMQ provider not available");

    public virtual async Task InitializeAsync()
    {
        // Clear test validator configuration
        TestGoogleAuthValidator.Clear();

        // Reset the database and flush Redis using the dedicated providers
        var tasks = new List<Task>();
        tasks.Add(Respawner.ResetAsync());
        tasks.Add(Redis.FlushAllAsync());
        await Task.WhenAll(tasks);

        var client = Mailhog.CreateClient();
        await client.DeleteAllMessagesAsync();
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;
}
