using FinalProgramacion4.GymRutinas.api.Models;

namespace FinalProgramacion4.GymRutinas.api.Interfaces
{
    public interface IRutinas
    {
        Rutina CreateRutina(Rutina rutina);
        Rutina UpdateRutina(int id);
        void DeleteRutina(int id);

        List<Ejercicio> VerEjerciciosPorRutina(int idRutina);
    }
}
