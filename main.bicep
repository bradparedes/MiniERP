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
    reserved: true 
  }
}

// 2. Definimos la Web API basada en Código Nativo .NET 8
resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: 'minierp-api-brad-paredes'
  location: 'southcentralus'
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNET|8.0' // 🚀 Cambiado a código nativo puro de .NET 8
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
      ]
    }
  }
}
