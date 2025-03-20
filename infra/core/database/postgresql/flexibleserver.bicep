metadata description = 'Creates an Azure Database for PostgreSQL - Flexible Server.'
param name string
param location string = resourceGroup().location
param tags object = {}

param sku object
param storage object
param appUserLogin string
@secure()
param appUserLoginPassword string
param administratorLogin string
@secure()
param administratorLoginPassword string
param databaseName string
param allowAzureIPsFirewall bool = false
param allowAllIPsFirewall bool = false
param allowedSingleIPs array = []
param keyVaultName string
param connectionStringKey string

// PostgreSQL version
param version string

param utcNowString string = utcNow('yyyyMMddHHmm')

// Latest official version 2022-12-01 does not have Bicep types available
resource postgresServer 'Microsoft.DBforPostgreSQL/flexibleServers@2022-12-01' = {
  location: location
  tags: tags
  name: name
  sku: sku
  properties: {
    version: version
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    storage: storage
    highAvailability: {
      mode: 'Disabled'
    }
  }

  resource database 'databases' = {
    name: databaseName
  }

  resource firewall_all 'firewallRules' = if (allowAllIPsFirewall) {
    name: 'allow-all-IPs'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '255.255.255.255'
    }
  }

  resource firewall_azure 'firewallRules' = if (allowAzureIPsFirewall) {
    name: 'allow-all-azure-internal-IPs'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }

  resource firewall_single 'firewallRules' = [for ip in allowedSingleIPs: {
    name: 'allow-single-${replace(ip, '.', '')}'
    properties: {
      startIpAddress: ip
      endIpAddress: ip
    }
  }]
}

resource psqlDeploymentScript 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: '${name}-deployment-script'
  location: location
  kind: 'AzureCLI'
  properties: {
    azCliVersion: '2.37.0'
    retentionInterval: 'PT1H' // Retain the script resource for 1 hour after it ends running
    timeout: 'PT5M' // Five minutes
    cleanupPreference: 'OnSuccess'
    forceUpdateTag: utcNowString
    environmentVariables: [
      {
        name: 'APPUSERLOGIN'
        value: appUserLogin
      }
      {
        name: 'APPUSERPASSWORD'
        secureValue: appUserLoginPassword
      }
      {
        name: 'DBNAME'
        value: databaseName
      }
      {
        name: 'DBSERVER'
        value: name
      }
      {
        name: 'ADMINLOGIN'
        value: administratorLogin
      }
      {
        name: 'ADMINLOGINPASSWORD'
        secureValue: administratorLoginPassword
      }
    ]

    scriptContent: '''
apk add postgresql-client

cat << EOF > create_user.sql
CREATE ROLE "$APPUSERLOGIN" WITH LOGIN PASSWORD '$APPUSERPASSWORD';
GRANT ALL PRIVILEGES ON DATABASE $DBNAME TO "$APPUSERLOGIN";
EOF

psql "host=$DBSERVER.postgres.database.azure.com user=$ADMINLOGIN dbname=$DBNAME port=5432 password=$ADMINLOGINPASSWORD sslmode=require" < create_user.sql
    '''
  }
  dependsOn: [
    postgresServer
  ]
}

resource administratorLoginPasswordSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault
  name: 'dbAdminPassword'
  properties: {
    value: administratorLoginPassword
  }
}

resource appUserLoginPasswordSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault
  name: 'dbAppUserPassword'
  properties: {
    value: appUserLoginPassword
  }
}

resource sqlAzureConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault
  name: connectionStringKey
  properties: {
    value: '${connectionString}; Password=${appUserLoginPassword}'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

var connectionString = 'Host=${postgresServer.properties.fullyQualifiedDomainName};Port=5432;Database=${databaseName};Username=${appUserLogin}'
output connectionStringKey string = connectionStringKey
