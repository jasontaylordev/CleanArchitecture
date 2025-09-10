metadata description = 'Adds an agent pool to an Azure Kubernetes Service (AKS) cluster.'
param clusterName string

@description('The agent pool name')
param name string

@description('The agent pool configuration')
param config object

resource aksCluster 'Microsoft.ContainerService/managedClusters@2023-10-02-preview' existing = {
  name: clusterName
}

resource nodePool 'Microsoft.ContainerService/managedClusters/agentPools@2023-10-02-preview' = {
  parent: aksCluster
  name: name
  properties: config
}
