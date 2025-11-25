param environment string
param location string
param resourceNames object
param alertReceivers array
param tags object

resource insights 'Microsoft.Insights/components@2020-02-02' = {
    name: resourceNames.appInsights
    location: location
    kind: 'web'
    properties: {
        Application_Type: 'web'
        Flow_Type: 'Bluefield'
        Request_Source: 'rest'
        RetentionInDays: 90
    }
    tags: tags
}

resource insightsActionGroup 'Microsoft.Insights/actionGroups@2022-06-01' = {
  name: resourceNames.appInsightsActionGroup
  location: 'germanywestcentral'
  properties: {
    groupShortName: 'Group'
    enabled: true
    emailReceivers: alertReceivers
  }
  tags: tags
}

resource insightsAlerts 'microsoft.alertsManagement/smartDetectorAlertRules@2021-04-01' = {
  name: resourceNames.appInsightsAlerts
  location: location
  properties: {
    scope: [
        insights.id
    ]
    actionGroups: {
      customEmailSubject: '[${environment}] Alert'
      groupIds: [
          resourceId('Microsoft.Insights/actionGroups', 'app-insights-action-group-${environment}')
      ]
    }
    description: 'Failure Anomalies notifies you of an unusual rise in the rate of failed HTTP requests or dependency calls.'
    severity: 'Sev3'
    state: 'Enabled'
    frequency: 'PT1M'
    detector: {
        id: 'FailureAnomaliesDetector'
    }
  }
  tags: tags
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: resourceNames.appServicePlan
  location: location
  // https://github.com/Azure/app-service-linux-docs/blob/master/Things_You_Should_Know/kind_property.md#app-service-resource-kind-reference
  kind: 'app,linux'
  sku: {
    name: 'P1v2'
    tier: 'PremiumV2'
    size: 'P1v2'
    family: 'Pv2'
    capacity: 1
  }
  // https://stackoverflow.com/a/61731474
  properties: {
    reserved: true
  }
  tags: tags
}

resource appService 'Microsoft.Web/sites@2024-04-01' = {
  name: resourceNames.appService
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      minTlsVersion: '1.2'
      linuxFxVersion: 'DOTNETCORE|10.0'
      use32BitWorkerProcess: false
      alwaysOn: true
    }
  }
  tags: tags
}

output appInsightInstrumentationKey string = insights.properties.InstrumentationKey
output appInsightConnectionString string = insights.properties.ConnectionString
output principalId string = appService.identity.principalId
