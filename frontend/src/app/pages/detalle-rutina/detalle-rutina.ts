import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, input, numberAttribute, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { EjercicioCard } from '../../components/ejercicio-card/ejercicio-card';
import { DIAS_SEMANA, ETIQUETAS_DIA, RutinaDetalle } from '../../models/rutina';
import { MensajeService } from '../../services/mensaje.service';
import { RutinaService } from '../../services/rutina.service';

// ------------------------------------------------------------
// PÁGINA DetalleRutina: una rutina con sus ejercicios agrupados por día
// ------------------------------------------------------------
// Se muestra en la URL /rutinas/:id  (por ejemplo /rutinas/2).
@Component({
  selector: 'app-detalle-rutina',
  imports: [DatePipe, RouterLink, EjercicioCard],
  templateUrl: './detalle-rutina.html',
  styleUrl: './detalle-rutina.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DetalleRutina implements OnInit {
  private readonly rutinaService = inject(RutinaService);
  private readonly mensajeService = inject(MensajeService);
  // Router permite NAVEGAR desde el código (no desde un link).
  private readonly router = inject(Router);

  // PARÁMETRO DE LA URL COMO INPUT:
  // Gracias a withComponentInputBinding() (en app.config.ts), el ":id" de la
  // ruta llega SOLO a este input. Los parámetros de URL siempre son texto
  // ("2"); transform: numberAttribute lo convierte en número (2).
  readonly id = input.required({ transform: numberAttribute });

  protected readonly rutina = signal<RutinaDetalle | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly eliminando = signal(false);

  // ------------------------------------------------------------------
  // EJERCICIOS AGRUPADOS POR DÍA (estado derivado con computed)
  // ------------------------------------------------------------------
  // Recorremos los 7 días EN ORDEN (Lunes -> Domingo), juntamos los
  // ejercicios de cada día y descartamos los días sin ejercicios.
  // Resultado, por ejemplo:
  //   [ { dia: 'Martes', etiqueta: 'Martes', ejercicios: [...] },
  //     { dia: 'Jueves', etiqueta: 'Jueves', ejercicios: [...] } ]
  protected readonly ejerciciosPorDia = computed(() => {
    const rutina = this.rutina();
    if (rutina === null) {
      return [];
    }

    return DIAS_SEMANA
      // map: transforma cada día en un objeto "grupo".
      .map((dia) => ({
        dia,
        etiqueta: ETIQUETAS_DIA[dia],
        ejercicios: rutina.ejercicios
          .filter((ejercicio) => ejercicio.dia === dia)
          // sort ordena por el campo orden (a - b = de menor a mayor).
          .sort((a, b) => a.orden - b.orden),
      }))
      // filter: nos quedamos solo con los días que tienen ejercicios.
      .filter((grupo) => grupo.ejercicios.length > 0);
  });

  ngOnInit(): void {
    // En ngOnInit el input "id" YA tiene valor (en el constructor todavía no).
    this.cargarRutina();
  }

  protected cargarRutina(): void {
    this.cargando.set(true);
    this.error.set(null);

    this.rutinaService.obtener(this.id()).subscribe({
      next: (rutina) => {
        this.rutina.set(rutina);
        this.cargando.set(false);
      },
      error: (err: unknown) => {
        // Si la rutina no existe, la API responde 404 con
        // { mensaje: "No existe una rutina con id 99." } y lo mostramos.
        this.error.set(this.mensajeService.textoDeError(err));
        this.cargando.set(false);
      },
    });
  }

  protected eliminar(rutina: RutinaDetalle): void {
    const confirmado = confirm(
      `¿Seguro que querés eliminar la rutina "${rutina.nombre}"?\n` +
        `También se van a eliminar sus ${rutina.ejercicios.length} ejercicios. Esta acción no se puede deshacer.`,
    );
    if (!confirmado) {
      return;
    }

    this.eliminando.set(true);
    this.rutinaService.eliminar(rutina.id).subscribe({
      next: () => {
        this.mensajeService.exito(`Se eliminó la rutina "${rutina.nombre}".`);
        // Como la rutina ya no existe, volvemos al listado.
        // El aviso sigue visible porque MensajeService vive en toda la app.
        this.router.navigate(['/rutinas']);
      },
      error: (err: unknown) => {
        this.eliminando.set(false);
        this.mensajeService.errorHttp(err);
      },
    });
  }
}
