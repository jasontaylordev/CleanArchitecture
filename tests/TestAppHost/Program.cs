using CleanArchitecture.Shared;

namespace CleanArchitecture.TestAppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        #if (UsePostgreSQL)
        builder.AddPostgres(Services.DatabaseServer)
            .AddDatabase(Services.Database);
        #elif (UseSqlServer)
        builder.AddSqlServer(Services.DatabaseServer)
            .AddDatabase(Services.Database);
        #else
        builder
            .AddSqlite(Services.Database);
        #endif

        builder.Build().Run();
    }
}