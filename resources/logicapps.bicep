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

// Logic App suffix
param suffix string {
    allowed: [
        'latest'
        'random'
    ]
    default: 'latest'
}

// Logic App workflow parameters
// Recurrency
param recurrenceFrequency string
param recurrenceInterval int
param recurrenceStartTime string
param recurrenceTimeZone string = 'Korea Standard Time'

// Feed Details
param feedSource string {
    allowed: [
        'Blog'
        'YouTube'
    ]
}
param feedUri string
param feedNumberOfItems int = 10
param feedIsRandom bool = false
param feedPrefixesExcluded string
param feedPrefixesIncluded string

// Feed Item Picker
param feedItemPickerUrl string
param feedItemPickerAuthKey string {
    secure: true
}

var metadata = {
    longName: '{0}-${name}-${env}-${locationCode}-twitter'
    shortName: '{0}${name}${env}${locationCode}'
}

var apiConnTwitter = {
    connectionId: '${resourceGroup().id}/providers/Microsoft.Web/connections/${format(metadata.longName, 'apicon')}-{0}'
    connectionName: '${format(metadata.longName, 'apicon')}-{0}'
    id: '${subscription().id}/providers/Microsoft.Web/locations/${location}/managedApis/twitter'
    location: location
}

resource apiconAzpls 'Microsoft.Web/connections@2018-07-01-preview' = {
    name: '${format(apiConnTwitter.connectionName, 'azpls')}'
    location: apiConnTwitter.location
    kind: 'V1'
    properties: {
        displayName: '${format(apiConnTwitter.connectionName, 'azpls')}'
        api: {
            id: apiConnTwitter.id
        }
    }
}

resource apiconJustin 'Microsoft.Web/connections@2018-07-01-preview' = {
    name: '${format(apiConnTwitter.connectionName, 'justin')}'
    location: apiConnTwitter.location
    kind: 'V1'
    properties: {
        displayName: '${format(apiConnTwitter.connectionName, 'justin')}'
        api: {
            id: apiConnTwitter.id
        }
    }
}

var logicApp = {
    name: '${format(metadata.longName, 'logapp')}-${suffix}'
    location: location
}

var recurrence = {
    frequency: recurrenceFrequency
    interval: recurrenceInterval
    startTime: recurrenceStartTime
    timeZone: recurrenceTimeZone
}

var feedDetails = {
    feedSource: feedSource
    feedUri: feedUri
    numberOfFeedItems: feedNumberOfItems
    isRandom: feedIsRandom
    prefixesExcluded: split(coalesce(feedPrefixesExcluded, ''), ',')
    prefixesIncluded: split(coalesce(feedPrefixesIncluded, ''), ',')
    postHeader: '${feedIsRandom ? '애저 듣보잡 비디오 다시 보기:' : '애저 듣보잡 새 비디오 업로드:'}'
}

var feedItemPicker = {
    url: feedItemPickerUrl
    authKey: feedItemPickerAuthKey
}

resource logapp 'Microsoft.Logic/workflows@2019-05-01' = {
    name: logicApp.name
    location: logicApp.location
    properties: {
        state: 'Enabled'
        parameters: {
            '$connections': {
                value: {
                    twitterAzpls: {
                        connectionId: '${format(apiConnTwitter.connectionId, 'azpls')}'
                        connectionName: apiconAzpls.name
                        id: apiConnTwitter.id
                    }
                    twitterJustin: {
                        connectionId: '${format(apiConnTwitter.connectionId, 'justin')}'
                        connectionName: apiconJustin.name
                        id: apiConnTwitter.id
                    }
                }
            }
            recurrence: {
                value: {
                    frequency: recurrence.frequency
                    interval: recurrence.interval
                    startTime: recurrence.startTime
                    timeZone: recurrence.timeZone
                }
            }
            feedDetails: {
                value: {
                    feedSource: feedDetails.feedSource
                    feedUri: feedDetails.feedUri
                    numberOfFeedItems: feedDetails.numberOfFeedItems
                    isRandom: feedDetails.isRandom
                    prefixesExcluded: feedDetails.prefixesExcluded
                    prefixesIncluded: feedDetails.prefixesIncluded
                    postHeader: feedDetails.postHeader
                }
            }
            feedItemPicker: {
                value: {
                    url: feedItemPicker.url
                    authKey: feedItemPicker.authKey
                }
            }
        }
        definition: {
            '$schema': 'https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#'
            contentVersion: '1.0.0.0'
            parameters: {
                '$connections': {
                    type: 'object'
                    defaultValue: {}
                }
                recurrence: {
                    type: 'object'
                    defaultValue: {}
                }
                feedDetails: {
                    type: 'object'
                    defaultValue: {}
                }
                feedItemPicker: {
                    type: 'object'
                    defaultValue: {}
                }
            }
            triggers: {
                Run_Everyday_at_Given_Time: {
                    type: 'recurrence'
                    recurrence: {
                        frequency: '@parameters(\'recurrence\')[\'frequency\']'
                        interval: '@parameters(\'recurrence\')[\'interval\']'
                        startTime: '@parameters(\'recurrence\')[\'startTime\']'
                        timeZone: '@parameters(\'recurrence\')[\'timeZone\']'
                    }
                }
            }
            actions: {
                Build_Request_Payload: {
                    type: 'Compose'
                    runAfter: {}
                    inputs: {
                        feedSource: '@parameters(\'feedDetails\')[\'feedSource\']'
                        feedUri: '@parameters(\'feedDetails\')[\'feedUri\']'
                        numberOfFeedItems: '@parameters(\'feedDetails\')[\'numberOfFeedItems\']'
                        isRandom: '@parameters(\'feedDetails\')[\'isRandom\']'
                        prefixesExcluded: '@parameters(\'feedDetails\')[\'prefixesExcluded\']'
                        prefixesIncluded: '@parameters(\'feedDetails\')[\'prefixesIncluded\']'
                    }
                }
                Fetch_Latest_FeedItem: {
                    type: 'Http'
                    runAfter: {
                        Build_Request_Payload: [
                            'Succeeded'
                        ]
                    }
                    inputs: {
                        method: 'POST'
                        uri: '@parameters(\'feedItemPicker\')[\'url\']'
                        headers: {
                            'x-functions-key': '@parameters(\'feedItemPicker\')[\'authKey\']'
                        }
                        body: '@outputs(\'Build_Request_Payload\')'
                    }
                }
                Split_Description: {
                    type: 'Compose'
                    runAfter: {
                        Fetch_Latest_FeedItem: [
                            'Succeeded'
                        ]
                    }
                    inputs: '@split(body(\'Fetch_Latest_FeedItem\')?[\'description\'], \'---\')'
                }
                Build_Tweet_Post: {
                    type: 'Compose'
                    runAfter: {
                        Split_Description: [
                            'Succeeded'
                        ]
                    }
                    inputs: '@{parameters(\'feedDetails\')[\'postHeader\']}\n\n@{trim(first(outputs(\'Split_Description\')))}\n\n@{body(\'Fetch_Latest_FeedItem\')?[\'link\']}'
                }
                Post_Tweet: {
                    type: 'ApiConnection'
                    runAfter: {
                        Build_Tweet_Post: [
                            'Succeeded'
                        ]
                    }
                    inputs: {
                        method: 'POST'
                        host: {
                            connection: {
                                name: '@parameters(\'$connections\')[\'twitterAzpls\'][\'connectionId\']'
                            }
                        }
                        path: '/posttweet'
                        queries: {
                            tweetText: '@{outputs(\'Build_Tweet_Post\')}'
                        }
                    }
                }
                Retweet: {
                    type: 'ApiConnection'
                    runAfter: {
                        Post_Tweet: [
                            'Succeeded'
                        ]
                    }
                    inputs: {
                        method: 'POST'
                        host: {
                            connection: {
                                name: '@parameters(\'$connections\')[\'twitterJustin\'][\'connectionId\']'
                            }
                        }
                        path: '/retweet'
                        queries: {
                            tweetId: '@body(\'Post_Tweet\')[\'TweetId\']'
                            trimUser: false
                        }
                    }
                }
            }
            outputs: {}
        }
    }
}