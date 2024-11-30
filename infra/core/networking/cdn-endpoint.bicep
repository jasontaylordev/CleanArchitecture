metadata description = 'Adds an endpoint to an Azure CDN profile.'
param name string
param location string = resourceGroup().location
param tags object = {}

@description('The name of the CDN profile resource')
@minLength(1)
param cdnProfileName string

@description('Delivery policy rules')
param deliveryPolicyRules array = []

@description('The origin URL for the endpoint')
@minLength(1)
param originUrl string

resource endpoint 'Microsoft.Cdn/profiles/endpoints@2022-05-01-preview' = {
  parent: cdnProfile
  name: name
  location: location
  tags: tags
  properties: {
    originHostHeader: originUrl
    isHttpAllowed: false
    isHttpsAllowed: true
    queryStringCachingBehavior: 'UseQueryString'
    optimizationType: 'GeneralWebDelivery'
    origins: [
      {
        name: replace(originUrl, '.', '-')
        properties: {
          hostName: originUrl
          originHostHeader: originUrl
          priority: 1
          weight: 1000
          enabled: true
        }
      }
    ]
    deliveryPolicy: {
      rules: deliveryPolicyRules
    }
  }
}

resource cdnProfile 'Microsoft.Cdn/profiles@2022-05-01-preview' existing = {
  name: cdnProfileName
}

output id string = endpoint.id
output name string = endpoint.name
output uri string = 'https://${endpoint.properties.hostName}'
