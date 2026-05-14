namespace CleanArchitecture.Web.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class LoginStepDefinitions(LoginPage loginPage)
{
    [BeforeFeature("Login")]
    public static async Task BeforeLoginFeature(IObjectContainer container)
    {
        var context = await PlaywrightSetup.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        container.RegisterInstanceAs(context);
        container.RegisterInstanceAs(new LoginPage(page));
    }

    [AfterFeature]
    public static async Task AfterLoginFeature(IObjectContainer container)
    {
        var context = container.Resolve<IBrowserContext>();
        await context.DisposeAsync();
    }

    [Given("a logged out user")]
    public Task GivenALoggedOutUser() => loginPage.GotoAsync();

    [When("the user logs in with valid credentials")]
    public async Task TheUserLogsInWithValidCredentials()
    {
        await loginPage.SetEmail("administrator@localhost");
        await loginPage.SetPassword("Administrator1!");
        await loginPage.ClickLogin();
    }

    [Then("they log in successfully")]
    public async Task TheyLogInSuccessfully()
    {
        var logoutButtonText = await loginPage.LogoutButtonText();

        logoutButtonText.ShouldNotBeNull();
        logoutButtonText.ShouldBe("Log out");
    }

    [When("the user logs in with invalid credentials")]
    public async Task TheUserLogsInWithInvalidCredentials()
    {
        await loginPage.SetEmail("hacker@localhost");
        await loginPage.SetPassword("l337hax!");
        await loginPage.ClickLogin();
    }

    [Then("an error is displayed")]
    public Task AnErrorIsDisplayed() => loginPage.AssertErrorVisible();
}
