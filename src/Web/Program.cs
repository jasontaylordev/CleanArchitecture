using CleanArchitecture.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#if (UseAspire)
builder.AddServiceDefaults();
#endif
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

#if (!UseAspire)
app.UseHealthChecks("/health");
#endif
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

#if (!UseApiOnly)
app.MapRazorPages();

app.MapFallbackToFile("index.html");
#endif

app.UseExceptionHandler(options => { });

#if (UseApiOnly)
app.Map("/", () => Results.Redirect("/api"));
#endif

#if (UseAspire)
app.MapDefaultEndpoints();
#endif
app.MapEndpoints();

app.Run();

public partial class Program { }
