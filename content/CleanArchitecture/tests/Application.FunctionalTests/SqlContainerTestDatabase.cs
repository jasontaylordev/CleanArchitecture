using Testcontainers.MsSql;

namespace Cubido.Template.Application.FunctionalTests;

public class SqlContainerTestDatabase : SqlTestDatabase
{
    private MsSqlContainer? container;

    public override async Task InitializeAsync()
    {
        container = new MsSqlBuilder()
            .WithAutoRemove(true)
            // https://github.com/testcontainers/testcontainers-dotnet/issues/1264
            .WithImage("mcr.microsoft.com/mssql/server:2022-CU22-ubuntu-22.04")
            .Build();

        await container.StartAsync();
        ConnectionString = container.GetConnectionString();

        await base.InitializeAsync();
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        if (container != null)
        {
            await container.DisposeAsync();
        }
    }
}
