using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Shared.SQLScripts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Migrator;

public class MigrationExecutor : IMigrationExecutor
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MigrationExecutor> _logger;
    private readonly ApplicationDbContext _context;

    public MigrationExecutor(
        IConfiguration configuration,
        ILogger<MigrationExecutor> logger,
        ApplicationDbContext context)
    {
        _configuration = configuration;
        _logger = logger;
        _context = context;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var connectionString = _configuration.GetConnectionString("CleanArchitectureDb")
            ?? throw new InvalidOperationException(
                "Connection string 'CleanArchitectureDb' not found in configuration.");

        _logger.LogInformation("=== Database Migration Started ===");

        await RunEfCoreMigrationsAsync(cancellationToken);
        RunSqlScripts(connectionString);

        _logger.LogInformation("=== Database Migration Completed Successfully ===");
    }

    private async Task RunEfCoreMigrationsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Applying EF Core migrations...");

        var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
        var migrations = pendingMigrations.ToList();

        if (migrations.Count == 0)
        {
            _logger.LogInformation("No pending EF Core migrations found.");
            return;
        }

        _logger.LogInformation("Found {Count} pending migration(s): {Migrations}",
            migrations.Count, string.Join(", ", migrations));

        await _context.Database.MigrateAsync(cancellationToken);

        _logger.LogInformation("EF Core migrations applied successfully.");
    }

    private void RunSqlScripts(string connectionString)
    {
        _logger.LogInformation("Deploying SQL scripts via DbUp...");
        ScriptDeployer.Deploy(connectionString, _logger);
    }
}
