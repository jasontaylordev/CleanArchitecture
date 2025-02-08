metadata description = 'Creates an Azure App Configuration store.'

@description('The name for the Azure App Configuration store')
param name string

@description('The Azure region/location for the Azure App Configuration store')
param location string = resourceGroup().location

@description('Custom tags to apply to the Azure App Configuration store')
param tags object = {}

@description('Specifies the names of the key-value resources. The name is a combination of key and label with $ as delimiter. The label is optional.')
param keyValueNames array = []

@description('Specifies the values of the key-value resources.')
param keyValueValues array = []

@description('The principal ID to grant access to the Azure App Configuration store')
param principalId string

resource configStore 'Microsoft.AppConfiguration/configurationStores@2023-03-01' = {
  name: name
  location: location
  sku: {
    name: 'standard'
  }
  tags: tags
}

resource configStoreKeyValue 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-03-01' = [for (item, i) in keyValueNames: {
  parent: configStore
  name: item
  properties: {
    value: keyValueValues[i]
    tags: tags
  }
}]

module configStoreAccess '../security/configstore-access.bicep' = {
  name: 'app-configuration-access'
  params: {
    configStoreName: name
    principalId: principalId
  }
  dependsOn: [configStore]
}

output endpoint string = configStore.properties.endpoint
