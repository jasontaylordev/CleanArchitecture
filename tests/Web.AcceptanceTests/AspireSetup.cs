using Aspire.Hosting;

namespace CleanArchitecture.Web.AcceptanceTests;

[SetUpFixture]
public class AspireSetup
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    public static IDistributedApplicationTestingBuilder Builder { get; private set; } = null!;
    public static DistributedApplication App { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        var cts = new CancellationTokenSource(DefaultTimeout);
        var cancellationToken = cts.Token;

        Builder = await DistributedApplicationTestingBuilder
             .CreateAsync<Projects.AppHost>(
                args: ["--environment=Testing"],
                configureBuilder: (options, _) =>
                {
                    options.DisableDashboard = false; // Enable the dashboard for testing purposes
                });

        Builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

        Builder.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            logging.AddFilter(Builder.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });

        Builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        App = await Builder
            .BuildAsync(cancellationToken)
            .WaitAsync(cancellationToken);

        await App
            .StartAsync(cancellationToken)
            .WaitAsync(cancellationToken);

        await Task.WhenAll(
            App.ResourceNotifications.WaitForResourceHealthyAsync("web", cancellationToken).WaitAsync(cancellationToken),
            App.ResourceNotifications.WaitForResourceHealthyAsync("frontend", cancellationToken).WaitAsync(cancellationToken));
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await App.DisposeAsync();
    }
}
