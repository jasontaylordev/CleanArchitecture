namespace CleanArchitecture.Web.AcceptanceTests.Pages;

public class FetchDataPage(IPage page) : BasePage(page)
{
    public override string PagePath => $"{BaseUrl}/fetch-data";

    public Task AssertHeading(string text)
        => Assertions.Expect(Page.Locator("h1#tableLabel")).ToHaveTextAsync(text);

    public Task AssertTableVisible()
        => Assertions.Expect(Page.Locator("table")).ToBeVisibleAsync();

    public Task AssertRowCount(int count)
        => Assertions.Expect(Page.Locator("tbody tr")).ToHaveCountAsync(count);
}
