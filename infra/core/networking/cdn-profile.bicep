metadata description = 'Creates an Azure CDN profile.'
param name string
param location string = resourceGroup().location
param tags object = {}

@description('The pricing tier of this CDN profile')
@allowed([
  'Custom_Verizon'
  'Premium_AzureFrontDoor'
  'Premium_Verizon'
  'StandardPlus_955BandWidth_ChinaCdn'
  'StandardPlus_AvgBandWidth_ChinaCdn'
  'StandardPlus_ChinaCdn'
  'Standard_955BandWidth_ChinaCdn'
  'Standard_Akamai'
  'Standard_AvgBandWidth_ChinaCdn'
  'Standard_AzureFrontDoor'
  'Standard_ChinaCdn'
  'Standard_Microsoft'
  'Standard_Verizon'
])
param sku string = 'Standard_Microsoft'

resource profile 'Microsoft.Cdn/profiles@2022-05-01-preview' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: sku
  }
}

output id string = profile.id
output name string = profile.name
