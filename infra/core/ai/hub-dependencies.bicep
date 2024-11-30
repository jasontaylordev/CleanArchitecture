param location string = resourceGroup().location
param tags object = {}

@description('Name of the key vault')
param keyVaultName string
@description('Name of the storage account')
param storageAccountName string
@description('Name of the OpenAI cognitive services')
param openAiName string
@description('Array of OpenAI model deployments')
param openAiModelDeployments array = []
@description('Name of the Log Analytics workspace')
param logAnalyticsName string = ''
@description('Name of the Application Insights instance')
param applicationInsightsName string = ''
@description('Name of the container registry')
param containerRegistryName string = ''
@description('Name of the Azure Cognitive Search service')
param searchServiceName string = ''

module keyVault '../security/keyvault.bicep' = {
  name: 'keyvault'
  params: {
    location: location
    tags: tags
    name: keyVaultName
  }
}

module storageAccount '../storage/storage-account.bicep' = {
  name: 'storageAccount'
  params: {
    location: location
    tags: tags
    name: storageAccountName
    containers: [
      {
        name: 'default'
      }
    ]
    files: [
      {
        name: 'default'
      }
    ]
    queues: [
      {
        name: 'default'
      }
    ]
    tables: [
      {
        name: 'default'
      }
    ]
    corsRules: [
      {
        allowedOrigins: [
          'https://mlworkspace.azure.ai'
          'https://ml.azure.com'
          'https://*.ml.azure.com'
          'https://ai.azure.com'
          'https://*.ai.azure.com'
          'https://mlworkspacecanary.azure.ai'
          'https://mlworkspace.azureml-test.net'
        ]
        allowedMethods: [
          'GET'
          'HEAD'
          'POST'
          'PUT'
          'DELETE'
          'OPTIONS'
          'PATCH'
        ]
        maxAgeInSeconds: 1800
        exposedHeaders: [
          '*'
        ]
        allowedHeaders: [
          '*'
        ]
      }
    ]
    deleteRetentionPolicy: {
      allowPermanentDelete: false
      enabled: false
    }
    shareDeleteRetentionPolicy: {
      enabled: true
      days: 7
    }
  }
}

module logAnalytics '../monitor/loganalytics.bicep' =
  if (!empty(logAnalyticsName)) {
    name: 'logAnalytics'
    params: {
      location: location
      tags: tags
      name: logAnalyticsName
    }
  }

module applicationInsights '../monitor/applicationinsights.bicep' =
  if (!empty(applicationInsightsName) && !empty(logAnalyticsName)) {
    name: 'applicationInsights'
    params: {
      location: location
      tags: tags
      name: applicationInsightsName
      logAnalyticsWorkspaceId: !empty(logAnalyticsName) ? logAnalytics.outputs.id : ''
    }
  }

module containerRegistry '../host/container-registry.bicep' =
  if (!empty(containerRegistryName)) {
    name: 'containerRegistry'
    params: {
      location: location
      tags: tags
      name: containerRegistryName
    }
  }

module cognitiveServices '../ai/cognitiveservices.bicep' = {
  name: 'cognitiveServices'
  params: {
    location: location
    tags: tags
    name: openAiName
    kind: 'AIServices'
    deployments: openAiModelDeployments
  }
}

module searchService '../search/search-services.bicep' =
  if (!empty(searchServiceName)) {
    name: 'searchService'
    params: {
      location: location
      tags: tags
      name: searchServiceName
    }
  }

output keyVaultId string = keyVault.outputs.id
output keyVaultName string = keyVault.outputs.name
output keyVaultEndpoint string = keyVault.outputs.endpoint

output storageAccountId string = storageAccount.outputs.id
output storageAccountName string = storageAccount.outputs.name

output containerRegistryId string = !empty(containerRegistryName) ? containerRegistry.outputs.id : ''
output containerRegistryName string = !empty(containerRegistryName) ? containerRegistry.outputs.name : ''
output containerRegistryEndpoint string = !empty(containerRegistryName) ? containerRegistry.outputs.loginServer : ''

output applicationInsightsId string = !empty(applicationInsightsName) ? applicationInsights.outputs.id : ''
output applicationInsightsName string = !empty(applicationInsightsName) ? applicationInsights.outputs.name : ''
output logAnalyticsWorkspaceId string = !empty(logAnalyticsName) ? logAnalytics.outputs.id : ''
output logAnalyticsWorkspaceName string = !empty(logAnalyticsName) ? logAnalytics.outputs.name : ''

output openAiId string = cognitiveServices.outputs.id
output openAiName string = cognitiveServices.outputs.name
output openAiEndpoint string = cognitiveServices.outputs.endpoints['OpenAI Language Model Instance API']

output searchServiceId string = !empty(searchServiceName) ? searchService.outputs.id : ''
output searchServiceName string = !empty(searchServiceName) ? searchService.outputs.name : ''
output searchServiceEndpoint string = !empty(searchServiceName) ? searchService.outputs.endpoint : ''
