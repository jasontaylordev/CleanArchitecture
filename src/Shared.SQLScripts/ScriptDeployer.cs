using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CleanArchitecture.Shared.SQLScripts;

public static class ScriptDeployer
{
    public static void Deploy(string connectionString, ILogger logger)
    {
        logger.LogInformation("Starting SQL scripts deployment on connection: {ConnectionString}",
            MaskConnectionString(connectionString));

        DeployEverytimeScripts(connectionString, logger);
        DeployOnetimeScripts(connectionString, logger);

        logger.LogInformation("SQL scripts deployment completed successfully.");
    }

    public static void DeployFunctions(string connectionString, ILogger logger)
    {
        DeployEverytimeFunctionScripts(connectionString, logger);
    }

    private static void DeployEverytimeScripts(string connectionString, ILogger logger)
    {
        logger.LogInformation("Deploying everytime scripts (SPs, Functions, Views)...");

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                s => s.Contains("everytime", StringComparison.OrdinalIgnoreCase)
                     && !s.Contains("02_Functions", StringComparison.OrdinalIgnoreCase))
            .LogToConsole()
            .JournalTo(new NullJournal())
            .Build();

        ExecuteUpgrade(upgrader, logger, "Everytime scripts");
    }

    private static void DeployEverytimeFunctionScripts(string connectionString, ILogger logger)
    {
        logger.LogInformation("Deploying everytime function scripts...");

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                s => s.Contains("02_Functions", StringComparison.OrdinalIgnoreCase))
            .LogToConsole()
            .JournalTo(new NullJournal())
            .Build();

        ExecuteUpgrade(upgrader, logger, "Function scripts");
    }

    private static void DeployOnetimeScripts(string connectionString, ILogger logger)
    {
        logger.LogInformation("Deploying onetime scripts (Seeding, Migrations)...");

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                s => s.Contains("onetime", StringComparison.OrdinalIgnoreCase))
            .LogToConsole()
            .Build();

        ExecuteUpgrade(upgrader, logger, "Onetime scripts");
    }

    private static void ExecuteUpgrade(UpgradeEngine upgrader, ILogger logger, string scriptCategory)
    {
        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            logger.LogError(result.Error, "Failed to deploy {ScriptCategory}.", scriptCategory);
            throw new InvalidOperationException(
                $"SQL script deployment failed for '{scriptCategory}'. See inner exception for details.",
                result.Error);
        }

        logger.LogInformation("{ScriptCategory} deployed successfully.", scriptCategory);
    }

    /// <summary>
    /// Masks the connection string for safe logging (hides password).
    /// </summary>
    private static string MaskConnectionString(string connectionString)
    {
        // Simple masking — replace password value if present
        var parts = connectionString.Split(';');
        for (var i = 0; i < parts.Length; i++)
        {
            if (parts[i].TrimStart().StartsWith("Password", StringComparison.OrdinalIgnoreCase) ||
                parts[i].TrimStart().StartsWith("Pwd", StringComparison.OrdinalIgnoreCase))
            {
                var eqIndex = parts[i].IndexOf('=');
                if (eqIndex >= 0)
                {
                    parts[i] = parts[i][..(eqIndex + 1)] + "***";
                }
            }
        }
        return string.Join(';', parts);
    }
}
