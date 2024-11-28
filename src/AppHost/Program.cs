var builder = DistributedApplication.CreateBuilder(args);

// Note: To run without Docker, simply remove sql and database:
//       builder.AddProject<Projects.Web>("web");

#if (!UseSqlite)
var sql = builder.AddSqlServer("sql");

var database = sql.AddDatabase("CleanArchitectureDb");

builder.AddProject<Projects.Web>("web")
    .WaitFor(database)
    .WithReference(database);
#else
builder.AddProject<Projects.Web>("web");
#endif

builder.Build().Run();
