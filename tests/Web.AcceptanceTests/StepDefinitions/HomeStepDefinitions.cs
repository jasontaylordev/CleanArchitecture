namespace CleanArchitecture.Web.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class HomeStepDefinitions(HomePage homePage)
{
    [BeforeFeature("Home")]
    public static async Task BeforeHomeFeature(IObjectContainer container)
    {
        var context = await PlaywrightSetup.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        container.RegisterInstanceAs(context);
        container.RegisterInstanceAs(new HomePage(page));
    }

    [AfterFeature]
    public static async Task AfterHomeFeature(IObjectContainer container)
    {
        var context = container.Resolve<IBrowserContext>();
        await context.DisposeAsync();
    }

    [Given("a user visits the home page")]
    public Task GivenAUserVisitsTheHomePage() => homePage.GotoAsync();

    [Then("the heading {string} is visible")]
    public Task ThenTheHeadingIsVisible(string text) => homePage.AssertHeading(text);
}
