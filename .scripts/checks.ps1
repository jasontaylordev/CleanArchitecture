# Check if Azure CLI is installed
$azPath = (Get-Command az -ErrorAction SilentlyContinue).Source
if (-not $azPath) {
    Write-Host "Azure CLI (az) is not installed. Please install it and try again." -ForegroundColor Red
    exit 1
}

# Check if Azure CLI is authenticated
az account show --output none
if ($LASTEXITCODE -ne 0) {
    Write-Host "Azure CLI (az) is not authenticated. Please authenticate with Azure CLI and try again." -ForegroundColor Red
    exit 1
}

# Check if GitHub CLI is installed
$ghPath = (Get-Command gh -ErrorAction SilentlyContinue).Source
if (-not $ghPath) {
    Write-Host "GitHub CLI (gh) is not installed. Please install it and try again." -ForegroundColor Red
    exit 1
}

# Check if GitHub CLI is authenticated
gh auth status | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host "GitHub CLI (gh) is not authenticated. Please authenticate with GitHub CLI and try again." -ForegroundColor Red
    exit 1
}