using Npgsql;
using Respawn;
using System.Data;
using System.Data.Common;

namespace Tests.Common.TestContainerDependencies;

public class RespawnerProvider : IAsyncDisposable
{
    private readonly string _connectionString;
    private Respawner? _respawner;
    private DbConnection? _dbConnection;
    private readonly SemaphoreSlim _resetLock = new(1, 1);

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
        if (_respawner == null) throw new InvalidOperationException("Respawner not initialized.");

        await _resetLock.WaitAsync();
        try
        {
            await EnsureConnectionIsOpenAsync();
            await _respawner.ResetAsync(_dbConnection!);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Connection is not open", StringComparison.OrdinalIgnoreCase))
        {
            // Self-heal closed connection edge cases and retry reset once.
            await RecreateOpenConnectionAsync();
            await _respawner.ResetAsync(_dbConnection!);
        }
        finally
        {
            _resetLock.Release();
        }
    }

    private async Task EnsureConnectionIsOpenAsync()
    {
        if (_dbConnection == null)
        {
            await RecreateOpenConnectionAsync();
            return;
        }

        if (_dbConnection.State == ConnectionState.Open)
        {
            return;
        }

        if (_dbConnection.State == ConnectionState.Connecting)
        {
            return;
        }

        await RecreateOpenConnectionAsync();
    }

    private async Task RecreateOpenConnectionAsync()
    {
        if (_dbConnection != null)
        {
            await _dbConnection.DisposeAsync();
        }

        _dbConnection = new NpgsqlConnection(_connectionString);
        await _dbConnection.OpenAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbConnection != null) await _dbConnection.DisposeAsync();
        _resetLock.Dispose();
    }
}
