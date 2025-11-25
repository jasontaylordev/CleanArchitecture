param resourceNames object
param appSettings object

resource appService 'Microsoft.Web/sites@2024-04-01' existing = {
  name: resourceNames.appService
}

resource appServiceConfig 'Microsoft.Web/sites/config@2024-04-01' = {
  parent: appService
  name: 'appsettings'
  properties: appSettings
}
