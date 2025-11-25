using Cubido.Template.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using System.Data.Common;

namespace Cubido.Template.Application.FunctionalTests;

public abstract class SqlTestDatabase : ITestDatabase
{
    private SqlConnection connection = null!;
    private Respawner respawner = null!;

    protected string ConnectionString { get; set; } = null!;

    public virtual async Task InitializeAsync()
    {
        connection = new SqlConnection(ConnectionString);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        var context = new ApplicationDbContext(options);

        context.Database.Migrate();

        respawner = await Respawner.CreateAsync(ConnectionString, new RespawnerOptions
        {
            TablesToIgnore = ["__EFMigrationsHistory"]
        });

        await ResetAsync();
    }

    public DbConnection GetConnection()
    {
        return connection;
    }

    public async Task ResetAsync()
    {
        await respawner.ResetAsync(ConnectionString);
    }

    public virtual async Task DisposeAsync()
    {
        await connection.DisposeAsync();
    }
}
