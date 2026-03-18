using CleanArchitecture.Shared;

var builder = DistributedApplication.CreateBuilder(args);

#if (UsePostgreSQL)
var databaseServer = builder
    .AddPostgres(Services.DatabaseServer)
    .AddDatabase(Services.Database);
#elif (UseSqlServer)
var databaseServer = builder.AddSqlServer(Services.DatabaseServer)
    .AddDatabase(Services.Database);
#else
var databaseServer = builder
    .AddSqlite(Services.Database);
#endif

var web = builder.AddProject<Projects.Web>(Services.WebApi)
    .WithReference(databaseServer)
    .WaitFor(databaseServer)
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

#if (!UseApiOnly)
builder.AddJavaScriptApp(Services.WebFrontend, "./../Web/ClientApp")
    .WithRunScript("start")
    .WithReference(web)
    .WaitFor(web)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();
#endif

builder.Build().Run();
