@description('The location into which your Azure resources should be deployed.')
param location string = resourceGroup().location

@description('Select the type of environment you want to provision. Allowed values are Production, Staging, and Development.')
@allowed([
  'Production'
  'Staging'
  'Development'
])
param environmentName string

@description('A unique suffix to add to resource names that need to be globally unique.')
@maxLength(13)
param resourceNameSuffix string = uniqueString(resourceGroup().id)

@description('The administrator login username for the SQL server.')
param sqlAdministratorUsername string

@secure()
@description('The administrator login password for the SQL server.')
param sqlAdministratorPassword string

@description('The name of the project.')
param projectName string

// Define the environment configuration map.
var environmentConfigurationMap = {
  Production: {
    environmentAbbreviation: 'prd'
    appServicePlan: {
      sku: {
        name: 'S1'
        capacity: 1
      }
    }
    sqlDatabase: {
      sku: {
        name: 'Standard'
        tier: 'Standard'
      }
    }
  }
  Staging: {
    environmentAbbreviation: 'stg'
    appServicePlan: {
      sku: {
        name: 'B1'
      }
    }
    sqlDatabase: {
      sku: {
        name: 'Standard'
        tier: 'Standard'
      }
    }
  }
  Development: {
    environmentAbbreviation: 'dev'
    appServicePlan: {
      sku: {
        name: 'B1'
      }
    }
    sqlDatabase: {
      sku: {
        name: 'Standard'
        tier: 'Standard'
      }
    }
  }
}

// Define the names for resources.
var environmentAbbreviation = environmentConfigurationMap[environmentName].environmentAbbreviation
var keyVaultName = 'kv-${projectName}-${environmentAbbreviation}'
var appServiceAppName = 'as-${projectName}-${resourceNameSuffix}-${environmentAbbreviation}'
var appServicePlanName = 'plan-${projectName}-${environmentAbbreviation}'
var logAnalyticsWorkspaceName = 'log-${projectName}-${environmentAbbreviation}'
var applicationInsightsName = 'appi-${projectName}-${environmentAbbreviation}'
var sqlServerName = 'sql-${projectName}-${resourceNameSuffix}-${environmentAbbreviation}'
var sqlDatabaseName = '${projectName}-${environmentAbbreviation}'

// Define the SKUs for each component based on the environment type.
var appServicePlanSku = environmentConfigurationMap[environmentName].appServicePlan.sku
var sqlDatabaseSku = environmentConfigurationMap[environmentName].sqlDatabase.sku

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: keyVaultName
  location: location
  properties: {
    enabledForTemplateDeployment: true
    tenantId: subscription().tenantId
    accessPolicies: []
    sku: {
      name: 'standard'
      family: 'A'
    }
  }
}

resource keyVault_ConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  parent: keyVault
  name: 'ConnectionStrings--DefaultConnection'
  properties: {
    value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;User ID=${sqlAdministratorUsername};Password=${sqlAdministratorPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
  }
  dependsOn: [
    sqlDatabase
  ]
}

resource keyVault_DiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  scope: keyVault
  name: 'keyVaultDiagnosticSettings'
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    logs: [
      {
        category: 'AuditEvent'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-15' = {
  name: appServicePlanName
  location: location
  sku: appServicePlanSku
}

resource appServicePlan_DiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  scope: appServicePlan
  name: 'appServicePlanDiagnosticSettings'
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

resource appServiceApp 'Microsoft.Web/sites@2021-01-15' = {
  name: appServiceAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      healthCheckPath: '/health'
      netFrameworkVersion: 'v7.0'
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'KeyVaultUri'
          value: keyVault.properties.vaultUri
        }
      ]
    }
  }
}

resource appServiceApp_DiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  scope: appServiceApp
  name: 'appServiceAppDiagnosticSettings'
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    logs: [
      {
        category: 'AppServiceHTTPLogs'
        enabled: true
      }
      {
        category: 'AppServiceConsoleLogs'
        enabled: true
      }
      {
        category: 'AppServiceAppLogs'
        enabled: true
      }
      {
        category: 'AppServiceAuditLogs'
        enabled: true
      }
      {
        category: 'AppServiceIPSecAuditLogs'
        enabled: true
      }
      {
        category: 'AppServicePlatformLogs'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

resource keyVault_AccessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2022-07-01' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      {
        tenantId: appServiceApp.identity.tenantId
        objectId: appServiceApp.identity.principalId
        permissions: {
          keys: [
            'Get'
          ]
          secrets: [
            'Get'
            'List'

          ]
          certificates: [
            'Get'
            'List'
          ]
        }
      }
    ]
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

resource sqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorUsername
    administratorLoginPassword: sqlAdministratorPassword
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-02-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  sku: sqlDatabaseSku
}

resource sqlServer_AuditingSettings 'Microsoft.Sql/servers/auditingSettings@2021-11-01-preview' = {
  parent: sqlServer
  name: 'default'
  properties: {
    state: 'Enabled'
    isAzureMonitorTargetEnabled: true
  }
}

resource sqlServer_FirewallRule 'Microsoft.Sql/servers/firewallRules@2021-02-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

resource sqlDatabase_DiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  scope: sqlDatabase
  name: 'sqlDatabaseDiagnosticSettings'
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    logs: [
      {
        category: 'SQLInsights'
        enabled: true
      }
      {
        category: 'AutomaticTuning'
        enabled: true
      }
      {
        category: 'QueryStoreRuntimeStatistics'
        enabled: true
      }
      {
        category: 'QueryStoreWaitStatistics'
        enabled: true
      }
      {
        category: 'Errors'
        enabled: true
      }
      {
        category: 'DatabaseWaitStatistics'
        enabled: true
      }
      {
        category: 'Timeouts'
        enabled: true
      }
      {
        category: 'Blocks'
        enabled: true
      }
      {
        category: 'Deadlocks'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'Basic'
        enabled: true
      }
      {
        category: 'InstanceAndAppAdvanced'
        enabled: true
      }
      {
        category: 'WorkloadManagement'
        enabled: true
      }
    ]
  }
}

output appServiceAppName string = appServiceApp.name
output appServiceAppHostName string = appServiceApp.properties.defaultHostName
output sqlServerFullyQualifiedDomainName string = sqlServer.properties.fullyQualifiedDomainName
output sqlDatabaseName string = sqlDatabase.name
