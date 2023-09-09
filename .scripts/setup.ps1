Param(
  [String]$GitHubOrganisationName,
  [String]$GitHubRepositoryName,
  [String]$AzureLocation,
  [String]$AzureSubscriptionId,
  [String]$AzureTenantId,
  [ValidateLength(4, 17)]
  [String]$ProjectName,
  [String]$AzureSqlLogin = "SqlAdmin"
)

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$checksScript = Join-Path $scriptRoot "checks.ps1"
$environmentsFile = Join-Path $scriptRoot "environments.json"

try {
    . $checksScript
} catch {
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host "Setup script terminated due to the checks failure." -ForegroundColor Red
    exit 1
}

$MissingParameterValues = $false

if (-not $GitHubOrganisationName) {
  $ownerJson = gh repo view --json owner 2>$null | ConvertFrom-Json
  if ($ownerJson -and $ownerJson.owner -and $ownerJson.owner.login) {
    $GitHubOrganisationName = $ownerJson.owner.login
  }
  else {
    $MissingParameterValues = $true
  }
}

if (-not $GitHubRepositoryName) {
  $GitHubRepositoryName = $(gh repo view --json name -q '.name' 2> $null)
  if (-not $GitHubRepositoryName) { $MissingParameterValues = $true }
}

if (-not $AzureLocation) {
  $AzureLocation = "australiaeast"
}

if (-not $AzureSubscriptionId) {
  $AzureSubscriptionId = $(az account show --query id --output tsv 2> $null)
  if (-not $AzureSubscriptionId) { $MissingParameterValues = $true }
}

if (-not $AzureTenantId) {
  $AzureTenantId = $(az account show --query tenantId --output tsv 2> $null)
  if (-not $AzureTenantId) { $MissingParameterValues = $true }
}

if (-not $ProjectName) {
  if ($GitHubRepositoryName) {
    $ProjectName = $GitHubRepositoryName
  }

  if (-not $ProjectName) { $MissingParameterValues = $true }
}

$repoUrl = "https://github.com/$GitHubOrganisationName/$GitHubRepositoryName"

$environments = Get-Content -Raw -Path $environmentsFile | ConvertFrom-Json

$ParametersTableData = @{
  "AzureLocation"          = $AzureLocation
  "AzureSubscriptionID"    = $AzureSubscriptionId
  "AzureTenantID"          = $AzureTenantId
  "GitHubOrganisationName" = $GitHubOrganisationName
  "GitHubRepositoryName"   = $GitHubRepositoryName
  "ProjectName"            = $ProjectName
  "AzureSqlLogin"          = $AzureSqlLogin
}

Write-Host
Write-Host "This script automates the setup of environments, resources, and credentials for a project hosted on GitHub and deployed to Azure. It creates workload identities in Azure AD, sets up resource groups, and configures environment-specific variables and secrets in the GitHub repository. The script leverages the Azure CLI, GitHub CLI, and GitHub APIs to perform these tasks. It aims to streamline the process of setting up and configuring development, staging, and production environments for the project."
Write-Host
Write-Host "Parameters:" -ForegroundColor Green
$ParametersTableData | Format-Table -AutoSize

if ($MissingParameterValues) {
  Write-Host "Script execution cancelled. Missing parameter values." -ForegroundColor Red
  exit 1
}

$EnvironmentTableData = foreach ($environment in $environments.PSObject.Properties) {
  [PSCustomObject]@{
    Abbreviation = $environment.Name
    Name         = $environment.Value
  }
} 

Write-Host "Environments:" -ForegroundColor Green
$EnvironmentTableData | Select-Object Name, Abbreviation | Format-Table -AutoSize
Write-Host

Write-Host "Warning: Running this script will perform various operations in your GitHub repository and Azure subscription. Ensure that you have the necessary permissions and understand the consequences. " -ForegroundColor Red
Write-Host
Write-Host "Disclaimer: Use this script at your own risk. The author and contributors are not responsible for any loss of data or unintended consequences resulting from running this script." -ForegroundColor Yellow
Write-Host

$confirmation = Read-Host "Do you want to continue? (y/N)"

if ($confirmation -ne "y") {
  Write-Host "Script execution cancelled." -ForegroundColor Red
  return
}

Write-Host

function CreateWorkloadIdentity {
  param (
    $environmentAbbr,
    $environmentName
  )

  # Create Azure AD Application Registration
  $applicationRegistrationDetails=$(az ad app create --display-name "$ProjectName$environmentAbbr") | ConvertFrom-Json

  # Create federated credentials 
  $credential = @{
    name="$ProjectName$environmentName";
    issuer="https://token.actions.githubusercontent.com";
    subject="repo:${GitHubOrganisationName}/${GitHubRepositoryName}:environment:$environmentName";
    audiences=@("api://AzureADTokenExchange")
  } | ConvertTo-Json
  
  $credential | az ad app federated-credential create --id $applicationRegistrationDetails.id --parameters "@-" | Out-Null
  
  $credential = @{
    name="$ProjectName";
    issuer="https://token.actions.githubusercontent.com";
    subject="repo:${GitHubOrganisationName}/${GitHubRepositoryName}:ref:refs/heads/main";
    audiences=@("api://AzureADTokenExchange")
  } | ConvertTo-Json
  
  $credential | az ad app federated-credential create --id $applicationRegistrationDetails.id --parameters "@-" | Out-Null

  return $applicationRegistrationDetails.appId
}

function CreateResourceGroup {
  param (
    $environmentAbbr,
    $appId
  )

  $resourceGroupId = $(az group create --name "$ProjectName$environmentAbbr" --location $AzureLocation --query id --output tsv)
  az ad sp create --id $appId
  az role assignment create --assignee $appId --role Contributor --scope $resourceGroupId
}

function CreateEnvironment {
  param (
    $environmentName
  )
  
  $token = gh auth token
  $header = @{"Authorization" = "token $token" }
  $contentType = "application/json"

  $uri = "https://api.github.com/repos/$GitHubOrganisationName/$GitHubRepositoryName/environments/$environmentName"
  Invoke-WebRequest -Method PUT -Header $header -ContentType $contentType -Uri $uri
}

function GenerateRandomPassword {
  param (
    [int]$Length = 16
  )

  $ValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#^_-+=?<>|~".ToCharArray()
  $Password = -join ((Get-Random -Count $Length -InputObject $ValidChars) | Get-Random -Count $Length)

  return $Password
}

function SetVariables() {    
  gh variable set AZURE_TENANT_ID --body $AzureTenantId --repo $repoUrl
  gh variable set AZURE_SUBSCRIPTION_ID --body $AzureSubscriptionId --repo $repoUrl
  gh variable set PROJECT_NAME --body $ProjectName --repo $repoUrl
}

function SetEnvironmentVariablesAndSecrets {
  param(
    $environmentAbbr,
    $environmentName,
    $appId
  )
  
  gh variable set AZURE_CLIENT_ID --body "$appId" --env $environmentName --repo $repoUrl
  gh variable set AZURE_RESOURCE_GROUP_NAME --body "$ProjectName$environmentAbbr" --env $environmentName --repo $repoUrl
  gh variable set AZURE_SQL_ADMINISTRATOR_USERNAME --body "$AzureSqlLogin" --env $environmentName --repo $repoUrl
  gh secret set AZURE_SQL_ADMINISTRATOR_PASSWORD --body (GenerateRandomPassword) --env $environmentName --repo $repoUrl
}

SetVariables

foreach ($environment in $environments.PSObject.Properties) {
  $environmentAbbr = $environment.Name
  $environmentName = $environment.Value
  
  CreateEnvironment $environmentName
  $appId = CreateWorkloadIdentity $environmentAbbr $environmentName
  CreateResourceGroup $environmentAbbr $appId
  SetEnvironmentVariablesAndSecrets $environmentAbbr $environmentName $appId
}

Write-Host "✅ Done"