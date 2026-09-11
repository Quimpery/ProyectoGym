import { Routes } from '@angular/router';

// ------------------------------------------------------------
// TABLA DE RUTAS: qué componente se muestra para cada URL
// ------------------------------------------------------------
// El router recorre la lista EN ORDEN y usa la primera ruta que coincide.
export const routes: Routes = [
  // URL vacía (http://localhost:4200/) -> redirige a /rutinas.
  // pathMatch: 'full' = solo si la URL es EXACTAMENTE vacía.
  { path: '', redirectTo: 'rutinas', pathMatch: 'full' },

  // LAZY LOADING con loadComponent: el código de esta pantalla se descarga
  // recién cuando el usuario entra a /rutinas, no al abrir la app.
  // import('...') carga el archivo y .then(m => m.RutinaLista) toma la clase.
  {
    path: 'rutinas',
    loadComponent: () => import('./pages/rutina-lista/rutina-lista').then((m) => m.RutinaLista),
    title: 'Rutinas | Gym Rutinas', // texto de la pestaña del navegador
  },

  // "**" = cualquier otra URL que no exista. Va SIEMPRE AL FINAL.
  { path: '**', redirectTo: 'rutinas' },
];
