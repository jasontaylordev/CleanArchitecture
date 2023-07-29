namespace CleanArchitecture.Application.FunctionalTests;

public static class TestDatabaseFactory
{
    public static async Task<ITestDatabase> CreateAsync()
    {
#if (UseSQLite)
        var database = new SqliteTestDatabase();
#else
#if DEBUG
        var database = new SqlServerTestDatabase();
    #else
        var database = new TestcontainersTestDatabase();
    #endif
#endif

        await database.InitialiseAsync();

        return database;
    }
}
