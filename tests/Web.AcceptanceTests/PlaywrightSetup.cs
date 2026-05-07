using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace CleanArchitecture.Web.AcceptanceTests;

[SetUpFixture]
public class PlaywrightSetup
{
    private static bool IsHeadless => Debugger.IsAttached is false;
    private static IPlaywright? _playwright;

    public static IBrowser Browser { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Assertions.SetDefaultExpectTimeout(10_000);

        _playwright = await Playwright.CreateAsync();
        Browser = await LaunchChromiumAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (Browser is not null)
        {
            await Browser.CloseAsync();
        }

        _playwright?.Dispose();
    }

    private static async Task<IBrowser> LaunchChromiumAsync()
    {
        try
        {
            return await _playwright!.Chromium.LaunchAsync(CreateLaunchOptions());
        }
        catch (PlaywrightException ex) when (IsMissingBrowserExecutable(ex))
        {
            InstallPlaywrightBrowsers();
            return await _playwright!.Chromium.LaunchAsync(CreateLaunchOptions());
        }
    }

    private static BrowserTypeLaunchOptions CreateLaunchOptions()
    {
        return new BrowserTypeLaunchOptions
        {
            Headless = IsHeadless,
            SlowMo = IsHeadless ? 0 : 500
        };
    }

    private static bool IsMissingBrowserExecutable(PlaywrightException exception)
    {
        return exception.Message.Contains("Executable doesn't exist", StringComparison.OrdinalIgnoreCase)
            || exception.Message.Contains("playwright.ps1 install", StringComparison.OrdinalIgnoreCase)
            || exception.Message.Contains("Please run the following command to download new browsers", StringComparison.OrdinalIgnoreCase);
    }

    private static void InstallPlaywrightBrowsers()
    {
        var scriptPath = FindPlaywrightScript();

        if (scriptPath is null)
        {
            Assert.Fail($"Playwright browser executable was not found, and the Playwright install script could not be located from '{AppContext.BaseDirectory}'. Build the Web.AcceptanceTests project and run the generated playwright.ps1 install command before running acceptance tests.");
            return;
        }

        var installResult = TryRunPowerShellInstall(scriptPath);

        if (installResult.ExitCode != 0)
        {
            Assert.Fail($"Playwright browser installation failed with exit code {installResult.ExitCode}.{Environment.NewLine}{installResult.Output}");
        }
    }

    private static string? FindPlaywrightScript()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var scriptPath = Path.Combine(directory.FullName, "playwright.ps1");

            if (File.Exists(scriptPath))
            {
                return scriptPath;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static ProcessResult TryRunPowerShellInstall(string scriptPath)
    {
        var errors = new StringBuilder();

        foreach (var shell in new[] { "pwsh", "powershell" })
        {
            try
            {
                return RunPowerShellInstall(shell, scriptPath);
            }
            catch (Win32Exception ex)
            {
                errors.AppendLine($"Unable to start '{shell}': {ex.Message}");
            }
        }

        return new ProcessResult(1, errors.ToString());
    }

    private static ProcessResult RunPowerShellInstall(string shell, string scriptPath)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = shell,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        process.StartInfo.ArgumentList.Add("-NoProfile");
        process.StartInfo.ArgumentList.Add("-ExecutionPolicy");
        process.StartInfo.ArgumentList.Add("Bypass");
        process.StartInfo.ArgumentList.Add("-File");
        process.StartInfo.ArgumentList.Add(scriptPath);
        process.StartInfo.ArgumentList.Add("install");
        process.StartInfo.ArgumentList.Add("chromium");

        process.Start();

        var standardOutput = process.StandardOutput.ReadToEnd();
        var standardError = process.StandardError.ReadToEnd();

        process.WaitForExit();

        return new ProcessResult(
            process.ExitCode,
            string.Join(Environment.NewLine, new[] { standardOutput, standardError }.Where(s => !string.IsNullOrWhiteSpace(s))));
    }

    private sealed record ProcessResult(int ExitCode, string Output);
}
