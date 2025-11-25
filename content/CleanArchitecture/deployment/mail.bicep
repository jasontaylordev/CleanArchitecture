param environment string
param tags object

// wrapper to access the smtp server via different channels. (e.g. REST)
resource api 'Microsoft.Communication/communicationServices@2023-04-01' = {
  name: 'acs-${environment}'
  location: 'global'
  properties: {
    dataLocation: 'Europe'
  }
  tags: tags
}

// SMPT server
resource service 'Microsoft.Communication/emailServices@2023-04-01' = {
    name: 'ac-email-service-${environment}'
    location: 'global'
    properties: {
      dataLocation: 'Europe'
    }
    tags: tags
}

// Domain incl. SPF, DKIM, DKIM2, and DMARC
resource domain 'Microsoft.Communication/emailServices/domains@2023-04-01' = {
  name: 'ac-AzureManagedDomain'
  location: 'global'
  parent: service
  properties: {
    userEngagementTracking: 'Disabled'
    domainManagement: 'AzureManaged'
  }
  tags: tags
}
