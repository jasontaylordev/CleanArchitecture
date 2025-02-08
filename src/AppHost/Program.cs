var builder = DistributedApplication.CreateBuilder(args);

#if (UsePostgreSQL)
var databaseName = "CleanArchitectureDb";

var postgres = builder
    .AddPostgres("postgres")
    // Set the name of the default database to auto-create on container startup.
    .WithEnvironment("POSTGRES_DB", databaseName);

var database = postgres.AddDatabase(databaseName);

builder.AddProject<Projects.Web>("web")
    .WithReference(database)
    .WaitFor(database);
#elif (UseSqlite)
builder.AddProject<Projects.Web>("web");
#else
var sql = builder.AddSqlServer("sql");

var database = sql.AddDatabase("CleanArchitectureDb");

builder.AddProject<Projects.Web>("web")
    .WithReference(database)
    .WaitFor(database);
#endif

builder.Build().Run();
