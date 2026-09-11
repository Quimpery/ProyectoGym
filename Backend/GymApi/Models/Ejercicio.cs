using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Models;

// ------------------------------------------------------------
// ENTIDAD Ejercicio  ->  tabla "Ejercicios" en SQL Server
// ------------------------------------------------------------
// Cada ejercicio pertenece a UNA rutina (el lado "N" de la relación 1 a N).
public class Ejercicio
{
    // Clave primaria autonumérica (IDENTITY).
    public int Id { get; set; }

    // CLAVE FORÁNEA (FK): guarda el Id de la rutina a la que pertenece.
    // EF la reconoce por el nombre: "Rutina" + "Id". Como es int (no int?),
    // es obligatoria: no puede existir un ejercicio sin rutina.
    // EF además crea un índice sobre esta columna automáticamente.
    public int RutinaId { get; set; }

    // Navegación hacia la rutina "padre". Permite escribir ejercicio.Rutina.Nombre.
    // Es nullable porque no siempre la cargamos desde la base.
    public Rutina? Rutina { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    // Día en que se hace el ejercicio. En la base se guarda como TEXTO
    // ("Lunes") y no como número: lo configuramos en GymDbContext.
    public DiaSemana Dia { get; set; }

    // Las validaciones de "número positivo" NO van acá sino en los DTOs
    // (lo que recibe la API). La entidad solo describe la tabla.
    public int Series { get; set; }

    public int Repeticiones { get; set; }

    // decimal? -> opcional (null = ejercicio de peso corporal, ej. dominadas).
    // [Precision(6, 2)] -> decimal(6,2) en SQL: hasta 9999.99 kg.
    // Usamos decimal y no double porque decimal guarda exacto (72.5 es 72.5).
    [Precision(6, 2)]
    public decimal? Peso { get; set; }

    [MaxLength(500)]
    public string? Notas { get; set; }

    // Posición del ejercicio dentro de su día (1, 2, 3...).
    public int Orden { get; set; }
}
