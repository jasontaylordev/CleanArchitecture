using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cubido.Template.Application.FunctionalTests;

public class SqlServerTestDatabase : SqlTestDatabase
{
    public override async Task InitializeAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection_Test");
        Guard.Against.Null(connectionString);
        ConnectionString = connectionString;

        await base.InitializeAsync();
    }
}
