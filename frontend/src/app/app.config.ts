import { registerLocaleData } from '@angular/common';
import { provideHttpClient } from '@angular/common/http';
import localeEsAr from '@angular/common/locales/es-AR';
import { ApplicationConfig, LOCALE_ID, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';

// ------------------------------------------------------------
// app.config.ts: la configuración global de la app
// ------------------------------------------------------------
// Es el equivalente a la parte de builder.Services... de Program.cs en .NET:
// acá se REGISTRAN los servicios que va a usar toda la aplicación.
// Si un componente pide algo que no está registrado, la app falla con un
// error "NullInjectorError: No provider for ...".

// Carga los datos del idioma español (Argentina): nombres de meses, formato
// de fechas y números. Lo usan pipes como "date".
registerLocaleData(localeEsAr);

export const appConfig: ApplicationConfig = {
  providers: [
    // Muestra en la consola los errores no controlados de la app.
    provideBrowserGlobalErrorListeners(),

    // Habilita el ROUTER con la tabla de rutas de app.routes.ts.
    provideRouter(routes),

    // Habilita HttpClient para poder hacer peticiones a la API.
    // Sin esta línea, RutinaService falla al inyectar HttpClient.
    provideHttpClient(),

    // Idioma por defecto de la app: los pipes formatean en español.
    { provide: LOCALE_ID, useValue: 'es-AR' },
  ],
};
