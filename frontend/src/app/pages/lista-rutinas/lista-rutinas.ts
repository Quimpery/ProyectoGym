import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { Observable, Subject, catchError, debounceTime, distinctUntilChanged, map, of, startWith, switchMap, tap } from 'rxjs';

import { Buscador } from '../../components/buscador/buscador';
import { ETIQUETAS_DIA, RutinaResumen } from '../../models/rutina';
import { MensajeService } from '../../services/mensaje.service';
import { RutinaService } from '../../services/rutina.service';

// ------------------------------------------------------------
// PÁGINA ListaRutinas: listado + búsqueda en tiempo real + eliminar
// ------------------------------------------------------------
@Component({
  selector: 'app-lista-rutinas',
  // Buscador es un componente nuestro: también hay que importarlo.
  // RouterLink hace falta para los links [routerLink] al detalle.
  imports: [DatePipe, RouterLink, Buscador],
  templateUrl: './lista-rutinas.html',
  styleUrl: './lista-rutinas.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ListaRutinas implements OnInit {
  private readonly rutinaService = inject(RutinaService);
  private readonly mensajeService = inject(MensajeService);
  // DestroyRef "sabe" cuándo se destruye el componente (lo usamos más abajo).
  private readonly destroyRef = inject(DestroyRef);

  // ----- Estado de la pantalla (signals) -----
  protected readonly rutinas = signal<RutinaResumen[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);
  // Texto que se está buscando ('' = sin filtro, se muestran todas).
  protected readonly textoBusqueda = signal('');
  // Id de la rutina que se está eliminando (para deshabilitar SU botón).
  protected readonly eliminandoId = signal<number | null>(null);

  protected readonly etiquetasDia = ETIQUETAS_DIA;

  // computed que depende de DOS signals: se recalcula si cambia cualquiera.
  protected readonly resumen = computed(() => {
    const cantidad = this.rutinas().length;
    const texto = this.textoBusqueda();
    if (texto) {
      return cantidad === 1 ? `1 rutina encontrada para "${texto}"` : `${cantidad} rutinas encontradas para "${texto}"`;
    }
    return cantidad === 1 ? '1 rutina' : `${cantidad} rutinas`;
  });

  // SUBJECT: un Observable al que NOSOTROS le "empujamos" valores con next().
  // Cada vez que el usuario escribe, mandamos el texto acá. Por convención,
  // las variables que son Observables terminan en $.
  private readonly busqueda$ = new Subject<string>();

  ngOnInit(): void {
    // ------------------------------------------------------------------
    // BÚSQUEDA EN TIEMPO REAL con operadores de RxJS
    // ------------------------------------------------------------------
    // pipe() encadena OPERADORES: cada uno recibe lo que emite el anterior
    // y lo transforma (parecido a encadenar .Where().Select() en LINQ).
    this.busqueda$
      .pipe(
        // 1) Espera a que el usuario deje de tipear 300 ms. Si escribe
        //    "fuerza" rápido, se hace UNA petición y no seis.
        debounceTime(300),
        // 2) Quita espacios de los costados.
        map((texto) => texto.trim()),
        // 3) Emite '' al empezar: así la PRIMERA CARGA (todas las rutinas)
        //    también pasa por este mismo camino.
        startWith(''),
        // 4) Si el texto es igual al anterior (ej. se agregó un espacio al
        //    final), no vuelve a buscar.
        distinctUntilChanged(),
        // 5) tap() ejecuta algo "al costado" sin transformar el valor.
        tap((texto) => {
          this.textoBusqueda.set(texto);
          this.cargando.set(true);
          this.error.set(null);
        }),
        // 6) switchMap cambia el texto por la PETICIÓN HTTP. Si llega un texto
        //    nuevo mientras la petición anterior no terminó, CANCELA la
        //    anterior. Así nunca se muestra el resultado de una búsqueda vieja.
        switchMap((texto) => this.pedirRutinas(texto)),
        // 7) Cuando el componente se destruye (el usuario sale de la página),
        //    se corta la suscripción. Evita "fugas de memoria".
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((rutinas) => this.mostrarResultado(rutinas));
  }

  // El Buscador (hijo) emite el texto -> lo empujamos al Subject.
  protected alBuscar(texto: string): void {
    this.busqueda$.next(texto);
  }

  // Botón "Reintentar": repite la búsqueda actual.
  protected reintentar(): void {
    this.cargando.set(true);
    this.error.set(null);
    this.pedirRutinas(this.textoBusqueda()).subscribe((rutinas) => this.mostrarResultado(rutinas));
  }

  protected eliminar(rutina: RutinaResumen): void {
    // confirm() es la ventanita nativa del navegador con Aceptar/Cancelar.
    // Devuelve true si el usuario aceptó.
    const confirmado = confirm(
      `¿Seguro que querés eliminar la rutina "${rutina.nombre}"?\n` +
        `También se van a eliminar sus ${rutina.cantidadEjercicios} ejercicios. Esta acción no se puede deshacer.`,
    );
    if (!confirmado) {
      return;
    }

    this.eliminandoId.set(rutina.id);
    this.rutinaService.eliminar(rutina.id).subscribe({
      next: () => {
        // Quitamos la rutina de la lista SIN volver a pedir todo a la API.
        this.rutinas.update((lista) => lista.filter((r) => r.id !== rutina.id));
        this.eliminandoId.set(null);
        this.mensajeService.exito(`Se eliminó la rutina "${rutina.nombre}".`);
      },
      error: (err: unknown) => {
        this.eliminandoId.set(null);
        this.mensajeService.errorHttp(err);
      },
    });
  }

  // Con texto -> GET /buscar?nombre=texto ; sin texto -> GET /rutinas.
  // Devuelve null si la petición falla.
  private pedirRutinas(texto: string): Observable<RutinaResumen[] | null> {
    const peticion = texto ? this.rutinaService.buscar(texto) : this.rutinaService.listar();
    // catchError atrapa el error y devuelve otro Observable en su lugar:
    // of(null) emite null. Es CLAVE ponerlo acá adentro: si un error llegara
    // hasta el pipe de la búsqueda, lo cortaría y el buscador dejaría de andar.
    return peticion.pipe(catchError(() => of(null)));
  }

  private mostrarResultado(rutinas: RutinaResumen[] | null): void {
    if (rutinas === null) {
      this.error.set('No se pudieron cargar las rutinas. ¿Está corriendo la API en http://localhost:5000?');
    } else {
      this.rutinas.set(rutinas);
    }
    this.cargando.set(false);
  }
}
