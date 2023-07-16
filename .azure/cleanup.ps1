Param(
    [String]$ProjectName
)

. ".\checks.ps1"

$MissingParameterValues = $false

if (-not $ProjectName) {
    $ProjectName = $(gh repo view --json name -q '.name' 2> $null)
    if (-not $ProjectName) { $MissingParameterValues = $true }
}

$ScriptParameters = @{
    "ProjectName" = $ProjectName
}

Write-Host
Write-Host "This script performs cleanup operations to delete resource groups, Azure AD applications, and purge deleted key vaults associated with a specific project hosted on GitHub. It searches for resources based on the project name and performs the necessary deletion and purging actions. The script leverages the Azure CLI to interact with Azure resources. It aims to facilitate the cleanup process and remove unnecessary resources from your Azure environment."
Write-Host
Write-Host "Parameters:" -ForegroundColor Green
$ScriptParameters | Format-Table -AutoSize
Write-Host

if ($MissingParameterValues) {
    Write-Host "Script execution cancelled. Missing parameter values." -ForegroundColor Red
    return
}

Write-Host "Warning: This script will perform cleanup operations, including deleting resource groups, Azure AD applications, and purging deleted key vaults starting with the project name '$ProjectName'. Make sure you understand the consequences and have verified the project name before proceeding." -ForegroundColor Red
Write-Host
Write-Host "Disclaimer: Use this script at your own risk. The author and contributors are not responsible for any loss of data or unintended consequences resulting from running this script." -ForegroundColor Yellow
Write-Host

$confirmation = Read-Host "Do you want to continue? (y/N)"

if ($confirmation -ne "y") {
    Write-Host "Script execution cancelled." -ForegroundColor Red
    return
}

Write-Host "🔍 Searching for Resource Groups..."
$resourceGroups = az group list --query "[?starts_with(name, '$ProjectName')].name" --output tsv

foreach ($rg in $resourceGroups) {
    Write-Host "🔥 Deleting: $rg"
    az group delete --name $rg --yes > $null 2>&1
}

Write-Host "🔍 Searching for Azure AD Applications..."
$appRegistrations = az ad app list --display-name $ProjectName --query "[].{Name:displayName, AppId:appId}" --output json | ConvertFrom-Json

foreach ($appRegistration in $appRegistrations) {
    $appName = $appRegistration.Name
    $appId = $appRegistration.AppId

    Write-Host "🔥 Deleting: $appName"
    az ad app delete --id $appId > $null 2>&1
}

Write-Host "🔍 Searching for Deleted Key Vaults..."
$deletedKeyVaults = az keyvault list-deleted --query "[?starts_with(name, 'kv-$ProjectName')].name" --output tsv

foreach ($vaultName in $deletedKeyVaults) {
    Write-Host "🔥 Purging: $vaultName"
    az keyvault purge --name $vaultName > $null 2>&1
}

Write-Host "✅ Done"
