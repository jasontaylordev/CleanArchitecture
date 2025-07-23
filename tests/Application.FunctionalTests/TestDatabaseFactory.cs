namespace CleanArchitecture.Application.FunctionalTests;

public static class TestDatabaseFactory
{
    public static async Task<ITestDatabase> CreateAsync()
    {
#if (UsePostgreSQL)
        // Testcontainers requires Docker. To use a local PostgreSQL database instead,
        // switch to `PostgreSQLTestDatabase` and update appsettings.json.
        var database = new PostgreSQLTestcontainersTestDatabase();
#elif (UseSqlite)
        var database = new SqliteTestDatabase();
#else
        // Testcontainers requires Docker. To use a local SQL Server database instead,
        // switch to `SqlTestDatabase` and update appsettings.json.
        var database = new SqlTestcontainersTestDatabase();
#endif

        await database.InitialiseAsync();

        return database;
    }
}
