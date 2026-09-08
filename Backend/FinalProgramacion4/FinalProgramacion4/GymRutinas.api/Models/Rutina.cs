using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
namespace FinalProgramacion4.GymRutinas.api.Models
{
    [Table("Rutinas")]
    public class Rutina
    {
        [Key]
        public int Id_rutina { get; set; }
        [Required]
        [MaxLength(30)]
        public string Nombre { get; set; }
       
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Required]
        public string DiaSemana { get; set; }
        [Required]
        public int Id_ejercicio { get; set; }

        public ICollection<Ejercicio> EjerciciosPorDia { get; set; }=new List<Ejercicio>();
    }
}
