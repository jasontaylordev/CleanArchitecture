var builder = DistributedApplication.CreateBuilder(args);

var web = builder.AddProject<Projects.Web>("web")
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

builder.AddJavaScriptApp("frontend", "./../Web/ClientApp")
    .WithRunScript("start")
    .WithReference(web)
    .WaitFor(web)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

#if (UsePostgreSQL)
var postgres = builder
    .AddPostgres("postgres")
    .AddDatabase("CleanArchitectureDb");

web
    .WithReference(postgres)
    .WaitFor(postgres);
#elif (UseSqlServer)
var sql = builder.AddSqlServer("sql")
    .AddDatabase("CleanArchitectureDb");

web
    .WithReference(sql)
    .WaitFor(sql);
#else
var sqlite = builder
    .AddSqlite("CleanArchitectureDb");

web
    .WithReference(sqlite)
    .WaitFor(sqlite);
#endif

builder.Build().Run();
