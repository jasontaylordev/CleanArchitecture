param name string
param location string = resourceGroup().location
param tags object = {}

param serviceName string = 'web'
param applicationInsightsName string = ''
param keyVaultName string = ''

module appServicePlan '../core/host/appserviceplan.bicep' = {
  name: 'appServicePlan'
  params: {
    name: name
    location: location
    tags: tags
    sku: {
      name: 'B1'
    }
  }
}

module appService '../core/host/appservice.bicep' = {
  name: 'appService'
  params: {
    name: name
    location: location
    tags: union(tags, { 'azd-service-name': serviceName })
    appServicePlanId: appServicePlan.outputs.id
    applicationInsightsName: applicationInsightsName
    keyVaultName: keyVaultName
    runtimeName: 'dotnetcore'
    runtimeVersion: '9.0'
    healthCheckPath: '/health'
    appSettings: {
      ASPNETCORE_ENVIRONMENT: 'Development'
    }
  }
}

output name string = appService.outputs.name
output uri string = appService.outputs.uri
output identityPrincipalId string = appService.outputs.identityPrincipalId
