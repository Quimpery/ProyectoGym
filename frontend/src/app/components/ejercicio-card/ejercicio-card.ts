import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

import { Ejercicio } from '../../models/rutina';

// ------------------------------------------------------------
// COMPONENTE EjercicioCard: muestra UN ejercicio
// ------------------------------------------------------------
// Uso desde el padre:  <app-ejercicio-card [ejercicio]="ejercicio" />
// Los CORCHETES [ejercicio] significan "lo de adentro de las comillas es
// código TypeScript" (una variable). Sin corchetes, ejercicio="ejercicio"
// pasaría el TEXTO literal "ejercicio".
@Component({
  selector: 'app-ejercicio-card',
  imports: [DecimalPipe],
  template: `
    <article class="ejercicio">
      <span class="orden" aria-hidden="true">{{ ejercicio().orden }}</span>
      <div>
        <h3>{{ ejercicio().nombre }}</h3>
        <p class="carga">
          {{ ejercicio().series }} series × {{ ejercicio().repeticiones }} repeticiones ·
          @if (tienePeso()) {
            <!-- pipe number: '1.0-2' = mínimo 1 entero, 0 a 2 decimales. En es-AR: 80,5 -->
            {{ ejercicio().peso | number: '1.0-2' }} kg
          } @else {
            Peso corporal
          }
        </p>
        @if (ejercicio().notas) {
          <p class="notas">{{ ejercicio().notas }}</p>
        }
      </div>
    </article>
  `,
  styles: `
    .ejercicio {
      display: flex;
      align-items: flex-start;
      gap: 0.75rem;
      padding: 0.65rem;
      border: 1px solid var(--color-borde);
      border-radius: 8px;
      background: var(--color-superficie);
    }

    /* Circulito con el número de orden. */
    .orden {
      display: grid;
      place-items: center;
      flex-shrink: 0;
      width: 1.75rem;
      height: 1.75rem;
      border-radius: 50%;
      font-size: 0.85rem;
      font-weight: 700;
      color: var(--color-primario-oscuro);
      background: var(--color-primario-claro);
    }

    h3 {
      margin: 0;
      font-size: 1rem;
    }

    .carga {
      margin: 0.15rem 0 0;
      font-size: 0.9rem;
      color: var(--color-texto-suave);
    }

    .notas {
      margin: 0.25rem 0 0;
      font-size: 0.85rem;
      font-style: italic;
      color: var(--color-texto-suave);
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EjercicioCard {
  // input.required: el padre ESTÁ OBLIGADO a pasarlo (si no, error de compilación).
  readonly ejercicio = input.required<Ejercicio>();

  // Un computed puede leer inputs, porque los inputs también son signals.
  protected readonly tienePeso = computed(() => this.ejercicio().peso !== null);
}
