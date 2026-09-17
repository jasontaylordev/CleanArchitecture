namespace CleanArchitecture.Web.AcceptanceTests.Pages;

public class HomePage(IPage page) : BasePage(page)
{
    public override string PagePath => BaseUrl;

    public Task AssertHeading(string text)
        => Assertions.Expect(Page.Locator("h1")).ToHaveTextAsync(text);
}
