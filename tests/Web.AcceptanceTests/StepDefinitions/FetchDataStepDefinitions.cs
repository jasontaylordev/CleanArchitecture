namespace CleanArchitecture.Web.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class FetchDataStepDefinitions(FetchDataPage fetchDataPage)
{
    [BeforeFeature("FetchData")]
    public static async Task BeforeFetchDataFeature(IObjectContainer container)
    {
        var context = await PlaywrightSetup.Browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var loginPage = new LoginPage(page);
        await loginPage.GotoAsync();
        await loginPage.SetEmail("administrator@localhost");
        await loginPage.SetPassword("Administrator1!");
        await loginPage.ClickLogin();
        await Assertions.Expect(page.Locator("button.nav-link:has-text('Log out')")).ToBeVisibleAsync();

        container.RegisterInstanceAs(context);
        container.RegisterInstanceAs(new FetchDataPage(page));
    }

    [AfterFeature]
    public static async Task AfterFetchDataFeature(IObjectContainer container)
    {
        var context = container.Resolve<IBrowserContext>();
        await context.DisposeAsync();
    }

    [Given("an authenticated user visits the fetch data page")]
    public Task GivenAnAuthenticatedUserVisitsTheFetchDataPage() => fetchDataPage.GotoAsync();

    [Then("the weather forecast heading is {string}")]
    public Task ThenTheWeatherForecastHeadingIs(string text) => fetchDataPage.AssertHeading(text);

    [Then("the weather forecast table is displayed")]
    public Task ThenTheWeatherForecastTableIsDisplayed() => fetchDataPage.AssertTableVisible();

    [Then("{int} weather forecasts are shown")]
    public Task ThenWeatherForecastsAreShown(int count) => fetchDataPage.AssertRowCount(count);
}
