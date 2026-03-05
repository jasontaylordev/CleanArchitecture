namespace CleanArchitecture.Web.AcceptanceTests.Pages;

public class LoginPage : BasePage
{
    public LoginPage(IBrowser browser, IPage page)
    {
        Browser = browser;
        Page = page;
    }

    public override string PagePath => $"{BaseUrl}/login";

    public override IBrowser Browser { get; }

    public override IPage Page { get; set; }

    public Task SetEmail(string email)
        => Page.FillAsync("#email", email);

    public Task SetPassword(string password)
        => Page.FillAsync("#password", password);

    public Task ClickLogin()
        => Page.Locator("button[type='submit']").ClickAsync();

    public Task<string?> LogoutButtonText()
        => Page.Locator("button.nav-link:has-text('Log out')").TextContentAsync();

    public Task<bool> InvalidLoginAttemptMessageVisible()
        => Page.Locator(".alert-danger").IsVisibleAsync();
}
