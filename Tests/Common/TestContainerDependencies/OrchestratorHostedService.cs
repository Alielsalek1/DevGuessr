using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Tests.Common.TestContainerDependencies;

/// <summary>
/// Hosted service that runs during application startup in the test host. It ensures
/// the database migrations are applied and initializes the respawner in the orchestrator.
/// This runs once when the test host starts.
/// </summary>
public class OrchestratorHostedService : IHostedService
{
    private readonly ContainerOrchestrator _orchestrator;
    private readonly IServiceProvider _services;
    private readonly ILogger<OrchestratorHostedService> _logger;

    public OrchestratorHostedService(ContainerOrchestrator orchestrator, IServiceProvider services, ILogger<OrchestratorHostedService> logger)
    {
        _orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _orchestrator.InitializeRespawnerAsync(_services);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize respawner in test host startup. Tests may fail.");
            // Do not let exceptions here crash the test host startup; let tests fail with a clear message.
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
