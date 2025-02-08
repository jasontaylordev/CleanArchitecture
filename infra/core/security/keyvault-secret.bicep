metadata description = 'Creates or updates a secret in an Azure Key Vault.'
param name string
param tags object = {}
param keyVaultName string
param contentType string = 'string'
@description('The value of the secret. Provide only derived values like blob storage access, but do not hard code any secrets in your templates')
@secure()
param secretValue string

param enabled bool = true
param exp int = 0
param nbf int = 0

resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: name
  tags: tags
  parent: keyVault
  properties: {
    attributes: {
      enabled: enabled
      exp: exp
      nbf: nbf
    }
    contentType: contentType
    value: secretValue
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}
