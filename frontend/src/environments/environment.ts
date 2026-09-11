// ------------------------------------------------------------
// CONFIGURACIÓN DE PRODUCCIÓN (se usa con "ng build")
// ------------------------------------------------------------
// Cuando se publique el backend en un servidor real, cambiar apiUrl por la
// URL pública de la API (por ejemplo 'https://mi-gym-api.com/api').
// Con "ng serve" (desarrollo) se usa environment.development.ts: Angular
// reemplaza este archivo por aquel (ver "fileReplacements" en angular.json).
export const environment = {
  apiUrl: 'http://localhost:5000/api',
};
