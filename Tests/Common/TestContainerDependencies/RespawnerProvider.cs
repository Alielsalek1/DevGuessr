using Npgsql;
using Respawn;
using System.Data.Common;

namespace Tests.Common.TestContainerDependencies;

public class RespawnerProvider : IAsyncDisposable
{
    private readonly string _connectionString;
    private Respawner? _respawner;
    private DbConnection? _dbConnection;

    public RespawnerProvider(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task InitializeAsync()
    {
        _dbConnection = new NpgsqlConnection(_connectionString);
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
        });
    }

    public async Task ResetAsync()
    {
        if (_respawner == null || _dbConnection == null) throw new InvalidOperationException("Respawner not initialized.");
        await _respawner.ResetAsync(_dbConnection);
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbConnection != null) await _dbConnection.DisposeAsync();
    }
}
