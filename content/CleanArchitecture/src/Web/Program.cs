using Cubido.Template.Infrastructure.Data;
using Scalar.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices(builder.Configuration);
builder.AddInfrastructureServices();
builder.AddWebServices();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:Origins").Get<string>()!.Split(';', StringSplitOptions.RemoveEmptyEntries);
        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Dont run migrations during an OpenAPI generator run
    if (Assembly.GetEntryAssembly()?.GetName().Name != "GetDocument.Insider")
    {
        await app.InitialiseDatabaseAsync();
    }
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapOpenApi("/api/swagger.json");
app.MapScalarApiReference("/api/swagger", options =>
{
    options.WithOpenApiRoutePattern("/api/swagger.json");
});

app.UseCors();

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/api"));

app.MapEndpoints();

app.Run();


public partial class Program { }
