using FinalProgramacion4.GymRutinas.api.Interfaces;
using FinalProgramacion4.GymRutinas.api.Models;
using FinalProgramacion4.GymRutinas.api.Data;


namespace FinalProgramacion4.GymRutinas.api.Services
{
    public class ServiceEjercicio : IEjercicios
    {
        private readonly List<Ejercicio> ejercicios = new List<Ejercicio>();
        readonly AplicationDbContext context;
        public ServiceEjercicio(AplicationDbContext context)
        {
            this.context = context;
        }
        public Ejercicio CreateEjercicio(Ejercicio ejercicio)
        {
            context.Ejercicios.Add(ejercicio);
            context.SaveChanges();
            return ejercicio;

        }

        public void DeleteEjercicio(int id)
        {
            context.Ejercicios.Remove(context.Ejercicios.Find(id));
        }

        public Ejercicio UpdateEjercicio(int id)
        {
            context.Ejercicios.Update(context.Ejercicios.Find(id));
            context.SaveChanges();
            return context.Ejercicios.Find(id);
        }

        public List<Ejercicio> VerEjercicios()
        {
          return ejercicios.ToList();
        }
    }
}
