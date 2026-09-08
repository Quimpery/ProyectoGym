using Microsoft.EntityFrameworkCore;
using FinalProgramacion4.GymRutinas.api.Models;
namespace FinalProgramacion4.GymRutinas.api.Data

{
    public class AplicationDbContext : DbContext
    {
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options):base(options)
        {
        }

        public DbSet<Rutina> Rutinas { get; set; }
        public DbSet<Ejercicio> Ejercicios { get; set; }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rutina>()
                .HasIndex(u=> u.DiaSemana)
                .IsUnique();

        }*/

    }
}
