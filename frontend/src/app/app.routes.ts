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
  // import('...') carga el archivo y .then(m => m.ListaRutinas) toma la clase.
  {
    path: 'rutinas',
    loadComponent: () => import('./pages/lista-rutinas/lista-rutinas').then((m) => m.ListaRutinas),
    title: 'Rutinas | Gym Rutinas', // texto de la pestaña del navegador
  },

  // ALTA. IMPORTANTE: tiene que ir ANTES de 'rutinas/:id'. Si no, el router
  // tomaría la palabra "nueva" como si fuera un id.
  {
    path: 'rutinas/nueva',
    loadComponent: () => import('./pages/formulario-rutina/formulario-rutina').then((m) => m.FormularioRutina),
    title: 'Nueva rutina | Gym Rutinas',
  },

  // ":id" es un PARÁMETRO: coincide con /rutinas/1, /rutinas/25, etc.
  // Su valor llega al input "id" de DetalleRutina.
  {
    path: 'rutinas/:id',
    loadComponent: () => import('./pages/detalle-rutina/detalle-rutina').then((m) => m.DetalleRutina),
    title: 'Detalle de rutina | Gym Rutinas',
  },

  // EDICIÓN: usa el MISMO componente que el alta; como hay :id, sabe que edita.
  {
    path: 'rutinas/:id/editar',
    loadComponent: () => import('./pages/formulario-rutina/formulario-rutina').then((m) => m.FormularioRutina),
    title: 'Editar rutina | Gym Rutinas',
  },

  // "**" = cualquier otra URL que no exista. Va SIEMPRE AL FINAL.
  { path: '**', redirectTo: 'rutinas' },
];
