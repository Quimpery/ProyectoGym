import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

// ------------------------------------------------------------
// COMPONENTE Buscador: barra de búsqueda REUTILIZABLE
// ------------------------------------------------------------
// Muestra cómo se COMUNICAN un componente PADRE y un componente HIJO:
//
//   Padre (ListaRutinas)                         Hijo (Buscador)
//   <app-buscador etiqueta="..."          --->   etiqueta = input(...)    DATOS BAJAN
//                 (buscar)="alBuscar($event)" <--- buscar = output<string>()  EVENTOS SUBEN
//
// El buscador NO sabe nada de rutinas ni de la API: solo avisa "el usuario
// escribió esto". Qué hacer con ese texto lo decide el padre. Por eso se
// podría reutilizar para buscar cualquier otra cosa.
@Component({
  selector: 'app-buscador',
  template: `
    <!-- [for] y [id] conectan el label con el input (accesibilidad). -->
    <label class="etiqueta" [for]="idCampo()">{{ etiqueta() }}</label>
    <!--
      (input) se dispara en CADA tecla. $event es el evento del navegador.
      type="search" agrega una "x" para borrar (que también dispara input).
    -->
    <input
      class="campo"
      type="search"
      autocomplete="off"
      [id]="idCampo()"
      [placeholder]="placeholder()"
      (input)="alEscribir($event)"
    />
  `,
  styles: `
    /* :host = el propio elemento <app-buscador>. Por defecto es "inline". */
    :host {
      display: block;
      max-width: 420px;
      margin-bottom: 1.25rem;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Buscador {
  // INPUTS: datos que recibe del padre. input('valor') define el valor por
  // defecto si el padre no lo pasa. Se leen como signals: etiqueta().
  readonly etiqueta = input('Buscar');
  readonly placeholder = input('');
  readonly idCampo = input('buscador');

  // OUTPUT: evento que el hijo emite hacia el padre, con un string adentro.
  readonly buscar = output<string>();

  protected alEscribir(evento: Event): void {
    // evento.target es el elemento que disparó el evento. TypeScript solo sabe
    // que es un "EventTarget", así que le decimos que es un <input> ("as").
    const texto = (evento.target as HTMLInputElement).value;
    // emit() dispara el evento: el padre recibe este texto en $event.
    this.buscar.emit(texto);
  }
}
