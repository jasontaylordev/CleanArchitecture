param resourceNames object
param location string
param tags object
param appServicePrincipalId string
param remoteAccessEntraGroupName string
param remoteAccessEntraGroupSID string
param remoteAccessFrom string

// https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#databases  (SQL DB Contributor in ↓ case)
var roleDefinitionId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '9b7fa17d-e63e-47b0-bb0a-15c516ac86ec')
var sqlServerHostname = az.environment().suffixes.sqlServerHostname

resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: resourceNames.sqlServer
  location: location
  properties: {
    minimalTlsVersion: '1.2'
    administrators: {
      administratorType: 'ActiveDirectory'
      principalType: 'Group'
      login: remoteAccessEntraGroupName
      sid: remoteAccessEntraGroupSID
      tenantId: subscription().tenantId
      azureADOnlyAuthentication: true
    }
    version: '12.0'
  }
  tags: tags
  identity:{
    type: 'SystemAssigned'
  }
}

resource database 'Microsoft.Sql/servers/databases@2021-11-01' = {
    name: resourceNames.sqlDatabase
    location: location
    parent: sqlServer
    properties: {
      autoPauseDelay: 120
      catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    }
    sku: {
      name: 'Basic'
      tier: 'Basic'
      capacity: 5
    }
    tags: tags
}

// grant the guard access to the database
resource assignRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: resourceNames.sqlRole
  scope: database
  properties: {
    roleDefinitionId: roleDefinitionId
    principalId: appServicePrincipalId
  }
}

// ToDo: The appService still might have no access to the database
// We require to create a user and grant access to the database
// CREATE USER [resourceNames.appService] FROM EXTERNAL PROVIDER;
// ALTER ROLE db_datareader ADD MEMBER [resourceNames.appService];
// ALTER ROLE db_datawriter ADD MEMBER [resourceNames.appService];
// ALTER ROLE db_ddladmin ADD MEMBER [resourceNames.appService];

// ToDo: We need to create a VNET to avoid this rule.
// Because this rule opens up to ALL azure services (across all tenants)

resource sqlServerInternalAccess 'Microsoft.Sql/servers/firewallRules@2021-11-01' = {
    name: 'AllowAzureIP'
    parent: sqlServer
    properties: {
        startIpAddress: '0.0.0.0'
        endIpAddress: '0.0.0.0'
    }
}

resource sqlServerExternalAccess 'Microsoft.Sql/servers/firewallRules@2021-11-01' = if (remoteAccessFrom != '') {
    name: 'AllowSpecificIP'
    parent: sqlServer
    properties: {
        startIpAddress: remoteAccessFrom
        endIpAddress: remoteAccessFrom
    }
}

output connectionString string = 'Server=${sqlServer.name}${sqlServerHostname};Database=${database.name};Authentication=Active Directory Default;Encrypt=True;Connection Timeout=30;'
