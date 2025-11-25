@secure()
param vaultName string
param resourceNames object

resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
  name: resourceNames.blobStorage
}

resource keyVault 'Microsoft.KeyVault/vaults@2024-11-01' existing = {
    name: vaultName
}

// BlobStorage access-key
resource blobStorageSecret 'Microsoft.KeyVault/vaults/secrets@2024-11-01' = {
  parent: keyVault
  name: 'BlobStorage--AccessKey'
  properties: {
    value: storageAccount.listKeys().keys[0].value
  }
}
