@description('Name of Azure App Configuration store')
param configStoreName string

@description('The principal ID of the service principal to assign the role to')
param principalId string

resource configStore 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: configStoreName
}

var configStoreDataReaderRole = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '516239f1-63e1-4d78-a4de-a74fb236a071')

resource configStoreDataReaderRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(subscription().id, resourceGroup().id, principalId, configStoreDataReaderRole)
  scope: configStore
  properties: {
    roleDefinitionId: configStoreDataReaderRole
    principalId: principalId
    principalType: 'ServicePrincipal' 
  }
}
