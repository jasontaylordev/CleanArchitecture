param resourceNames object
param appServicePrincipalId string
param remoteAccessEntraGroupSID string
param location string
param tags object

resource keyVault 'Microsoft.KeyVault/vaults@2024-11-01' = {
  name: resourceNames.keyVault
  location: location
  properties: {
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    tenantId: subscription().tenantId
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: remoteAccessEntraGroupSID
        permissions: {
          keys: [
            'all'
          ]
          certificates: [
            'all'
          ]
          secrets: [
            'all'
          ]
          storage: [
            'all'
          ]
        }
      }
      {
        tenantId: subscription().tenantId
        objectId: appServicePrincipalId
        permissions: {
          keys: [
            'list'
            'get'
          ]
          certificates: [
            'list'
            'get'
          ]
          secrets: [
            'list'
            'get'
          ]
          storage: [
            'list'
            'get'
          ]
        }
      }
      {
        tenantId: subscription().tenantId
        objectId: deployer().objectId
        permissions: {
          keys: [
            'all'
          ]
          certificates: [
            'all'
          ]
          secrets: [
            'all'
          ]
          storage: [
            'all'
          ]
        }
      }
    ]
    sku: {
      name: 'standard'
      family: 'A'
    }
  }
  tags: tags
}

output resourceName string = keyVault.name
output uri string = keyVault.properties.vaultUri
