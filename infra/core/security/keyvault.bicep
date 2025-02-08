metadata description = 'Creates an Azure Key Vault.'
param name string
param location string = resourceGroup().location
param tags object = {}
param logAnalyticsWorkspaceId string = ''

param principalId string = ''

@description('Allow the key vault to be used during resource creation.')
param enabledForDeployment bool = false
@description('Allow the key vault to be used for template deployment.')
param enabledForTemplateDeployment bool = false

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: name
  location: location
  tags: tags
  properties: {
    tenantId: subscription().tenantId
    sku: { family: 'A', name: 'standard' }
    accessPolicies: !empty(principalId) ? [
      {
        objectId: principalId
        permissions: { secrets: [ 'get', 'list' ] }
        tenantId: subscription().tenantId
      }
    ] : []
    enabledForDeployment: enabledForDeployment
    enabledForTemplateDeployment: enabledForTemplateDeployment
  }
}

resource keyVault_DiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = if (!(empty(logAnalyticsWorkspaceId))) {
  scope: keyVault
  name: 'keyVaultDiagnosticSettings'
  properties: {
    workspaceId: logAnalyticsWorkspaceId
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

output endpoint string = keyVault.properties.vaultUri
output id string = keyVault.id
output name string = keyVault.name
