// ------------------------------------------------------------
// CONFIGURACIÓN DE DESARROLLO (se usa con "ng serve" / "npm start")
// ------------------------------------------------------------
// URL base de la API .NET. Tiene que coincidir con el puerto de
// Backend/GymApi/Properties/launchSettings.json.
// En el código SIEMPRE se importa "environments/environment" (sin
// .development): Angular hace el reemplazo solo al compilar.
export const environment = {
  apiUrl: 'http://localhost:5000/api',
};
