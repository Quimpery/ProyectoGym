using GymApi.Models;

namespace GymApi.Dtos;

// ------------------------------------------------------------
// DTO de SALIDA para el LISTADO de rutinas (información resumida)
// ------------------------------------------------------------
// En el listado no hace falta mandar todos los ejercicios: alcanza con
// cuántos son y qué días se entrena. Así la respuesta es más liviana.
public class RutinaResumenDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int CantidadEjercicios { get; set; }

    // Días que tienen al menos un ejercicio, ordenados de Lunes a Domingo.
    // Ej: ["Lunes", "Miercoles", "Viernes"]
    public List<DiaSemana> Dias { get; set; } = new();
}
