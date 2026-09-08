using FinalProgramacion4.GymRutinas.api.Models;

namespace FinalProgramacion4.GymRutinas.api.Interfaces
{
    public interface IEjercicios
    {
        Ejercicio CreateEjercicio(Ejercicio ejercicio);
        Ejercicio UpdateEjercicio(int id);
        void DeleteEjercicio(int id);

      List<Ejercicio>VerEjercicios();
        


    }
}
