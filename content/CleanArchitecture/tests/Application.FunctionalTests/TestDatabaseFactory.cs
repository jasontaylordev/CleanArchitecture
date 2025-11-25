namespace Cubido.Template.Application.FunctionalTests;

public static class TestDatabaseFactory
{
    public static async Task<ITestDatabase> CreateAsync()
    {
        ITestDatabase database = Environment.GetEnvironmentVariable("USE_CONTAINER") == "true"
            ? new SqlContainerTestDatabase() // for build pipeline, use docker container to test
            : new SqlServerTestDatabase(); // for local development, use local SQL Server instance

        await database.InitializeAsync();

        return database;
    }
}
