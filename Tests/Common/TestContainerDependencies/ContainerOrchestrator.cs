using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Testcontainers.RabbitMq;

namespace Tests.Common.TestContainerDependencies;

public class ContainerOrchestrator : IAsyncDisposable
{
    private PostgreSqlContainer? _dbContainer;
    private IContainer? _mailhogContainer;
    private RedisContainer? _redisContainer;
    private RabbitMqContainer? _rabbitMqContainer;

    private RedisProvider? _redisProvider;
    private MailhogProvider? _mailhogProvider;
    private DatabaseProvider? _databaseProvider;
    private RespawnerProvider? _respawnerProvider;
    private RabbitMqProvider? _rabbitMqProvider;

    public RedisProvider? RedisProvider => _redisProvider;
    public MailhogProvider? MailhogProvider => _mailhogProvider;
    public DatabaseProvider? DatabaseProvider => _databaseProvider;
    public RespawnerProvider? RespawnerProvider => _respawnerProvider;
    public RabbitMqProvider? RabbitMqProvider => _rabbitMqProvider;

    // Called after the application service provider is available. Ensures migrations are applied
    // and initializes the respawner used to reset the database between tests.
    public async Task InitializeRespawnerAsync(IServiceProvider services)
    {
        if (_databaseProvider == null) throw new InvalidOperationException("DatabaseProvider is not initialized.");

        // Drop and recreate the public schema to ensure a clean state for migrations (handles container reuse)
        var connString = _dbContainer!.GetConnectionString();
        await using (var conn = new Npgsql.NpgsqlConnection(connString))
        {
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DO $$
            BEGIN
                IF EXISTS (SELECT 1 FROM pg_namespace WHERE nspname = 'public') THEN
                    EXECUTE 'DROP SCHEMA public CASCADE';
                END IF;
                EXECUTE 'CREATE SCHEMA public';
            END$$;";
            await cmd.ExecuteNonQueryAsync();
        }

        // Ensure EF migrations have been applied using the app's service provider
        await DatabaseProvider.EnsureDatabaseMigratedAsync(services);

        // Create and initialize respawner now that tables exist
        _respawnerProvider = new RespawnerProvider(connString);
        await _respawnerProvider.InitializeAsync();
    }

    public async Task StartAsync()
    {
        // Create containers
        _dbContainer = ContainerFactory.CreatePostgreSqlContainer();
        _mailhogContainer = ContainerFactory.CreateMailhogContainer();
        _redisContainer = ContainerFactory.CreateRedisContainer();
        _rabbitMqContainer = ContainerFactory.CreateRabbitMqContainer();

        await Task.WhenAll(
            _dbContainer.StartAsync(),
            _mailhogContainer.StartAsync(),
            _redisContainer.StartAsync(),
            _rabbitMqContainer.StartAsync()
        );

        // Create providers that operate on containers/resources
        _mailhogProvider = new MailhogProvider(_mailhogContainer);
        _redisProvider = new RedisProvider(_redisContainer!);
        _databaseProvider = new DatabaseProvider(_dbContainer!.GetConnectionString());
        _respawnerProvider = new RespawnerProvider(_dbContainer.GetConnectionString());
        _rabbitMqProvider = new RabbitMqProvider(_rabbitMqContainer!);

        // Do NOT initialize respawner here — database migrations must run first
        // Respawner will be initialized later once the application services are available.

        // Configure environment variables for the application to use
        // (Keep environment wiring inside the orchestrator so factory remains focused)
        var smtpPort = _mailhogContainer.GetMappedPublicPort(1025);
        TestEnvironment.SetEmailEnvironmentVariables(smtpPort);

        TestEnvironment.SetDatabaseEnvironmentVariables(_dbContainer.GetConnectionString());

        TestEnvironment.SetRedisEnvironmentVariables(_redisContainer.GetConnectionString());

        TestEnvironment.SetRabbitMqEnvironmentVariables(_rabbitMqProvider.GetConnectionString());

        TestEnvironment.SetAspNetCoreEnvironment();

        TestEnvironment.SetSeqUrl();
    }

    public async ValueTask DisposeAsync()
    {
        var tasks = new List<Task>();
        if (_respawnerProvider != null) tasks.Add(_respawnerProvider.DisposeAsync().AsTask());
        if (_mailhogContainer != null) tasks.Add(_mailhogContainer.DisposeAsync().AsTask());
        if (_dbContainer != null) tasks.Add(_dbContainer.DisposeAsync().AsTask());
        if (_redisContainer != null) tasks.Add(_redisContainer.DisposeAsync().AsTask());
        if (_rabbitMqContainer != null) tasks.Add(_rabbitMqContainer.DisposeAsync().AsTask());
        await Task.WhenAll(tasks);
    }
}
