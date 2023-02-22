//copy db2 license
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;
using Serilog.Sinks.SystemConsole.Themes;

try
{
    var licensePath = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location)?.FullName + "/clidriver/license/db2consv_ee.lic";
    if (!File.Exists(licensePath))
    {
        File.Copy("/db2/license/db2consv_ee.lic", licensePath);
    }

    var builder = WebApplication.CreateBuilder(args);

    // setup AWS CloudWatch client
    // var client = myAppConfigRoot.GetAwsOptions().CreateServiceClient<IAmazonCloudWatchLogs>();
    // Setup logging
    var logLevel = builder.Configuration.GetValue<LogEventLevel>("LogLevel");
    var logger = new LoggerConfiguration()
                    // .ReadFrom.Configuration(builder.Configuration)
                    .WriteTo.Console(
                        theme: AnsiConsoleTheme.Code,
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {RequestId} {Message:lj}{NewLine}{Exception}",
                        restrictedToMinimumLevel: logLevel
                    )
                    .WriteTo.AmazonCloudWatch(
                        logGroup: builder.Configuration.GetValue<string>("LogGroup"),
                        logStreamPrefix: builder.Configuration.GetValue<string>("LogStreamPrefix"),
                        restrictedToMinimumLevel: logLevel,
                        batchSizeLimit: 100,
                        batchUploadPeriodInSeconds: 15,
                        logGroupRetentionPolicy: LogGroupRetentionPolicy.OneWeek,
                        appendUniqueInstanceGuid: false
                    )
                    .Enrich.FromLogContext()
                    .CreateLogger();
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger);
    builder.Host.UseSerilog(logger);

    builder.Services.AddHttpLogging(options =>
    {
        options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                                HttpLoggingFields.RequestBody |
                                HttpLoggingFields.ResponsePropertiesAndHeaders |
                                HttpLoggingFields.ResponseBody;
    });

    // Add services to the container.
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddWebUIServices();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();

        // Initialise and seed database
        // using (var scope = app.Services.CreateScope())
        // {
        //     var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        //     await initialiser.InitialiseAsync();
        //     await initialiser.SeedAsync();
        // }
    }
    else
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        // app.UseHsts();
    }
    app.UseSerilogRequestLogging();

    app.UseHealthChecks("/health");
    // app.UseHttpsRedirection();
    // app.UseStaticFiles();

    app.UseRouting();

    // app.UseAuthentication();
    // app.UseIdentityServer();
    // app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");

    // app.MapRazorPages();

    // app.MapFallbackToFile("index.html");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, $"Unhandled exception: ${ex.InnerException?.Message}");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

// Make the implicit Program class public so test projects can access it
public partial class Program { }