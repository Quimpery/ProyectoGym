using FinalProgramacion4.GymRutinas.api.Data;
using FinalProgramacion4.GymRutinas.api.Interfaces;
using FinalProgramacion4.GymRutinas.api.Models;

namespace FinalProgramacion4.GymRutinas.api.Services
{
    public class ServiceRutina:IRutinas
    {
        private readonly AplicationDbContext context; 

        public ServiceRutina(AplicationDbContext context)
        {
            this.context = context;
        }
        public Rutina CreateRutina(Rutina rutina)
        {
            context.Rutinas.Add(rutina);
            context.SaveChanges();
            return rutina;

        }

        public Rutina UpdateRutina(int id)
        {
            var rutina = context.Rutinas.Find(id);
            if (rutina == null)
            {
                throw new Exception("Rutina no encontrada");
            }
            context.Rutinas.Update(rutina);
            context.SaveChanges();
            return rutina;
        }

        public void DeleteRutina(int id)
        {
            
            var rutina = context.Rutinas.Find(id);
            if (rutina == null)
            {
                throw new Exception("Rutina no encontrada");
            }
            context.Rutinas.Remove(rutina);
            context.SaveChanges();
        }
        
        public List<Ejercicio> VerEjerciciosPorRutina(int idRutina)
        {
            var rutina = context.Rutinas.Find(idRutina);
            if (rutina == null)
            {
                throw new Exception("Rutina no encontrada");
            }
            return rutina.EjerciciosPorDia.ToList();
        }


    }
}
