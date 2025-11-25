param resourceNames object
param appServicePrincipalId string
param remoteAccessEntraGroupSID string
param location string
param tags object

// https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles (Storage Blob Data Contributor in ↓ case)
var roleDefinitionId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')

resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: resourceNames.blobStorage
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'BlobStorage'
  properties: {
    accessTier: 'Hot'
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
  tags: tags
}

resource assignRoleManagedIdentity 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: resourceNames.blobStorageManagedId
  scope: storageAccount
  properties: {
    principalType: 'ServicePrincipal'
    roleDefinitionId: roleDefinitionId
    principalId: appServicePrincipalId
  }
}

resource assignRoleEntraGroup 'Microsoft.Authorization/roleAssignments@2022-04-01' = {    
  name: resourceNames.blobStorageEntraGroupId
  scope: storageAccount
  properties: {
    principalType: 'Group'
    roleDefinitionId: roleDefinitionId
    principalId: remoteAccessEntraGroupSID
  }
}

output url string = 'https://${storageAccount.name}.blob.${az.environment().suffixes.storage}/'
output id string = storageAccount.id
