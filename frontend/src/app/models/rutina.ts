// ------------------------------------------------------------
// MODELOS: la "forma" de los datos que van y vienen de la API
// ------------------------------------------------------------
// En C# usábamos clases (los DTOs). En TypeScript, para datos que vienen de
// una API, se usan INTERFACES: describen qué propiedades tiene un objeto y de
// qué tipo, pero NO generan código (desaparecen al compilar).
// Sirven para que el editor autocomplete y el compilador marque errores de
// tipeo, por ejemplo escribir "rutina.nombr".
//
// IMPORTANTE: los nombres van en camelCase ("cantidadEjercicios") porque así
// los manda ASP.NET en el JSON, aunque en C# se llamen "CantidadEjercicios".

// TIPO UNIÓN DE LITERALES: una variable DiaSemana solo puede tener uno de
// estos 7 textos exactos. Es el equivalente al enum DiaSemana de C#.
//   const d: DiaSemana = 'Lunez';   // <- error de compilación
export type DiaSemana = 'Lunes' | 'Martes' | 'Miercoles' | 'Jueves' | 'Viernes' | 'Sabado' | 'Domingo';

// Los 7 días en orden, para recorrerlos con @for (por ejemplo, en un <select>).
export const DIAS_SEMANA: DiaSemana[] = ['Lunes', 'Martes', 'Miercoles', 'Jueves', 'Viernes', 'Sabado', 'Domingo'];

// Record<Clave, Valor> es un objeto que tiene UNA entrada por cada clave.
// Como la clave es DiaSemana, TypeScript nos obliga a escribir los 7 días.
// La API manda "Miercoles" (sin tilde); en pantalla mostramos "Miércoles".
export const ETIQUETAS_DIA: Record<DiaSemana, string> = {
  Lunes: 'Lunes',
  Martes: 'Martes',
  Miercoles: 'Miércoles',
  Jueves: 'Jueves',
  Viernes: 'Viernes',
  Sabado: 'Sábado',
  Domingo: 'Domingo',
};

// ===================== LO QUE DEVUELVE LA API =====================

// Un ejercicio (EjercicioDto en C#).
export interface Ejercicio {
  id: number;
  nombre: string;
  dia: DiaSemana;
  series: number;
  repeticiones: number;
  // "number | null": puede ser un número o null (peso corporal).
  // En C# era decimal? ; en TypeScript no existe "?" para esto: se escribe la unión.
  peso: number | null;
  notas: string | null;
  orden: number;
}

// Una rutina en el LISTADO (RutinaResumenDto en C#).
export interface RutinaResumen {
  id: number;
  nombre: string;
  descripcion: string | null;
  // JSON no tiene tipo "fecha": la fecha llega como texto ISO
  // ("2026-09-11T16:21:02.67"). Para mostrarla linda usamos el pipe "date".
  fechaCreacion: string;
  cantidadEjercicios: number;
  dias: DiaSemana[];
}

// Una rutina con TODOS sus ejercicios (RutinaDetalleDto en C#).
export interface RutinaDetalle {
  id: number;
  nombre: string;
  descripcion: string | null;
  fechaCreacion: string;
  ejercicios: Ejercicio[];
}

// Formato de error de la API (ErrorRespuesta en C#).
export interface ErrorRespuesta {
  mensaje: string;
  errores: string[] | null;
}

// ===================== LO QUE LE MANDAMOS A LA API =====================

// Body de POST / PUT (RutinaGuardarDto en C#).
export interface RutinaGuardar {
  nombre: string;
  descripcion: string | null;
  ejercicios: EjercicioGuardar[];
}

// Cada ejercicio dentro del body (EjercicioGuardarDto en C#).
// Es igual a Ejercicio pero SIN el id: lo genera la base de datos.
// Omit<Tipo, 'campo'> crea un tipo nuevo copiando otro menos esos campos.
export type EjercicioGuardar = Omit<Ejercicio, 'id'>;
