// #if (!UseApiOnly)
#addin nuget:?package=Cake.Npm&version=4.0.0
// #endif

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var webServerPath = "./src/Web";
var webClientPath = "./src/Web/ClientApp";
var webUrl = "https://localhost:5001/";
// #if (!UseApiOnly)
webUrl = "https://localhost:44447/";
// #endif

IProcess webProcess = null;

Task("Build")
    .Does(() => {
        Information("Building project...");
        DotNetBuild("./CleanArchitecture.sln", new DotNetBuildSettings {
            Configuration = configuration
        });
// #if (!UseApiOnly)
        if (DirectoryExists(webClientPath)) {
            Information("Installing client app dependencies...");
            NpmInstall(settings => settings.WorkingDirectory = webClientPath);
            Information("Building client app...");
            NpmRunScript("build", settings => settings.WorkingDirectory = webClientPath);
        }
// #endif
    });

Task("Run")
    .Does(() => {
        Information("Starting web project...");
        var processSettings = new ProcessSettings {
            Arguments = $"run --project {webServerPath} --configuration {configuration} --no-build --no-restore",
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        webProcess = StartAndReturnProcess("dotnet", processSettings);
        Information("Waiting for web project to be available...");
        var maxRetries = 30;
        var delay = 2000; // 2 seconds
        var retries = 0;
        var isAvailable = false;

        while (retries < maxRetries && !isAvailable) {
            try {
                using (var client = new System.Net.Http.HttpClient()) {
                    var response = client.GetAsync(webUrl).Result;
                    if (response.IsSuccessStatusCode) {
                        isAvailable = true;
                    }
                }
            } catch {
                // Ignore exceptions and retry
            }

            if (!isAvailable) {
                retries++;
                System.Threading.Thread.Sleep(delay);
            }
        }

        if (!isAvailable) {
            throw new Exception("Web project is not available after waiting.");
        }

        Information("Web project is available.");
    });

Task("Test")
    .ContinueOnError()
    .Does(() => {
        Information("Testing project...");

        var testSettings = new DotNetTestSettings {
            Configuration = configuration,
            NoBuild = true
        };

// #if (!UseApiOnly)
        if (target == "Basic") {
            testSettings.Filter = "FullyQualifiedName!~AcceptanceTests";
        }
// #endif

        DotNetTest("./CleanArchitecture.sln", testSettings);
    });

Teardown(context =>
{
        if (webProcess != null) {
            Information("Stopping web project...");
            webProcess.Kill();
            webProcess.WaitForExit();
            Information("Web project stopped.");
        }
});

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Run")
    .IsDependentOn("Test");

Task("Basic")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

RunTarget(target);
