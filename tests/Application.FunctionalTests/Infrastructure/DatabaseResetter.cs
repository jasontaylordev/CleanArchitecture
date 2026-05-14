#if (UsePostgreSQL)
using Npgsql;
#elif (UseSqlServer)
using Microsoft.Data.SqlClient;
#else
using Microsoft.Data.Sqlite;
#endif
using Respawn;
using System.Data.Common;

namespace CleanArchitecture.Application.FunctionalTests.Infrastructure;

internal sealed class DatabaseResetter : IAsyncDisposable
{
    private readonly DbConnection _connection;
    private readonly Respawner _respawner;

    private DatabaseResetter(DbConnection connection, Respawner respawner)
    {
        _connection = connection;
        _respawner = respawner;
    }

    public static async Task<DatabaseResetter> CreateAsync(string connectionString)
    {
#if (UsePostgreSQL)
        var connection = new NpgsqlConnection(connectionString);
#elif (UseSqlServer)
        var connection = new SqlConnection(connectionString);
#else
        var connection = new SqliteConnection(connectionString);
#endif

        await connection.OpenAsync();
        var respawner = await Respawner.CreateAsync(connection);
        await connection.CloseAsync();
        return new DatabaseResetter(connection, respawner);
    }

    public async Task ResetAsync()
    {
        await _connection.OpenAsync();
        await _respawner.ResetAsync(_connection);
        await _connection.CloseAsync();
    }

    public async ValueTask DisposeAsync() => await _connection.DisposeAsync();
}
