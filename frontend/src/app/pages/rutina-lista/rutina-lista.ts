import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';

import { ETIQUETAS_DIA, RutinaResumen } from '../../models/rutina';
import { RutinaService } from '../../services/rutina.service';

// ------------------------------------------------------------
// COMPONENTE RutinaLista: pantalla principal con todas las rutinas
// ------------------------------------------------------------
// Un componente = una clase TypeScript (datos y lógica) + un HTML (template)
// + un CSS propio. El decorador @Component los une y le dice a Angular cómo
// tratar esta clase (como [ApiController] en .NET).
@Component({
  // Nombre del "tag" HTML. Esta pantalla la muestra el router, así que no
  // lo escribimos a mano en ningún lado, pero todo componente necesita uno.
  selector: 'app-rutina-lista',
  // imports: TODO lo que usa el template y no es HTML común.
  // Usamos el pipe "date" -> hay que importar DatePipe. Si te olvidás un
  // import, Angular muestra un error de compilación en la terminal.
  // (@if y @for NO se importan: son sintaxis propia del compilador.)
  imports: [DatePipe],
  templateUrl: './rutina-lista.html',
  styleUrl: './rutina-lista.css',
  // OnPush: Angular vuelve a dibujar este componente solo cuando cambia un
  // signal que usa el template, llega un input nuevo o hay un evento del
  // template (un click). Es más eficiente y encaja con los signals.
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RutinaLista implements OnInit {
  // "private": solo se usa dentro de la clase.
  private readonly rutinaService = inject(RutinaService);

  // ------------------------------------------------------------------
  // ESTADO DE LA PANTALLA con SIGNALS
  // ------------------------------------------------------------------
  // Un signal es una "caja" que guarda un valor y AVISA cuando cambia.
  // Esta app es "zoneless": Angular se entera de que tiene que redibujar
  // la pantalla cuando cambia un signal. Si usáramos una propiedad común
  // (rutinas: RutinaResumen[] = []) y la cambiáramos dentro de un subscribe,
  // la pantalla NO se actualizaría.
  //
  //   LEER:     this.rutinas()           (con paréntesis, como una función)
  //   ESCRIBIR: this.rutinas.set(valor)
  //
  // "protected": se puede usar desde el template (HTML) pero no desde afuera.
  protected readonly rutinas = signal<RutinaResumen[]>([]);
  protected readonly cargando = signal(true);
  // "string | null": o hay un mensaje de error, o no hay (null).
  protected readonly error = signal<string | null>(null);

  // COMPUTED: un valor DERIVADO de otros signals. Se recalcula solo cuando
  // cambia "rutinas" (Angular detecta qué signals leés adentro).
  protected readonly cantidad = computed(() => this.rutinas().length);

  // Guardamos la constante en la clase para poder usarla en el template.
  protected readonly etiquetasDia = ETIQUETAS_DIA;

  // ngOnInit es un "hook del ciclo de vida": Angular lo llama UNA vez, apenas
  // crea el componente. Es el lugar para cargar datos de la API.
  // "implements OnInit" hace que el compilador avise si escribís mal el nombre.
  ngOnInit(): void {
    this.cargarRutinas();
  }

  // Lo usa ngOnInit y también el botón "Reintentar" del template.
  protected cargarRutinas(): void {
    this.cargando.set(true);
    this.error.set(null);

    // subscribe() es lo que DISPARA la petición HTTP.
    // Recibe un objeto con dos funciones:
    //   next  -> se ejecuta cuando la API responde bien (2xx) con los datos.
    //   error -> se ejecuta si falla (API apagada, 4xx, 5xx...).
    // (Para peticiones HTTP no hace falta desuscribirse: el Observable se
    //  completa solo después de la respuesta.)
    this.rutinaService.listar().subscribe({
      next: (rutinas) => {
        this.rutinas.set(rutinas);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar las rutinas. ¿Está corriendo la API en http://localhost:5000?');
        this.cargando.set(false);
      },
    });
  }
}
