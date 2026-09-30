// 1. Definimos el Plan de Hosting Gratuito Linux F1
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: 'MiniERP-Free-Plan'
  location: 'southcentralus'
  sku: {
    name: 'F1'
    tier: 'Free'
  }
  kind: 'linux'
  properties: {
    reserved: true // Obligatorio para Linux
  }
}

// 2. Definimos la Web API basada en tu contenedor Docker
resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: 'minierp-api-brad-paredes'
  location: 'southcentralus'
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOCKER|docker.io/bradparedes/minierp-api:latest'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'WEBSITES_PORT'
          value: '8080'
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
      ]
    }
  }
}
