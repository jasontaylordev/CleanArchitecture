using CleanArchitecture.Migrator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        MigratorModule.ConfigureServices(services, hostContext.Configuration);
    })
    .Build();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("CleanArchitecture Database Migrator starting...");

    var migrationExecutor = services.GetRequiredService<IMigrationExecutor>();
    await migrationExecutor.ExecuteAsync();

    logger.LogInformation("CleanArchitecture Database Migrator completed successfully.");
    return 0;
}
catch (Exception ex)
{
    logger.LogCritical(ex, "CleanArchitecture Database Migrator terminated unexpectedly.");
    return 1;
}
