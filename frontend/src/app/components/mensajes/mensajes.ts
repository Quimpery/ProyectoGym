import { ChangeDetectionStrategy, Component, inject } from '@angular/core';

import { MensajeService } from '../../services/mensaje.service';

// ------------------------------------------------------------
// COMPONENTE Mensajes: dibuja los avisos de MensajeService
// ------------------------------------------------------------
// Es un componente chico, así que usamos TEMPLATE y ESTILOS "INLINE" (en el
// mismo archivo, entre backticks) en vez de .html y .css separados.
// Se usa una sola vez, en app.html: <app-mensajes />
@Component({
  selector: 'app-mensajes',
  template: `
    <div class="contenedor-mensajes">
      @for (mensaje of mensajeService.mensajes(); track mensaje.id) {
        <!--
          CLASS BINDING: [class.nombre]="condicion" agrega la clase CSS solo si
          la condición es true.
          [attr.role]: "alert" se anuncia de inmediato en lectores de pantalla,
          "status" se anuncia sin interrumpir.
        -->
        <div
          class="mensaje"
          [class.mensaje-exito]="mensaje.tipo === 'exito'"
          [class.mensaje-error]="mensaje.tipo === 'error'"
          [attr.role]="mensaje.tipo === 'error' ? 'alert' : 'status'"
        >
          <span>{{ mensaje.texto }}</span>
          <button type="button" class="cerrar" aria-label="Cerrar aviso" (click)="mensajeService.cerrar(mensaje.id)">
            ×
          </button>
        </div>
      }
    </div>
  `,
  styles: `
    /* position: fixed = queda fijo en la ventana aunque hagas scroll. */
    .contenedor-mensajes {
      position: fixed;
      right: 1rem;
      bottom: 1rem;
      left: 1rem;
      z-index: 1000;
      display: flex;
      flex-direction: column;
      align-items: flex-end;
      gap: 0.5rem;
      pointer-events: none; /* los clicks "atraviesan" el contenedor vacío */
    }

    .mensaje {
      display: flex;
      align-items: flex-start;
      gap: 0.75rem;
      max-width: 420px;
      padding: 0.75rem 0.9rem;
      border-radius: 8px;
      color: #ffffff;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.18);
      pointer-events: auto;
    }

    .mensaje-exito {
      background: #166534;
    }

    .mensaje-error {
      background: var(--color-peligro);
    }

    .cerrar {
      padding: 0 0.25rem;
      border: none;
      font-size: 1.25rem;
      line-height: 1;
      color: inherit;
      background: transparent;
      cursor: pointer;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Mensajes {
  // protected: el template necesita leer mensajes() y llamar a cerrar().
  protected readonly mensajeService = inject(MensajeService);
}
