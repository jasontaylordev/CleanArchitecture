using System.Diagnostics;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);


var web = builder.AddProject<Projects.Web>("web")
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

#pragma warning disable ASPIRECERTIFICATES001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.AddJavaScriptApp("frontend", "./../Web/ClientApp")
    .WithRunScript("start")
    .WithReference(web)
    .WaitFor(web)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();
#pragma warning restore ASPIRECERTIFICATES001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

#if (UsePostgreSQL)
var postgres = builder
    .AddPostgres("postgres")
    // Set the name of the default database to auto-create on container startup.
    .WithEnvironment("POSTGRES_DB", "CleanArchitectureDb")
    .AddDatabase("CleanArchitectureDb");

web
    .WithReference(database)
    .WaitFor(database);
#elif (UseSqlServer)
var sql = builder.AddSqlServer("sql")
    .AddDatabase("CleanArchitectureDb");

web
    .WithReference(database)
    .WaitFor(database);
#endif

builder.Build().Run();
