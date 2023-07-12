namespace CleanArchitecture.Web.AcceptanceTests.Pages;

public class LoginPage : BasePage
{
    public LoginPage(IBrowser browser, IPage page)
    {
        Browser = browser;
        Page = page;
    }

    public override string PagePath => $"{BaseUrl}/Identity/Account/Login";

    public override IBrowser Browser { get; }

    public override IPage Page { get; set; }

    public Task SetEmail(string email)
        => Page.FillAsync("#Input_Email", email);

    public Task SetPassword(string password)
        => Page.FillAsync("#Input_Password", password);

    public Task ClickLogin()
        => Page.Locator("#login-submit").ClickAsync();

    public Task<string?> ProfileLinkText()
        => Page.Locator("a[href='/Identity/Account/Manage']").TextContentAsync();

    public Task<bool> InvalidLoginAttemptMessageVisible()
        => Page.Locator("text=Invalid login attempt.").IsVisibleAsync();
}
