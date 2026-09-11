using System.ComponentModel.DataAnnotations;

namespace GymApi.Dtos;

// ------------------------------------------------------------
// DTO de ENTRADA para CREAR (POST) y MODIFICAR (PUT) una rutina
// ------------------------------------------------------------
// Usamos el mismo DTO para las dos operaciones porque piden los mismos datos.
//
// Ejemplo de JSON que acepta:
// {
//   "nombre": "Fuerza 3 días",
//   "descripcion": "Rutina full body",
//   "ejercicios": [
//     { "nombre": "Sentadilla", "dia": "Lunes", "series": 4, "repeticiones": 8,
//       "peso": 80, "notas": null, "orden": 1 }
//   ]
// }
//
// Fijate que en el JSON las propiedades van en camelCase ("nombre") y en C#
// en PascalCase ("Nombre"): ASP.NET hace esa conversión automáticamente.
public class RutinaGuardarDto
{
    [Required(ErrorMessage = "El nombre de la rutina es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre de la rutina no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    // Lista de ejercicios de la rutina. ASP.NET valida también CADA ejercicio
    // de la lista con las reglas de EjercicioGuardarDto.
    public List<EjercicioGuardarDto> Ejercicios { get; set; } = new();
}
