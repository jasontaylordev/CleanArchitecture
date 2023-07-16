# Check if Azure CLI is installed
$azPath = (Get-Command az -ErrorAction SilentlyContinue).Source
if (-not $azPath) {
    Write-Host "Azure CLI (az) is not installed. Please install it and try again."
    return
}

# Check if GitHub CLI is installed
$ghPath = (Get-Command gh -ErrorAction SilentlyContinue).Source
if (-not $ghPath) {
    Write-Host "GitHub CLI (gh) is not installed. Please install it and try again."
    return
}

# Check if Azure CLI is authenticated
$azLoggedIn = (az account show -o json) -ne $null
if (-not $azLoggedIn) {
    Write-Host "Azure CLI (az) is not authenticated. Please authenticate with Azure CLI and try again."
    return
}

# Check if GitHub CLI is authenticated
$ghAuthStatus = (gh auth status)
if (-not ($ghAuthStatus -like "*Logged in to github.com*")) {
    Write-Host "GitHub CLI (gh) is not authenticated. Please authenticate with GitHub CLI and try again."
    return
}