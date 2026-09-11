using GymApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Data;

// ------------------------------------------------------------
// GymDbContext: el "puente" entre C# y la base de datos
// ------------------------------------------------------------
// El DbContext representa una sesión con la base. A través de él:
//   - consultamos:  context.Rutinas.Where(...).ToListAsync()
//   - agregamos:    context.Rutinas.Add(rutina)
//   - guardamos:    context.SaveChangesAsync()   <- recién acá se ejecuta el SQL
//
// Hereda de DbContext (clase de Entity Framework Core).
public class GymDbContext : DbContext
{
    // El constructor recibe las "opciones" (qué motor usar y el connection
    // string). No las armamos acá: las configura Program.cs con AddDbContext
    // y llegan por inyección de dependencias.
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {
    }

    // Cada DbSet<T> representa una TABLA. Lo usamos como si fuera una lista
    // sobre la que se pueden hacer consultas LINQ que EF traduce a SQL.
    // (La forma "=> Set<T>()" evita un warning de C# por propiedades sin inicializar.)
    public DbSet<Rutina> Rutinas => Set<Rutina>();
    public DbSet<Ejercicio> Ejercicios => Set<Ejercicio>();

    // OnModelCreating permite configurar detalles que no se pueden (o no
    // conviene) expresar con atributos en las clases. Se llama "Fluent API".
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // RELACIÓN Rutina (1) ---- (N) Ejercicio
        // EF ya la deduce sola por los nombres, pero la escribimos explícita
        // para que se lea clara y para dejar fijo el borrado en CASCADA:
        // al borrar una rutina, SQL Server borra automáticamente sus ejercicios
        // (ON DELETE CASCADE). Así nunca quedan ejercicios "huérfanos".
        modelBuilder.Entity<Ejercicio>()
            .HasOne(e => e.Rutina)            // un ejercicio tiene una rutina
            .WithMany(r => r.Ejercicios)      // una rutina tiene muchos ejercicios
            .HasForeignKey(e => e.RutinaId)   // la FK es RutinaId
            .OnDelete(DeleteBehavior.Cascade);

        // Guardar el enum como TEXTO ("Lunes") en vez de número (1).
        // Ventaja: si mirás la tabla con SQL Server Management Studio se
        // entiende sin saber qué número es cada día.
        modelBuilder.Entity<Ejercicio>()
            .Property(e => e.Dia)
            .HasConversion<string>()
            .HasMaxLength(10);
    }
}
