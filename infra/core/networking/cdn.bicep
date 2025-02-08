metadata description = 'Creates an Azure CDN profile with a single endpoint.'
param location string = resourceGroup().location
param tags object = {}

@description('Name of the CDN endpoint resource')
param cdnEndpointName string

@description('Name of the CDN profile resource')
param cdnProfileName string

@description('Delivery policy rules')
param deliveryPolicyRules array = []

@description('Origin URL for the CDN endpoint')
param originUrl string

module cdnProfile 'cdn-profile.bicep' = {
  name: 'cdn-profile'
  params: {
    name: cdnProfileName
    location: location
    tags: tags
  }
}

module cdnEndpoint 'cdn-endpoint.bicep' = {
  name: 'cdn-endpoint'
  params: {
    name: cdnEndpointName
    location: location
    tags: tags
    cdnProfileName: cdnProfile.outputs.name
    originUrl: originUrl
    deliveryPolicyRules: deliveryPolicyRules
  }
}

output endpointName string = cdnEndpoint.outputs.name
output endpointId string = cdnEndpoint.outputs.id
output profileName string = cdnProfile.outputs.name
output profileId string = cdnProfile.outputs.id
output uri string = cdnEndpoint.outputs.uri
