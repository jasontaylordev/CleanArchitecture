namespace CleanArchitecture.TestAppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

#if (UsePostgreSQL)
        builder.AddPostgres("postgres").AddDatabase("CleanArchitectureDb");
#elif (UseSqlServer)
        builder.AddSqlServer("sql").AddDatabase("CleanArchitectureDb");
#else
        builder.AddSqlite("CleanArchitectureDb");
#endif

        builder.Build().Run();
    }
}
