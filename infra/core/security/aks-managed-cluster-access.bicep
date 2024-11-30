metadata description = 'Assigns RBAC role to the specified AKS cluster and principal.'

@description('The AKS cluster name used as the target of the role assignments.')
param clusterName string

@description('The principal ID to assign the role to.')
param principalId string

@description('The principal type to assign the role to.')
@allowed(['Device','ForeignGroup','Group','ServicePrincipal','User'])
param principalType string = 'User'

var aksClusterAdminRole = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b1ff04bb-8a4e-4dc4-8eb5-8693973ce19b')

resource aksRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: aksCluster // Use when specifying a scope that is different than the deployment scope
  name: guid(subscription().id, resourceGroup().id, principalId, aksClusterAdminRole)
  properties: {
    roleDefinitionId: aksClusterAdminRole
    principalType: principalType
    principalId: principalId
  }
}

resource aksCluster 'Microsoft.ContainerService/managedClusters@2023-10-02-preview' existing = {
  name: clusterName
}
