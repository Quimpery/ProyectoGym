using GymApi.Dtos;
using GymApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace GymApi.Controllers;

// ------------------------------------------------------------
// RutinasController: los ENDPOINTS HTTP de /api/rutinas
// ------------------------------------------------------------
// [ApiController] activa comportamientos pensados para APIs:
//   - Valida los DTOs automáticamente y responde 400 si algo falla.
//   - Lee el body JSON sin tener que poner [FromBody].
// [Route("api/rutinas")] es la URL base de todos los métodos de esta clase.
//
// Hereda de ControllerBase (no de Controller) porque una API no devuelve
// vistas HTML, solo datos. ControllerBase nos da Ok(), NotFound(), etc.
[ApiController]
[Route("api/rutinas")]
public class RutinasController : ControllerBase
{
    private readonly RutinaService _rutinaService;

    // El service llega por inyección de dependencias (registrado en Program.cs).
    public RutinasController(RutinaService rutinaService)
    {
        _rutinaService = rutinaService;
    }

    // GET /api/rutinas
    // ActionResult<T> significa "devuelvo un T o algún código HTTP".
    [HttpGet]
    public async Task<ActionResult<List<RutinaResumenDto>>> Listar()
    {
        List<RutinaResumenDto> rutinas = await _rutinaService.ListarAsync();
        return Ok(rutinas);   // 200 OK + la lista en JSON
    }

    // GET /api/rutinas/buscar?nombre=fuer
    // [FromQuery] toma el valor del query string (lo que va después del "?").
    [HttpGet("buscar")]
    public async Task<ActionResult<List<RutinaResumenDto>>> Buscar([FromQuery] string? nombre)
    {
        List<RutinaResumenDto> rutinas = await _rutinaService.ListarAsync(nombre);
        // Si no hay coincidencias devolvemos 200 con lista vacía [] (no 404):
        // "no encontré resultados" es una respuesta válida de una búsqueda.
        return Ok(rutinas);
    }

    // GET /api/rutinas/5
    // "{id:int}" captura el número de la URL y SOLO acepta enteros. Gracias a
    // eso "/api/rutinas/buscar" no se confunde con esta ruta.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RutinaDetalleDto>> Obtener(int id)
    {
        RutinaDetalleDto? rutina = await _rutinaService.ObtenerAsync(id);
        if (rutina == null)
        {
            return NotFound(new ErrorRespuesta($"No existe una rutina con id {id}."));  // 404
        }
        return Ok(rutina);
    }

    // POST /api/rutinas   (body: RutinaGuardarDto en JSON)
    // Cuando llega acá, el DTO YA pasó las validaciones de los atributos.
    [HttpPost]
    public async Task<ActionResult<RutinaDetalleDto>> Crear(RutinaGuardarDto dto)
    {
        // Validación de negocio que un atributo no puede hacer: el nombre único.
        if (await _rutinaService.ExisteNombreAsync(dto.Nombre))
        {
            // 409 Conflict: los datos son válidos pero chocan con algo que ya existe.
            return Conflict(new ErrorRespuesta($"Ya existe una rutina llamada \"{dto.Nombre.Trim()}\"."));
        }

        RutinaDetalleDto creada = await _rutinaService.CrearAsync(dto);

        // 201 Created: además del JSON, agrega el header
        // "Location: /api/rutinas/{id}" apuntando al recurso recién creado.
        return CreatedAtAction(nameof(Obtener), new { id = creada.Id }, creada);
    }

    // PUT /api/rutinas/5   (body: RutinaGuardarDto en JSON)
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RutinaDetalleDto>> Actualizar(int id, RutinaGuardarDto dto)
    {
        // Primero verificamos que exista (404) y después el nombre (409).
        if (!await _rutinaService.ExisteAsync(id))
        {
            return NotFound(new ErrorRespuesta($"No existe una rutina con id {id}."));
        }

        if (await _rutinaService.ExisteNombreAsync(dto.Nombre, idExcluir: id))
        {
            return Conflict(new ErrorRespuesta($"Ya existe otra rutina llamada \"{dto.Nombre.Trim()}\"."));
        }

        RutinaDetalleDto? actualizada = await _rutinaService.ActualizarAsync(id, dto);
        if (actualizada == null)
        {
            // Caso raro: alguien la borró entre la verificación y la actualización.
            return NotFound(new ErrorRespuesta($"No existe una rutina con id {id}."));
        }
        return Ok(actualizada);
    }

    // DELETE /api/rutinas/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        bool eliminada = await _rutinaService.EliminarAsync(id);
        if (!eliminada)
        {
            return NotFound(new ErrorRespuesta($"No existe una rutina con id {id}."));
        }
        // 204 No Content: salió bien y no hay nada que devolver.
        return NoContent();
    }
}
