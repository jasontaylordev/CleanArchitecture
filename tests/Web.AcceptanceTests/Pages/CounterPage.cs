namespace CleanArchitecture.Web.AcceptanceTests.Pages;

public class CounterPage(IPage page) : BasePage(page)
{
    public override string PagePath => $"{BaseUrl}/counter";

    public Task AssertHeading(string text)
        => Assertions.Expect(Page.Locator("h1")).ToHaveTextAsync(text);

    public Task AssertCurrentCount(int count)
        => Assertions.Expect(Page.Locator("p[aria-live='polite'] strong")).ToHaveTextAsync(count.ToString());

    public Task ClickIncrement()
        => Page.Locator("button:has-text('Increment')").ClickAsync();
}
