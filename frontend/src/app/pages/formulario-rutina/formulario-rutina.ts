import { ChangeDetectionStrategy, Component, OnInit, computed, inject, input, signal } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormControl,
  FormGroup,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { DIAS_SEMANA, DiaSemana, ETIQUETAS_DIA, Ejercicio, EjercicioGuardar, RutinaGuardar } from '../../models/rutina';
import { MensajeService } from '../../services/mensaje.service';
import { RutinaService } from '../../services/rutina.service';

// ------------------------------------------------------------
// TIPO del formulario de UN ejercicio
// ------------------------------------------------------------
// Un FormGroup agrupa varios FormControl (uno por campo). Escribir el tipo
// explícito ayuda a que TypeScript sepa qué hay adentro:
// grupo.controls.series es un FormControl<number | null>.
// "number | null" porque si el usuario borra un <input type="number"> el
// valor pasa a ser null.
type EjercicioForm = FormGroup<{
  nombre: FormControl<string>;
  dia: FormControl<DiaSemana>;
  series: FormControl<number | null>;
  repeticiones: FormControl<number | null>;
  peso: FormControl<number | null>;
  notas: FormControl<string>;
}>;

// ------------------------------------------------------------
// VALIDADOR PROPIO
// ------------------------------------------------------------
// Un validador es una función que recibe el control y devuelve:
//   - null            -> el valor es VÁLIDO
//   - { clave: ... }  -> el valor es INVÁLIDO (la clave identifica el error)
// Validators.required acepta "   " (solo espacios) como válido; este no.
// Devolvemos la clave "required" para tratarlo igual que un campo vacío.
function textoObligatorio(control: AbstractControl): ValidationErrors | null {
  const valor = control.value as string;
  return valor.trim().length === 0 ? { required: true } : null;
}

// Solo números enteros (sin decimales). Validators.pattern convierte el
// número a texto y lo compara con la expresión regular: ^\d+$ = solo dígitos.
const soloEnteros = Validators.pattern(/^\d+$/);

// ------------------------------------------------------------
// PÁGINA FormularioRutina: ALTA y EDICIÓN de una rutina
// ------------------------------------------------------------
// El mismo componente sirve para las dos rutas:
//   /rutinas/nueva        -> crear   (no hay :id)
//   /rutinas/:id/editar   -> editar  (hay :id)
@Component({
  selector: 'app-formulario-rutina',
  // ReactiveFormsModule trae las directivas [formGroup], formControlName,
  // formArrayName, [formGroupName]... Sin este import, el form no funciona.
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './formulario-rutina.html',
  styleUrl: './formulario-rutina.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormularioRutina implements OnInit {
  // NonNullableFormBuilder: "fábrica" de controles. "NonNullable" significa
  // que al hacer reset() cada campo vuelve a su valor inicial y no a null.
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly rutinaService = inject(RutinaService);
  private readonly mensajeService = inject(MensajeService);
  private readonly router = inject(Router);

  // Viene de la URL gracias a withComponentInputBinding(). En /rutinas/nueva
  // no hay :id, así que queda undefined. Por eso es input() y no input.required().
  readonly id = input<string>();

  // null = estamos creando; un número = estamos editando esa rutina.
  protected readonly idRutina = computed(() => {
    const id = this.id();
    return id === undefined ? null : Number(id);
  });
  protected readonly esEdicion = computed(() => this.idRutina() !== null);

  // ----- Estado de la pantalla -----
  protected readonly cargando = signal(false);
  protected readonly errorCarga = signal<string | null>(null);
  protected readonly guardando = signal(false);
  protected readonly errorGuardado = signal<string | null>(null);

  protected readonly dias = DIAS_SEMANA;
  protected readonly etiquetasDia = ETIQUETAS_DIA;

  // ------------------------------------------------------------------
  // EL FORMULARIO REACTIVO
  // ------------------------------------------------------------------
  // El formulario se define ACÁ, en TypeScript, con sus valores iniciales y
  // sus validaciones. El HTML solo se "conecta" a estos controles.
  //   fb.control(valorInicial, [validadores])
  //   fb.array([...])  -> una LISTA de controles (acá, de ejercicios)
  protected readonly formulario = this.fb.group({
    nombre: this.fb.control('', [textoObligatorio, Validators.maxLength(100)]),
    descripcion: this.fb.control('', Validators.maxLength(500)),
    ejercicios: this.fb.array<EjercicioForm>([]),
  });

  // GETTER: una "propiedad calculada". Permite escribir "ejercicios" en el
  // template en vez de "formulario.controls.ejercicios".
  protected get ejercicios(): FormArray<EjercicioForm> {
    return this.formulario.controls.ejercicios;
  }

  ngOnInit(): void {
    const id = this.idRutina();
    if (id === null) {
      // Rutina nueva: arrancamos con un ejercicio vacío para completar.
      this.agregarEjercicio();
    } else {
      // Edición: traemos la rutina de la API y llenamos el formulario.
      this.cargarRutina(id);
    }
  }

  protected reintentarCarga(): void {
    const id = this.idRutina();
    if (id !== null) {
      this.cargarRutina(id);
    }
  }

  // ----- Manejo de la lista de ejercicios (FormArray) -----

  protected agregarEjercicio(): void {
    // Para ahorrar clicks, el nuevo ejercicio usa el mismo día que el último.
    const cantidad = this.ejercicios.length;
    const dia = cantidad > 0 ? this.ejercicios.at(cantidad - 1).controls.dia.value : 'Lunes';
    // push() agrega un grupo al final. Angular dibuja la nueva fila solo.
    this.ejercicios.push(this.crearGrupoEjercicio({ dia }));
  }

  protected quitarEjercicio(indice: number): void {
    this.ejercicios.removeAt(indice);
  }

  // direccion: -1 = subir, 1 = bajar.
  protected moverEjercicio(indice: number, direccion: -1 | 1): void {
    const destino = indice + direccion;
    if (destino < 0 || destino >= this.ejercicios.length) {
      return;
    }
    // Sacamos el grupo de su posición y lo insertamos en la nueva.
    const grupo = this.ejercicios.at(indice);
    this.ejercicios.removeAt(indice);
    this.ejercicios.insert(destino, grupo);
  }

  // ¿Hay que mostrar el error de este campo?
  // Solo si es inválido Y el usuario ya pasó por él ("touched" = hizo foco y
  // salió). Así no se ve todo en rojo apenas se abre el formulario.
  protected tieneError(control: AbstractControl): boolean {
    return control.invalid && control.touched;
  }

  // ----- Guardar -----

  // Se ejecuta con (ngSubmit): al tocar el botón type="submit" o apretar Enter.
  protected guardar(): void {
    this.errorGuardado.set(null);

    if (this.formulario.invalid) {
      // Marca TODOS los campos como tocados -> aparecen todos los errores.
      this.formulario.markAllAsTouched();
      this.errorGuardado.set('Hay campos con errores. Revisá los mensajes en rojo antes de guardar.');
      return;
    }

    const rutina = this.armarRutina();
    const id = this.idRutina();

    // Misma lógica para crear o editar: solo cambia qué método llamamos.
    const peticion = id === null ? this.rutinaService.crear(rutina) : this.rutinaService.actualizar(id, rutina);

    this.guardando.set(true);
    peticion.subscribe({
      next: (guardada) => {
        this.mensajeService.exito(
          id === null ? `Se creó la rutina "${guardada.nombre}".` : `Se guardaron los cambios de "${guardada.nombre}".`,
        );
        // Vamos al detalle para ver cómo quedó.
        this.router.navigate(['/rutinas', guardada.id]);
      },
      error: (err: unknown) => {
        this.guardando.set(false);
        // Por ejemplo, 409: "Ya existe una rutina llamada ...".
        this.errorGuardado.set(this.mensajeService.textoDeError(err));
      },
    });
  }

  // ----- Métodos privados -----

  private cargarRutina(id: number): void {
    this.cargando.set(true);
    this.errorCarga.set(null);

    this.rutinaService.obtener(id).subscribe({
      next: (rutina) => {
        // patchValue: carga valores en los controles que nombramos.
        this.formulario.patchValue({
          nombre: rutina.nombre,
          // "??" = si descripcion es null, usá '' (el control es de tipo string).
          descripcion: rutina.descripcion ?? '',
        });
        // Vaciamos la lista y agregamos un grupo por cada ejercicio.
        // La API ya los manda ordenados por día y por orden.
        this.ejercicios.clear();
        for (const ejercicio of rutina.ejercicios) {
          this.ejercicios.push(this.crearGrupoEjercicio(ejercicio));
        }
        this.cargando.set(false);
      },
      error: (err: unknown) => {
        this.errorCarga.set(this.mensajeService.textoDeError(err));
        this.cargando.set(false);
      },
    });
  }

  // Crea el FormGroup de UN ejercicio, con valores iniciales opcionales.
  // Partial<Ejercicio> = un Ejercicio donde TODOS los campos son opcionales.
  private crearGrupoEjercicio(datos: Partial<Ejercicio> = {}): EjercicioForm {
    return this.fb.group({
      nombre: this.fb.control(datos.nombre ?? '', [textoObligatorio, Validators.maxLength(100)]),
      dia: this.fb.control<DiaSemana>(datos.dia ?? 'Lunes', Validators.required),
      series: this.fb.control<number | null>(datos.series ?? 3, [
        Validators.required,
        Validators.min(1),
        Validators.max(100),
        soloEnteros,
      ]),
      repeticiones: this.fb.control<number | null>(datos.repeticiones ?? 10, [
        Validators.required,
        Validators.min(1),
        Validators.max(1000),
        soloEnteros,
      ]),
      // Sin Validators.required: el peso es opcional. min/max no validan null.
      peso: this.fb.control<number | null>(datos.peso ?? null, [Validators.min(0.01), Validators.max(9999.99)]),
      notas: this.fb.control(datos.notas ?? '', Validators.maxLength(500)),
    });
  }

  // Convierte los valores del formulario al JSON que espera la API.
  private armarRutina(): RutinaGuardar {
    // getRawValue() devuelve un objeto con TODOS los valores del formulario.
    const valores = this.formulario.getRawValue();

    // ORDEN: es la posición del ejercicio entre los de SU MISMO DÍA.
    // Llevamos un contador por día: el primer "Lunes" es 1, el segundo 2...
    const contadorPorDia: Partial<Record<DiaSemana, number>> = {};

    const ejercicios: EjercicioGuardar[] = valores.ejercicios.map((ejercicio) => {
      const orden = (contadorPorDia[ejercicio.dia] ?? 0) + 1;
      contadorPorDia[ejercicio.dia] = orden;

      return {
        nombre: ejercicio.nombre.trim(),
        dia: ejercicio.dia,
        // Ya pasaron las validaciones, así que no son null. "?? 0" es solo
        // para que TypeScript acepte el tipo number.
        series: ejercicio.series ?? 0,
        repeticiones: ejercicio.repeticiones ?? 0,
        peso: ejercicio.peso,
        // "|| null": si el texto quedó vacío, mandamos null.
        notas: ejercicio.notas.trim() || null,
        orden,
      };
    });

    return {
      nombre: valores.nombre.trim(),
      descripcion: valores.descripcion.trim() || null,
      ejercicios,
    };
  }
}
