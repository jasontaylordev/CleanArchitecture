namespace CleanArchitecture.Application.FunctionalTests;

public static class TestDatabaseFactory
{
    public static async Task<ITestDatabase> CreateAsync()
    {
#if (UseSqlite)
        var database = new SqliteTestDatabase();
#else
        // Testcontainers requires Docker. To use a local SQL Server database instead,
        // switch to `SQLServerTestDatabase`.
        var database = new TestcontainersTestDatabase();
#endif

        await database.InitialiseAsync();

        return database;
    }
}
