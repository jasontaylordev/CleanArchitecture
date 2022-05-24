using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace CleanArchitecture.WebUI.AcceptanceTests;

[Parallelizable(ParallelScope.Self)]
public class Tests
{
    private const string RootUrl = "https://localhost:44447/";

    async Task<IBrowser> Browser()
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            //Headless = false,
            //SlowMo = 1000
        });

        // IBrowserType
        _ = new[] { playwright.Chromium, playwright.Firefox, playwright.Webkit };

        return browser;
    }

    async Task<IPage> Page()
    {
        var browser = await Browser();
        var page = await browser.NewPageAsync();

        return page;
    }

    [Test]
    public async Task GivenValidCredentialsLoginSucceeds()
    {
        var email = "administrator@localhost";
        var password = "Administrator1!";

        var page = await Page();

        await page.GotoAsync(RootUrl);

        await page.Locator("text=Login").ClickAsync();
        await page.FillAsync("#Input_Email", email);
        await page.FillAsync("#Input_Password", password);

        await page.Locator("#login-submit").ClickAsync();

        var result = await page.Locator("a[href='/authentication/profile']").TextContentAsync();

        result.Should().Be($"Hello {email}");
    }
}
