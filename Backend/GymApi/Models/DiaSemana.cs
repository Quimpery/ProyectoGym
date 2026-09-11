namespace GymApi.Models;

// ------------------------------------------------------------
// ENUM DiaSemana
// ------------------------------------------------------------
// Un enum es una lista cerrada de valores posibles. Usarlo en vez de un
// string suelto hace que sea IMPOSIBLE guardar un día inválido como "Lunez":
// si alguien manda eso en el JSON, ASP.NET no puede convertirlo y responde
// 400 Bad Request automáticamente.
//
// Los nombres van sin tilde porque así viajan en el JSON ("Miercoles").
// El front se encarga de mostrarlos lindos ("Miércoles").
//
// Los números (1..7) definen el ORDEN natural de la semana. Lo usamos para
// ordenar los ejercicios de Lunes a Domingo (y no alfabéticamente).
public enum DiaSemana
{
    Lunes = 1,
    Martes = 2,
    Miercoles = 3,
    Jueves = 4,
    Viernes = 5,
    Sabado = 6,
    Domingo = 7
}
