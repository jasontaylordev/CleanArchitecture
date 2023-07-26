# Check if Azure CLI is installed
$azPath = (Get-Command az -ErrorAction SilentlyContinue).Source
if (-not $azPath) {
    throw "Azure CLI (az) is not installed. Please install it and try again."
}

# Check if Azure CLI is authenticated
az account show --output none
if ($LASTEXITCODE -ne 0) {
    throw "Azure CLI (az) is not authenticated. Please authenticate with Azure CLI and try again."
}

# Check if GitHub CLI is installed
$ghPath = (Get-Command gh -ErrorAction SilentlyContinue).Source
if (-not $ghPath) {
    throw "GitHub CLI (gh) is not installed. Please install it and try again."
}

# Check if GitHub CLI is authenticated
gh auth status | Out-Null
if ($LASTEXITCODE -ne 0) {
    throw "GitHub CLI (gh) is not authenticated. Please authenticate with GitHub CLI and try again."
}

# Check if Git repo is initialised
git status | Out-Null
if ($LASTEXITCODE -ne 0) {
    throw "The Git repository has not been initialised. Please create a new GitHub repository and try again."
}