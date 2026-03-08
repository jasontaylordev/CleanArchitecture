using System.Diagnostics;
using Projects;

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
    // Set the name of the default database to auto-create on container startup.
    .WithEnvironment("POSTGRES_DB", "CleanArchitectureDb")
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
#endif

builder.Build().Run();
