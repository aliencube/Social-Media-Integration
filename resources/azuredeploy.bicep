// Resource name
param name string

// Provisioning environment
param env string {
    allowed: [
        'dev'
        'test'
        'prod'
    ]
    default: 'dev'
}

// Resource location
param location string = resourceGroup().location

// Resource location code
param locationCode string = 'wus2'

// Storage CosmosAccount
param storageAccountSku string = 'Standard_LRS'

// Function App
param functionAppWorkerRuntime string = 'dotnet'
param functionAppEnvironment string {
    allowed: [
        'Development'
        'Staging'
        'Production'
    ]
    default: 'Development'
}
param functionAppTimezone string = 'Korea Standard Time'

var metadata = {
    longName: '{0}-${name}-${env}-${locationCode}'
    shortName: '{0}${name}${env}${locationCode}'
}

var storage = {
    name: format(metadata.shortName, 'st')
    location: location
    sku: storageAccountSku
}

resource st 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storage.name
    location: storage.location
    kind: 'StorageV2'
    sku: {
        name: storage.sku
    }
    properties: {
        supportsHttpsTrafficOnly: true
    }
}

var appInsights = {
    name: format(metadata.longName, 'appins')
    location: location
}

resource appins 'Microsoft.Insights/components@2020-02-02-preview' = {
    name: appInsights.name
    location: appInsights.location
    kind: 'web'
    properties: {
        Application_Type: 'web'
        Request_Source: 'IbizaWebAppExtensionCreate'
    }
}

var servicePlan = {
    name: format(metadata.longName, 'csplan')
    location: location
}

resource csplan 'Microsoft.Web/serverfarms@2019-08-01' = {
    name: servicePlan.name
    location: servicePlan.location
    sku: {
        name: 'Y1'
        tier: 'Dynamic'
    }
    properties: {
        name: servicePlan.name
        computeMode: 'Dynamic'
    }
}

var functionApp = {
    name: format(metadata.longName, 'fncapp')
    location: location
    environment: functionAppEnvironment
    runtime: functionAppWorkerRuntime
    timezone: functionAppTimezone
}

resource fncapp 'Microsoft.Web/sites@2020-06-01' = {
    name: functionApp.name
    location: functionApp.location
    kind: 'functionapp'
    properties: {
        serverFarmId: csplan.id
        httpsOnly: true
        alwaysOn: true
        siteConfig: {
            appSettings: [
                {
                    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
                    value: '${reference(appins.id, '2020-02-02-preview', 'Full').properties.InstrumentationKey}'
                }
                {
                    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
                    value: '${reference(appins.id, '2020-02-02-preview', 'Full').properties.connectionString}'
                }
                {
                    name: 'AZURE_FUNCTIONS_ENVIRONMENT'
                    value: functionApp.environment
                }
                {
                    name: 'AzureWebJobsStorage'
                    value: 'DefaultEndpointsProtocol=https;AccountName=${st.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(st.id, '2019-06-01').keys[0].value}'
                }
                {
                    name: 'FUNCTIONS_EXTENSION_VERSION'
                    value: '~3'
                }
                {
                    name: 'FUNCTION_APP_EDIT_MODE'
                    value: 'readonly'
                }
                {
                    name: 'FUNCTIONS_WORKER_RUNTIME'
                    value: functionApp.runtime
                }
                {
                    name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
                    value: 'DefaultEndpointsProtocol=https;AccountName=${st.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(st.id, '2019-06-01').keys[0].value}'
                }
                {
                    name: 'WEBSITE_CONTENTSHARE'
                    value: functionApp.name
                }
                {
                    name: 'WEBSITE_NODE_DEFAULT_VERSION'
                    value: '~10'
                }
                {
                    name: 'WEBSITE_TIME_ZONE'
                    value: functionApp.timezone
                }
            ]
        }
    }
}
