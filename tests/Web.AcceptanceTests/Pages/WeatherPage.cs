namespace CleanArchitecture.Web.AcceptanceTests.Pages;

public class WeatherPage(IPage page) : BasePage(page)
{
    public override string PagePath => $"{BaseUrl}/weather";

    public Task AssertHeading(string text)
        => Assertions.Expect(Page.Locator("h1")).ToHaveTextAsync(text);

    public Task AssertTableVisible()
        => Assertions.Expect(Page.Locator("table")).ToBeVisibleAsync();

    public Task AssertRowCount(int count)
        => Assertions.Expect(Page.Locator("tbody tr")).ToHaveCountAsync(count);
}
