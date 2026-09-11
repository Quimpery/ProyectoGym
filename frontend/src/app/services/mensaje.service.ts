import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';

import { environment } from '../../environments/environment';
import { ErrorRespuesta } from '../models/rutina';

// Tipos de mensaje que puede mostrar la app.
export type TipoMensaje = 'exito' | 'error';

export interface Mensaje {
  id: number;
  texto: string;
  tipo: TipoMensaje;
}

// ------------------------------------------------------------
// MensajeService: avisos de éxito/error ("toasts") para toda la app
// ------------------------------------------------------------
// Este servicio muestra que un servicio no sirve solo para llamar a la API:
// como es un SINGLETON (una sola instancia), es el lugar ideal para ESTADO
// COMPARTIDO entre pantallas.
//   - Cualquier componente llama a mensajeService.exito('...').
//   - El componente <app-mensajes> (en app.html) lee mensajes() y los dibuja.
// Como mensajes es un signal, la pantalla se actualiza sola.
@Injectable({ providedIn: 'root' })
export class MensajeService {
  // Patrón muy común: el signal "escribible" es PRIVADO...
  private readonly listaMensajes = signal<Mensaje[]>([]);
  // ...y hacia afuera se expone una versión de SOLO LECTURA.
  // Así nadie puede hacer mensajeService.mensajes.set(...) desde otro lado:
  // solo este servicio decide cómo cambia la lista.
  readonly mensajes = this.listaMensajes.asReadonly();

  private proximoId = 1;

  exito(texto: string): void {
    this.mostrar(texto, 'exito', 4000);
  }

  error(texto: string): void {
    this.mostrar(texto, 'error', 7000);
  }

  // Muestra el error que devolvió una petición HTTP.
  errorHttp(error: unknown): void {
    this.error(this.textoDeError(error));
  }

  cerrar(id: number): void {
    // update() recibe el valor ACTUAL y devuelve el NUEVO.
    // filter() crea un array nuevo sin el mensaje: nunca modificamos el array
    // original, porque el signal solo avisa si cambia la referencia.
    this.listaMensajes.update((lista) => lista.filter((mensaje) => mensaje.id !== id));
  }

  // Convierte cualquier error de HttpClient en un texto para el usuario.
  // "unknown" = no sabemos de qué tipo es; hay que comprobarlo antes de usarlo.
  textoDeError(error: unknown): string {
    // instanceof comprueba si es un error HTTP de Angular.
    if (error instanceof HttpErrorResponse) {
      // status 0 = ni siquiera hubo respuesta (API apagada, CORS, sin red).
      if (error.status === 0) {
        return `No se pudo conectar con la API (${environment.apiUrl}). ¿Está corriendo el backend?`;
      }

      // error.error es el BODY de la respuesta: nuestro { mensaje, errores }.
      const cuerpo = error.error as ErrorRespuesta | null;
      // "?." (optional chaining): si cuerpo es null no explota, devuelve undefined.
      if (cuerpo?.mensaje) {
        // Si hay detalles de validación, los agregamos al mensaje.
        return cuerpo.errores?.length ? `${cuerpo.mensaje} ${cuerpo.errores.join(' ')}` : cuerpo.mensaje;
      }

      if (error.status === 404) {
        return 'No se encontró lo que buscabas.';
      }
    }
    return 'Ocurrió un error inesperado. Intentá de nuevo.';
  }

  private mostrar(texto: string, tipo: TipoMensaje, duracionMs: number): void {
    const id = this.proximoId++;
    // [...lista, nuevo] = array NUEVO con todo lo anterior más el mensaje nuevo.
    this.listaMensajes.update((lista) => [...lista, { id, texto, tipo }]);
    // setTimeout ejecuta la función una vez pasado el tiempo: el aviso se va solo.
    setTimeout(() => this.cerrar(id), duracionMs);
  }
}
