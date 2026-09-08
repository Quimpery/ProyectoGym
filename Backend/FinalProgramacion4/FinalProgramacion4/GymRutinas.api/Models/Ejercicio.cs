using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
namespace FinalProgramacion4.GymRutinas.api.Models
{
    [Table("Ejercicios")]
    public class Ejercicio
    {
        [Key]
        public int Id_ejercicio { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string DiaSemana { get; set; }

        [Required]
        public int Series { get; set; }


        [Required]
        public int Repeticiones { get; set; }

     
        public double Peso { get; set; }

        [Required]
        public int Orden { get; set; }

        [Required]
        public int Id_rutina { get; set; }

        [ForeignKey("Id_rutina")]
        public Rutina Rutina { get; set; }




    }
}
