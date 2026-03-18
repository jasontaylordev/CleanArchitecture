namespace CleanArchitecture.Web.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class WeatherStepDefinitions(WeatherPage weatherPage)
{
    [BeforeFeature("Weather")]
    public static async Task BeforeWeatherFeature(IObjectContainer container)
    {
        var context = await PlaywrightSetup.Browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var loginPage = new LoginPage(page);
        await loginPage.GotoAsync();
        await loginPage.SetEmail("administrator@localhost");
        await loginPage.SetPassword("Administrator1!");
        await loginPage.ClickLogin();
        await Assertions.Expect(page.Locator("a:has-text('Log out')")).ToBeVisibleAsync();

        container.RegisterInstanceAs(context);
        container.RegisterInstanceAs(new WeatherPage(page));
    }

    [AfterFeature]
    public static async Task AfterWeatherFeature(IObjectContainer container)
    {
        var context = container.Resolve<IBrowserContext>();
        await context.DisposeAsync();
    }

    [Given("an authenticated user visits the weather page")]
    public Task GivenAnAuthenticatedUserVisitsTheWeatherPage() => weatherPage.GotoAsync();

    [Then("the weather forecast heading is {string}")]
    public Task ThenTheWeatherForecastHeadingIs(string text) => weatherPage.AssertHeading(text);

    [Then("the weather forecast table is displayed")]
    public Task ThenTheWeatherForecastTableIsDisplayed() => weatherPage.AssertTableVisible();

    [Then("{int} weather forecasts are shown")]
    public Task ThenWeatherForecastsAreShown(int count) => weatherPage.AssertRowCount(count);
}
