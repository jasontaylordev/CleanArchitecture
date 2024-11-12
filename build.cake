var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var webPath = "./src/Web";
var webUrl = "https://localhost:44447/";
IProcess webProcess = null;

Task("Build")
    .Does(() => {
        Information("Building project...");
        DotNetBuild("./CleanArchitecture.sln", new DotNetBuildSettings {
            Configuration = configuration
        });
    });

Task("StartWeb")
    .IsDependentOn("Build")
    .Does(() => {
        Information("Starting web project...");
        var processSettings = new ProcessSettings {
            Arguments = $"run --project {webPath} --configuration {configuration}",
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
    .IsDependentOn("StartWeb")
    .Does(() => {
        Information("Testing project...");
        DotNetTest("./CleanArchitecture.sln", new DotNetTestSettings {
            Configuration = configuration,
            NoBuild = true
        });
    });

Task("StopWeb")
    .IsDependentOn("Test")
    .Does(() => {
        if (webProcess != null) {
            Information("Stopping web project...");
            webProcess.Kill();
            webProcess.WaitForExit();
            Information("Web project stopped.");
        }
    });

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("StartWeb")
    .IsDependentOn("Test")
    .IsDependentOn("StopWeb");

RunTarget(target);