#if (UsePostgreSQL)
using Npgsql;
#elif (UseSqlServer)
using Microsoft.Data.SqlClient;
#else
using Microsoft.Data.Sqlite;
#endif
using Respawn;
using System.Data.Common;

namespace CleanArchitecture.Application.FunctionalTests;

internal sealed class RespawnHelper : IAsyncDisposable
{
    private readonly DbConnection _connection;
    private readonly Respawner _respawner;

    private RespawnHelper(DbConnection connection, Respawner respawner)
    {
        _connection = connection;
        _respawner = respawner;
    }

    public static async Task<RespawnHelper> CreateAsync(string connectionString)
    {
#if (UsePostgreSQL)
        var connection = new NpgsqlConnection(connectionString);
        var options = new RespawnerOptions { DbAdapter = DbAdapter.Postgres };
#elif (UseSqlServer)
        var connection = new SqlConnection(connectionString);
        var options = new RespawnerOptions { DbAdapter = DbAdapter.SqlServer };
#else
        var connection = new SqliteConnection(connectionString);
        var options = new RespawnerOptions { DbAdapter = DbAdapter.Sqlite };
#endif

        await connection.OpenAsync();
        var respawner = await Respawner.CreateAsync(connection, options);
        await connection.CloseAsync();
        return new RespawnHelper(connection, respawner);
    }

    public async Task ResetAsync()
    {
        await _connection.OpenAsync();
        await _respawner.ResetAsync(_connection);
        await _connection.CloseAsync();
    }

    public async ValueTask DisposeAsync() => await _connection.DisposeAsync();
}
