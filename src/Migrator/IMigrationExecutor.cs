namespace CleanArchitecture.Migrator;

public interface IMigrationExecutor
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
