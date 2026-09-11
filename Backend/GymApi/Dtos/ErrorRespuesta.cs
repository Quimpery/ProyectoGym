namespace GymApi.Dtos;

// ------------------------------------------------------------
// Formato ÚNICO para todas las respuestas de error de la API
// ------------------------------------------------------------
// Siempre que algo sale mal (400, 404, 409, 500) devolvemos un JSON así:
// {
//   "mensaje": "Los datos enviados no son válidos.",
//   "errores": ["Las series deben ser un número entre 1 y 100."]
// }
// Tener un formato consistente le simplifica mucho la vida al front: siempre
// sabe dónde buscar el texto para mostrarle al usuario.
public class ErrorRespuesta
{
    // Mensaje general, pensado para mostrarse directamente al usuario.
    public string Mensaje { get; set; }

    // Detalle opcional (por ejemplo, la lista de validaciones que fallaron).
    public List<string>? Errores { get; set; }

    public ErrorRespuesta(string mensaje, List<string>? errores = null)
    {
        Mensaje = mensaje;
        Errores = errores;
    }
}
