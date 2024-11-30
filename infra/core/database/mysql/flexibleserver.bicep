metadata description = 'Creates an Azure Database for MySQL - Flexible Server.'
param name string
param location string = resourceGroup().location
param tags object = {}

param sku object
param storage object
param administratorLogin string
@secure()
param administratorLoginPassword string
param highAvailabilityMode string = 'Disabled'
param databaseNames array = []
param allowAzureIPsFirewall bool = false
param allowAllIPsFirewall bool = false
param allowedSingleIPs array = []

// MySQL version
param version string

resource mysqlServer 'Microsoft.DBforMySQL/flexibleServers@2023-06-30' = {
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
      mode: highAvailabilityMode
    }
  }

  resource database 'databases' = [for name in databaseNames: {
    name: name
  }]

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

output MYSQL_DOMAIN_NAME string = mysqlServer.properties.fullyQualifiedDomainName
