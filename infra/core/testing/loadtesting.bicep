param name string
param location string = resourceGroup().location
param managedIdentity bool = false
param tags object = {}

resource loadTest 'Microsoft.LoadTestService/loadTests@2022-12-01' = {
  name: name
  location: location
  tags: tags
  identity: { type: managedIdentity ? 'SystemAssigned' : 'None' }
  properties: {
  }
}

output loadTestingName string = loadTest.name
