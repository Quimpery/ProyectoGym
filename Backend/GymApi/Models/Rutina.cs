using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Models;

// ------------------------------------------------------------
// ENTIDAD Rutina  ->  tabla "Rutinas" en SQL Server
// ------------------------------------------------------------
// Una "entidad" es una clase que Entity Framework (el ORM) convierte en una
// tabla. Cada propiedad pública es una columna. No escribimos SQL a mano:
// EF lee esta clase y genera el CREATE TABLE en la migración.
//
// [Index(..., IsUnique = true)] crea un ÍNDICE ÚNICO sobre Nombre: la base de
// datos misma rechaza dos rutinas con el mismo nombre. Igual lo validamos en
// el service para devolver un mensaje claro, pero el índice es la última
// línea de defensa (y además acelera las búsquedas por nombre).
[Index(nameof(Nombre), IsUnique = true)]
public class Rutina
{
    // Por convención, una propiedad llamada "Id" es la clave primaria.
    // EF la configura como IDENTITY: SQL Server genera el número solo.
    public int Id { get; set; }

    // [Required] -> columna NOT NULL.
    // [MaxLength(100)] -> nvarchar(100) en vez de nvarchar(max).
    // "= string.Empty" evita el warning de C# sobre strings no inicializados.
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    // El "?" dice que puede ser null -> columna NULL (la descripción es opcional).
    [MaxLength(500)]
    public string? Descripcion { get; set; }

    // Se completa en el service con la fecha/hora actual al crear la rutina.
    public DateTime FechaCreacion { get; set; }

    // PROPIEDAD DE NAVEGACIÓN (relación 1 a N):
    // una Rutina tiene MUCHOS Ejercicios. Esto NO es una columna; le dice a EF
    // que existe la relación. La columna real (la FK) está en Ejercicio.RutinaId.
    public List<Ejercicio> Ejercicios { get; set; } = new();
}
