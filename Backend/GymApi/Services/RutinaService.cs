using GymApi.Data;
using GymApi.Dtos;
using GymApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Services;

// ------------------------------------------------------------
// RutinaService: la LÓGICA de negocio y el acceso a datos
// ------------------------------------------------------------
// Separación de responsabilidades:
//   - Controller -> recibe HTTP y decide el código de respuesta (200, 404...).
//   - Service    -> sabe CÓMO consultar/guardar y convertir entidades <-> DTOs.
//   - DbContext  -> habla con SQL Server.
// El controller nunca toca el DbContext directamente.
//
// Todos los métodos son "async": mientras SQL Server responde, el hilo queda
// libre para atender otras requests. Por eso devuelven Task<T> y usamos
// "await" + los métodos ...Async() de Entity Framework.
public class RutinaService
{
    private readonly GymDbContext _context;

    // INYECCIÓN DE DEPENDENCIAS: no hacemos "new GymDbContext()". ASP.NET ve
    // que el constructor pide un GymDbContext y nos pasa el que registramos
    // en Program.cs con AddDbContext.
    public RutinaService(GymDbContext context)
    {
        _context = context;
    }

    // Lista todas las rutinas, o solo las que contienen "nombre" si viene.
    // Se usa tanto para GET /api/rutinas como para GET /api/rutinas/buscar.
    public async Task<List<RutinaResumenDto>> ListarAsync(string? nombre = null)
    {
        // IQueryable = una consulta que TODAVÍA NO SE EJECUTÓ. Podemos ir
        // agregándole condiciones y EF arma un único SQL al final.
        // - AsNoTracking(): solo vamos a leer, no hace falta que EF siga los
        //   cambios de estos objetos (es más rápido).
        // - Include(): trae también los ejercicios de cada rutina (JOIN).
        IQueryable<Rutina> consulta = _context.Rutinas
            .AsNoTracking()
            .Include(r => r.Ejercicios);

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            string texto = nombre.Trim();
            // Contains se traduce a un LIKE '%texto%' en SQL (búsqueda parcial).
            // No hace falta pasar a minúsculas: la collation de la base
            // (Modern_Spanish_CI_AS, "CI" = Case Insensitive) ya compara
            // sin distinguir mayúsculas.
            consulta = consulta.Where(r => r.Nombre.Contains(texto));
        }

        // ToListAsync() es el momento en que se EJECUTA el SQL.
        List<Rutina> rutinas = await consulta
            .OrderBy(r => r.Nombre)
            .ToListAsync();

        // Convertimos cada entidad al DTO de resumen (esto ya es en memoria).
        return rutinas.Select(r => ConvertirAResumen(r)).ToList();
    }

    // Devuelve el detalle de una rutina, o null si no existe.
    public async Task<RutinaDetalleDto?> ObtenerAsync(int id)
    {
        Rutina? rutina = await _context.Rutinas
            .AsNoTracking()
            .Include(r => r.Ejercicios)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rutina == null)
        {
            return null;
        }

        return ConvertirADetalle(rutina);
    }

    // ¿Existe una rutina con ese id?
    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Rutinas.AnyAsync(r => r.Id == id);
    }

    // ¿Ya hay OTRA rutina con ese nombre?
    // "idExcluir" sirve al modificar: si edito la rutina 5 y dejo el mismo
    // nombre, no tiene que contar como repetido consigo misma.
    public async Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null)
    {
        string nombreLimpio = nombre.Trim();
        // "==" en SQL Server también respeta la collation CI:
        // "Fuerza" y "FUERZA" se consideran iguales.
        return await _context.Rutinas.AnyAsync(r =>
            r.Nombre == nombreLimpio && (idExcluir == null || r.Id != idExcluir));
    }

    // Crea la rutina con todos sus ejercicios en un solo SaveChanges.
    public async Task<RutinaDetalleDto> CrearAsync(RutinaGuardarDto dto)
    {
        var rutina = new Rutina
        {
            Nombre = dto.Nombre.Trim(),
            Descripcion = TextoONull(dto.Descripcion),
            // DateTime.Now = hora local de la PC donde corre la API.
            FechaCreacion = DateTime.Now,
            Ejercicios = dto.Ejercicios.Select(e => ConvertirAEntidad(e)).ToList()
        };

        // Add() solo marca la rutina (y sus ejercicios) como "a insertar".
        _context.Rutinas.Add(rutina);
        // SaveChangesAsync() ejecuta los INSERT. Después de esto, EF completa
        // los Id generados por SQL Server en rutina.Id y en cada ejercicio.Id.
        await _context.SaveChangesAsync();

        // Devolvemos la rutina tal como quedó GUARDADA, leyéndola de nuevo.
        // Así la respuesta del POST tiene exactamente el mismo formato que la
        // del GET (por ejemplo, la fecha sin zona horaria, como la guarda SQL).
        // El "!" es seguro: la acabamos de insertar, así que existe.
        return (await ObtenerAsync(rutina.Id))!;
    }

    // Modifica nombre, descripción y REEMPLAZA la lista de ejercicios.
    // Devuelve null si la rutina no existe.
    public async Task<RutinaDetalleDto?> ActualizarAsync(int id, RutinaGuardarDto dto)
    {
        // Sin AsNoTracking: queremos que EF detecte los cambios que hagamos.
        Rutina? rutina = await _context.Rutinas
            .Include(r => r.Ejercicios)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rutina == null)
        {
            return null;
        }

        // EF "sigue" este objeto: con solo cambiar las propiedades, al hacer
        // SaveChanges genera el UPDATE correspondiente.
        rutina.Nombre = dto.Nombre.Trim();
        rutina.Descripcion = TextoONull(dto.Descripcion);

        // Estrategia simple para los ejercicios: borrar todos los anteriores y
        // cargar los que vinieron en el JSON. Así con un único formulario se
        // puede agregar, modificar, eliminar y reordenar ejercicios.
        // (Contra: los ejercicios reciben Id nuevos en cada edición.)
        _context.Ejercicios.RemoveRange(rutina.Ejercicios);   // -> DELETE
        rutina.Ejercicios.Clear();
        rutina.Ejercicios.AddRange(dto.Ejercicios.Select(e => ConvertirAEntidad(e)));  // -> INSERT

        // Todo (UPDATE + DELETE + INSERT) se ejecuta junto, en una transacción:
        // o se guarda todo, o no se guarda nada.
        await _context.SaveChangesAsync();

        return ConvertirADetalle(rutina);
    }

    // Elimina la rutina. Devuelve false si no existía.
    public async Task<bool> EliminarAsync(int id)
    {
        // FindAsync busca por clave primaria.
        Rutina? rutina = await _context.Rutinas.FindAsync(id);
        if (rutina == null)
        {
            return false;
        }

        // No hace falta borrar los ejercicios a mano: la FK tiene
        // ON DELETE CASCADE y SQL Server los borra solo.
        _context.Rutinas.Remove(rutina);
        await _context.SaveChangesAsync();
        return true;
    }

    // ================= CONVERSIONES (métodos privados) =================
    // "static" porque no usan _context: solo transforman un objeto en otro.

    private static Ejercicio ConvertirAEntidad(EjercicioGuardarDto dto)
    {
        return new Ejercicio
        {
            Nombre = dto.Nombre.Trim(),
            // El "!" le dice al compilador "sé que no es null": ya lo garantizó
            // la validación [Required] antes de llegar acá.
            Dia = dto.Dia!.Value,
            Series = dto.Series,
            Repeticiones = dto.Repeticiones,
            Peso = dto.Peso,
            Notas = TextoONull(dto.Notas),
            Orden = dto.Orden
        };
    }

    private static RutinaResumenDto ConvertirAResumen(Rutina rutina)
    {
        return new RutinaResumenDto
        {
            Id = rutina.Id,
            Nombre = rutina.Nombre,
            Descripcion = rutina.Descripcion,
            FechaCreacion = rutina.FechaCreacion,
            CantidadEjercicios = rutina.Ejercicios.Count,
            // Días sin repetir (Distinct) y ordenados por el número del enum
            // (Lunes = 1 ... Domingo = 7), no alfabéticamente.
            Dias = rutina.Ejercicios
                .Select(e => e.Dia)
                .Distinct()
                .Order()
                .ToList()
        };
    }

    private static RutinaDetalleDto ConvertirADetalle(Rutina rutina)
    {
        return new RutinaDetalleDto
        {
            Id = rutina.Id,
            Nombre = rutina.Nombre,
            Descripcion = rutina.Descripcion,
            FechaCreacion = rutina.FechaCreacion,
            Ejercicios = rutina.Ejercicios
                .OrderBy(e => e.Dia)        // primero por día de la semana
                .ThenBy(e => e.Orden)       // y dentro del día, por orden
                .Select(e => new EjercicioDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Dia = e.Dia,
                    Series = e.Series,
                    Repeticiones = e.Repeticiones,
                    Peso = e.Peso,
                    Notas = e.Notas,
                    Orden = e.Orden
                })
                .ToList()
        };
    }

    // Convierte "" o "   " en null, y quita espacios de los costados.
    // Así no guardamos descripciones/notas vacías en la base.
    private static string? TextoONull(string? texto)
    {
        return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }
}
