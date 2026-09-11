using GymApi.Models;

namespace GymApi.Dtos;

// ------------------------------------------------------------
// DTO de SALIDA: un ejercicio tal como lo devuelve la API
// ------------------------------------------------------------
// Devolvemos DTOs y no entidades porque:
//   - La entidad Ejercicio tiene la navegación "Rutina", y la Rutina tiene la
//     lista de Ejercicios... serializarla generaría un ciclo infinito.
//   - Controlamos qué campos ve el cliente.
public class EjercicioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DiaSemana Dia { get; set; }       // viaja como texto: "Lunes"
    public int Series { get; set; }
    public int Repeticiones { get; set; }
    public decimal? Peso { get; set; }
    public string? Notas { get; set; }
    public int Orden { get; set; }
}
