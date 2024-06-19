using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.IntegrationTests.Data;

[TestFixture]
public abstract class BaseEfRepoTestFixture
{
    protected ApplicationDbContext dbContext;

    protected BaseEfRepoTestFixture()
    {
        var options = CreateNewContextOptions();
        dbContext = new ApplicationDbContext(options);
        dbContext.Database.OpenConnection(); // Ensure the connection is opened
        dbContext.Database.EnsureCreated(); // Ensure the database schema is created
    }

    protected static DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        // Create a fresh service provider, and therefore a fresh
        // SQLite database instance.
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkSqlite()
            .BuildServiceProvider();

        // Create a new options instance telling the context to use a
        // SQLite database and the new service provider.
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseSqlite("DataSource=:memory:")
               .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }

}
