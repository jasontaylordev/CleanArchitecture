var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Web>("web");

builder.Build().Run();
