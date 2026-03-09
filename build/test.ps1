param (
    [string[]]$ClientFramework = @("angular", "react", "none"),
    [string[]]$Database = @("sqlite", "sqlserver", "postgresql")
)

$outputPath = Join-Path (Split-Path $PSScriptRoot -Parent) "artifacts\template-tests"
$results = @()

function CreateAndTestProject {
    param (
        [string]$clientFramework,
        [string]$database
    )

    $name = "$clientFramework-$database"
    $projectPath = Join-Path $outputPath $name

    try {
        if (Test-Path $projectPath) {
            Write-Host "Removing existing directory: $name"
            Remove-Item -Recurse -Force $projectPath
        }

        Write-Host "Creating project: $name"
        $startTime = Get-Date

        dotnet new ca-sln --client-framework $clientFramework --database $database --name CleanArchitecture --output $projectPath --no-update-check
        if ($LASTEXITCODE -ne 0) { throw "dotnet new ca-sln failed for $name" }

        $exitCode = 0
        Push-Location $projectPath
        try {
            Write-Host "Building: $name"
            dotnet build --configuration Release
            if ($LASTEXITCODE -ne 0) { throw "Build failed for $name" }

            if ($clientFramework -ne "none") {
                Write-Host "Building client app: $name"
                Push-Location "./src/Web/ClientApp"
                try {
                    npm ci
                    if ($LASTEXITCODE -ne 0) { throw "npm ci failed for $name" }
                    npm run build
                    if ($LASTEXITCODE -ne 0) { throw "npm build failed for $name" }
                } finally {
                    Pop-Location
                }
            }

            if ($clientFramework -ne "none") {
                Write-Host "Installing Playwright browsers: $name"
                pwsh artifacts/bin/Web.AcceptanceTests/release/playwright.ps1 install --with-deps chromium
                if ($LASTEXITCODE -ne 0) { throw "Playwright install failed for $name" }
            }

            Write-Host "Testing: $name"
            dotnet test --no-build --configuration Release
            if ($LASTEXITCODE -ne 0) { $exitCode = $LASTEXITCODE }
        } finally {
            Pop-Location
        }

        $endTime = Get-Date
        $duration = $endTime - $startTime

        $script:results += [PSCustomObject]@{
            ClientFramework = $clientFramework
            Database        = $database
            ExitCode        = $exitCode
            Status          = if ($exitCode -eq 0) { "Success" } else { "Failure" }
            Duration        = $duration.ToString("c")
        }
    } catch {
        Write-Host "An error occurred while processing: $name"
        Write-Host $_.Exception.Message
        $script:results += [PSCustomObject]@{
            ClientFramework = $clientFramework
            Database        = $database
            ExitCode        = -1
            Status          = "Error"
            Duration        = "00:00:00.0000000"
        }
    }
}

if (-not (Test-Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath | Out-Null
}

foreach ($cf in $ClientFramework) {
    foreach ($db in $Database) {
        CreateAndTestProject -clientFramework $cf -database $db
    }
}

$results | Format-Table -Property ClientFramework, Database, Status, Duration -AutoSize

if ($results | Where-Object { $_.Status -ne "Success" }) {
    exit 1
}
