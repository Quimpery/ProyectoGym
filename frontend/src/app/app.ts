import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { Mensajes } from './components/mensajes/mensajes';

// ------------------------------------------------------------
// COMPONENTE RAÍZ (App)
// ------------------------------------------------------------
// Es el primer componente que se dibuja: main.ts lo arranca y Angular lo
// pone adentro de <app-root> en index.html.
// Su template (app.html) es el "layout" fijo de la app (como _Layout.cshtml
// en MVC): la barra de arriba no cambia; lo que cambia según la URL se
// dibuja adentro de <router-outlet>.
@Component({
  selector: 'app-root',
  // Todo lo que usa app.html y no es HTML común:
  //   RouterOutlet     -> <router-outlet />
  //   RouterLink       -> routerLink="/rutinas"
  //   RouterLinkActive -> routerLinkActive="activo"
  //   Mensajes         -> <app-mensajes />
  imports: [RouterOutlet, RouterLink, RouterLinkActive, Mensajes],
  templateUrl: './app.html',
  styleUrl: './app.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {}
