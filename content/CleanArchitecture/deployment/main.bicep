targetScope='resourceGroup'

@description('env added to each resource')
param environment string
@description('the execution env. This does not affect the azure infrastructure, but the future running services')
param executionEnvironment string
@description('where to deploy the resources (EU only)')
@allowed(['westeurope', 'northeurope', 'swedencentral', 'uksouth', 'francecentral', 'germanywestcentral'])
param location string
@description('where to deploy the sql-server resource (EU only)')
@allowed(['westeurope', 'northeurope', 'swedencentral', 'uksouth', 'francecentral', 'germanywestcentral'])
param sqlServerLocation string

@description('the EntraID Group name (just for display purposes - may be anything)')
param remoteAccessEntraGroupName string
@description('the EntraID Group identifier to grant access to the SQL Server')
@minLength(36)
@maxLength(36)
param remoteAccessEntraGroupSID string
@description('the IP address to whitelist to access the sql-server remotely. Empty if none')
param remoteAccessFrom string
@description('ppl. who receive any alerts from the app-insights')
param alertReceivers array

@description('additional tags added to each resource')
param tags object = {
  environment: environment
  project: 'cubido-template'
}

// Arbitrary but fixed random identifier for the resources
var uid = substring(replace(guid('cubido', tags.project, environment), '-', ''), 0, 24)
// uid for postfixes
var suid = substring(uid, 0, 6)

var resourceNames = {
    resourceGroup: 'rg-${tags.project}-${environment}'
    appSettings: 'appsettings-${environment}'
    appService: 'app-${environment}'
    appServicePlan: 'asp-${environment}'
    appInsightsAlerts: 'apr-${environment}'
    appInsightsActionGroup: 'ag-${environment}'
    appInsights: 'appi-${environment}'
    keyVault: 'kv-${environment}-${suid}'
    sqlServer: 'sql-${environment}-${suid}'
    sqlDatabase: 'sqldb-${environment}'
    sqlRole: guid(uid, 'sql-roles', environment)
    blobStorage: uid
    blobStorageEntraGroupId: guid(uid, 'blobStorageEntraGroupId')
    blobStorageManagedId: guid(uid, 'blobStorageManagedId')
}

// app-service
module app 'app.bicep' = {
    params: {
        environment: environment
        location: location
        resourceNames: resourceNames
        alertReceivers: alertReceivers
        tags: tags
    }
}

// key-vault
module keyVault 'keyvault.bicep' = {
    params: {
        resourceNames: resourceNames
        appServicePrincipalId: app.outputs.principalId
        remoteAccessEntraGroupSID: remoteAccessEntraGroupSID
        location: location
        tags: tags
    }
}

// storage-account
module storage 'blobstorage.bicep' = {
    params: {
        resourceNames: resourceNames
        appServicePrincipalId: app.outputs.principalId
        remoteAccessEntraGroupSID: remoteAccessEntraGroupSID
        location: location
        tags: tags
    }
}

// database
module database 'database.bicep' = {
    params: {
        location: sqlServerLocation
        resourceNames: resourceNames
        appServicePrincipalId: app.outputs.principalId
        remoteAccessFrom: remoteAccessFrom
        remoteAccessEntraGroupName: remoteAccessEntraGroupName
        remoteAccessEntraGroupSID: remoteAccessEntraGroupSID
        tags: tags
    }
}

// mail
module mail 'mail.bicep' = {
    params: {
        environment: environment
        tags: tags
    }
}

// gather environment variables for the app (non-confidential)
module appSettings 'appsettings.bicep' = {
  name: resourceNames.appSettings
  params: {
    resourceNames: resourceNames
    appSettings: {
        ConnectionStrings__DefaultConnection: database.outputs.connectionString
        BlobStorage__Url: storage.outputs.url
        ASPNETCORE_ENVIRONMENT: executionEnvironment
        APPINSIGHTS_INSTRUMENTATIONKEY: app.outputs.appInsightInstrumentationKey
        APPLICATIONINSIGHTS_CONNECTION_STRING: app.outputs.appInsightConnectionString
        AZURE_KEY_VAULT_ENDPOINT: keyVault.outputs.uri
    }
  }
}

// gather secrets for the vault
module vaultSecrets 'secrets.bicep' = {
    params: {
        vaultName: keyVault.outputs.resourceName
        resourceNames: resourceNames
    }
}
