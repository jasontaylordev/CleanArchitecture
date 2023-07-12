#if (UseSQLite)
using System.Data;
using System.Data.Common;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.IntegrationTests;

public class TestDatabase
{
    private readonly string _connectionString;
    private readonly SqliteConnection _connection;

    private TestDatabase()
    {
        _connectionString = "DataSource=:memory:";
        _connection = new SqliteConnection(_connectionString);
    }

    public static async Task<TestDatabase> CreateAsync()
    {
        var database = new TestDatabase();

        await database.InitialiseAsync();

        return database;
    }

    public async Task InitialiseAsync()
    {
        if (_connection.State == ConnectionState.Open)
        {
            await _connection.CloseAsync();
        }

        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new ApplicationDbContext(options, null, null);

        context.Database.Migrate();
    }

    public DbConnection GetConnection()
    {
        return _connection;
    }

    public async Task ResetAsync()
    {
        await InitialiseAsync();
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
#endif