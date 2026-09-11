namespace GymApi.Dtos;

// ------------------------------------------------------------
// DTO de SALIDA para el DETALLE de una rutina (con todos sus ejercicios)
// ------------------------------------------------------------
// Los ejercicios vienen ordenados por día (Lunes -> Domingo) y dentro de cada
// día por su campo Orden. El front solo tiene que agruparlos para mostrarlos.
public class RutinaDetalleDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public List<EjercicioDto> Ejercicios { get; set; } = new();
}
