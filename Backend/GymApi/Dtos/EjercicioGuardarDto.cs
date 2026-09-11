using System.ComponentModel.DataAnnotations;
using GymApi.Models;

namespace GymApi.Dtos;

// ------------------------------------------------------------
// DTO de ENTRADA: un ejercicio tal como llega en el JSON del front
// ------------------------------------------------------------
// DTO = "Data Transfer Object". Es una clase que define EXACTAMENTE qué datos
// acepta (o devuelve) la API. ¿Por qué no recibir directamente la entidad
// Ejercicio?
//   - La entidad tiene cosas que el cliente NO debe mandar (Id, RutinaId).
//   - Acá ponemos las VALIDACIONES de entrada sin ensuciar la entidad.
//
// Los atributos [Required], [Range], [MaxLength] se validan SOLOS: como el
// controller tiene [ApiController], si alguno falla ASP.NET responde 400
// antes de ejecutar nuestro código (el formato de ese 400 está en Program.cs).
public class EjercicioGuardarDto
{
    [Required(ErrorMessage = "El nombre del ejercicio es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre del ejercicio no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    // Es "DiaSemana?" (nullable) a propósito: si fuera no-nullable y el JSON no
    // trae el día, quedaría en 0 y [Required] no se daría cuenta.
    // [EnumDataType] rechaza valores que no son un día válido.
    [Required(ErrorMessage = "El día de la semana es obligatorio.")]
    [EnumDataType(typeof(DiaSemana), ErrorMessage = "El día de la semana no es válido.")]
    public DiaSemana? Dia { get; set; }

    // [Range(min, max)] -> tiene que ser un número positivo (y razonable).
    // Si no viene en el JSON queda en 0, y 0 no pasa el rango: también da error.
    [Range(1, 100, ErrorMessage = "Las series deben ser un número entre 1 y 100.")]
    public int Series { get; set; }

    [Range(1, 1000, ErrorMessage = "Las repeticiones deben ser un número entre 1 y 1000.")]
    public int Repeticiones { get; set; }

    // Opcional: si viene null, [Range] no lo valida (null es válido).
    // Si viene un número, tiene que ser positivo.
    [Range(0.01, 9999.99, ErrorMessage = "El peso debe ser un número positivo (máximo 9999.99 kg).")]
    public decimal? Peso { get; set; }

    [MaxLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
    public string? Notas { get; set; }

    [Range(1, 1000, ErrorMessage = "El orden debe ser un número mayor o igual a 1.")]
    public int Orden { get; set; }
}
