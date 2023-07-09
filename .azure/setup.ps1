Param(
    [Parameter(Mandatory)]
    [String]$GitHubOrganisationName,

    [Parameter(Mandatory)]
    [String]$GitHubRepositoryName,

    [Parameter(Mandatory)]
    [String]$AzureLocation,

    [ValidateNotNullOrEmpty()]
    [ValidateLength(4, 17)]
    [String]$ProjectName = $GitHubRepositoryName
)

$testEnvironmentName = "Test"
$productionEnvironmentName = "Production"

function CreateWorkloadIdentity {
  param (
    $environmentName
  )

  Write-Host "🧱 Creating Azure Workload Identity for $ProjectName$environmentName"

  # Create Azure AD Application Registration
  $applicationRegistrationDetails=$(az ad app create --display-name "$ProjectName$environmentName") | ConvertFrom-Json

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
    $environmentName, 
    $appId
  )

  Write-Host "🧱 Creating Azure Resource Group for $ProjectName$environmentName"

  $resourceGroupId=$(az group create --name "$ProjectName$environmentName" --location $AzureLocation --query id --output tsv)
  az ad sp create --id $appId
  az role assignment create --assignee $appId --role Contributor --scope $resourceGroupId
}

function CreateEnvironments {
  Write-Host "🧱 Creating GitHub Environments"

  $token = gh auth token
  $header = @{"Authorization" = "token $token"}
  $contentType = "application/json"

  # Test
  $uri = "https://api.github.com/repos/$GitHubOrganisationName/$GitHubRepositoryName/environments/$testEnvironmentName"
  Invoke-WebRequest -Method PUT -Header $header -ContentType $contentType -Uri $uri

  #Production
  $uri = "https://api.github.com/repos/$GitHubOrganisationName/$GitHubRepositoryName/environments/$productionEnvironmentName"
  Invoke-WebRequest -Method PUT -Header $header -ContentType $contentType -Uri $uri
}

function SetSecrets {
  param(
    $testAppId, 
    $prodAppId
  )

  Write-Host "🧱 Setting GitHub Secrets"
  
  $repo = "https://github.com/$GitHubOrganisationName/$GitHubRepositoryName"

  gh secret set "AZURE_CLIENT_ID_TEST" --repo $repo --body $testAppId
  gh secret set "AZURE_CLIENT_ID_PRODUCTION" --repo $repo --body $prodAppId
  gh secret set "AZURE_TENANT_ID" --repo $repo --body $(az account show --query tenantId --output tsv)
  gh secret set "AZURE_SUBSCRIPTION_ID" --repo $repo --body $(az account show --query id --output tsv)
  
  Write-Host "Specify the Test SQL Server Administrator Login Password:"
  gh secret set "SQL_SERVER_ADMINISTRATOR_LOGIN_PASSWORD_TEST" --repo $repo
  
  Write-Host "Specify the Production SQL Server Administrator Login Password:"
  gh secret set "SQL_SERVER_ADMINISTRATOR_LOGIN_PASSWORD_PRODUCTION" --repo $repo
}

# Azure Initialisation
$testAppId = CreateWorkloadIdentity $testEnvironmentName
CreateResourceGroup $testEnvironmentName $testAppId
$productionAppId = CreateWorkloadIdentity $productionEnvironmentName
CreateResourceGroup $productionEnvironmentName $productionAppId

# GitHub Initialisation
CreateEnvironments
SetSecrets $testAppId $productionAppId
