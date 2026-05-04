@description('Nombre del ambiente de Container Apps')
param containerAppsEnvironmentName string = 'libreria-app-env'

@description('Región de Azure para los recursos')
param location string = resourceGroup().location

@description('Imagen Docker de LibreriaAPI')
param libreriaImage string

@description('Imagen Docker del ApiGateway')
param gatewayImage string

@description('Servidor del registro de contenedores')
param containerRegistryServer string

@description('Usuario del registro de contenedores')
param containerRegistryUsername string

@description('Contraseña del registro de contenedores')
@secure()
param containerRegistryPassword string

@description('Nombre del espacio de trabajo de Log Analytics')
param logAnalyticsWorkspaceName string = 'law-libreria-app'

// Log Analytics Workspace para telemetría
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// Ambiente de Azure Container Apps
resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: containerAppsEnvironmentName
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsWorkspace.properties.customerId
        sharedKey: logAnalyticsWorkspace.listKeys().primarySharedKey
      }
    }
  }
}

// Secretos compartidos del registro de contenedores
var registrySecrets = [
  {
    name: 'registry-password'
    value: containerRegistryPassword
  }
]

var registryConfig = [
  {
    server: containerRegistryServer
    username: containerRegistryUsername
    passwordSecretRef: 'registry-password'
  }
]

// Azure Container App — LibreriaAPI
resource libreriaApiApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: 'libreria-api'
  location: location
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    configuration: {
      ingress: {
        external: false
        targetPort: 8080
        transport: 'http'
      }
      registries: registryConfig
      secrets: registrySecrets
    }
    template: {
      containers: [
        {
          name: 'libreria-api'
          image: libreriaImage
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Production'
            }
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://+:8080'
            }
          ]
          probes: [
            {
              type: 'Liveness'
              httpGet: {
                path: '/health'
                port: 8080
              }
              initialDelaySeconds: 15
              periodSeconds: 30
            }
            {
              type: 'Readiness'
              httpGet: {
                path: '/health'
                port: 8080
              }
              initialDelaySeconds: 5
              periodSeconds: 10
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 5
        rules: [
          {
            name: 'http-scaling'
            http: {
              metadata: {
                concurrentRequests: '50'
              }
            }
          }
        ]
      }
    }
  }
}

// Azure Container App — ApiGateway (punto de entrada externo)
resource apiGatewayApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: 'api-gateway'
  location: location
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        transport: 'http'
        corsPolicy: {
          allowedOrigins: ['*']
          allowedMethods: ['GET', 'POST', 'PUT', 'PATCH', 'DELETE', 'OPTIONS']
          allowedHeaders: ['*']
        }
      }
      registries: registryConfig
      secrets: registrySecrets
    }
    template: {
      containers: [
        {
          name: 'api-gateway'
          image: gatewayImage
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Production'
            }
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://+:8080'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 3
        rules: [
          {
            name: 'http-scaling'
            http: {
              metadata: {
                concurrentRequests: '100'
              }
            }
          }
        ]
      }
    }
  }
}

@description('URL pública del API Gateway')
output gatewayUrl string = 'https://${apiGatewayApp.properties.configuration.ingress.fqdn}'

@description('FQDN interno de LibreriaAPI (accesible desde el ambiente)')
output libreriaApiInternalFqdn string = libreriaApiApp.properties.configuration.ingress.fqdn
