using System.Diagnostics;

namespace CleanArchitecture.Web.AcceptanceTests;

[SetUpFixture]
public class PlaywrightSetup
{
    private static bool IsHeadless => Debugger.IsAttached is false;
    private static IPlaywright? _playwright;

    public static IBrowser Browser { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Assertions.SetDefaultExpectTimeout(10_000);

        _playwright = await Playwright.CreateAsync();

        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = IsHeadless,
            SlowMo = IsHeadless ? 0 : 500
        });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await Browser.CloseAsync();
        _playwright?.Dispose();
    }
}
