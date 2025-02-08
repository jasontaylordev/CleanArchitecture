metadata description = 'Creates an Application Insights instance based on an existing Log Analytics workspace.'
param name string
param dashboardName string = ''
param location string = resourceGroup().location
param tags object = {}
param logAnalyticsWorkspaceId string

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: name
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}

module applicationInsightsDashboard 'applicationinsights-dashboard.bicep' = if (!empty(dashboardName)) {
  name: 'application-insights-dashboard'
  params: {
    name: dashboardName
    location: location
    applicationInsightsName: applicationInsights.name
  }
}

output connectionString string = applicationInsights.properties.ConnectionString
output id string = applicationInsights.id
output instrumentationKey string = applicationInsights.properties.InstrumentationKey
output name string = applicationInsights.name
