using FinalProgramacion4.GymRutinas.api.Services;
using Microsoft.AspNetCore.Mvc;
using FinalProgramacion4.GymRutinas.api.Models;
namespace FinalProgramacion4.GymRutinas.api.Controllers
    
{
    [ApiController]
    public class EjercicioController:ControllerBase
    {
        private readonly ServiceEjercicio serviceEjercicio;
        public EjercicioController(ServiceEjercicio serviceEjercicio)
        {
            this.serviceEjercicio = serviceEjercicio;
        }

        [HttpGet]
        [Route("api/ejercicios")]
        public IActionResult GetEjercicios()
        {
            Ok( new { message = "Lista de ejercicios" });
            serviceEjercicio.VerEjercicios();
            return Ok(new { message = "Fin de lista" });
        }

        [HttpPost]
        [Route("api/ejercicios")]   
        public IActionResult CreateEjercicio([FromBody] Ejercicio ejercicio)
        {
            try
            {
                if(ejercicio != null)
                {
                    serviceEjercicio.CreateEjercicio(ejercicio);
                    return Ok(new { message = "Ejercicio creado correctamente" });
                }
                else
                {
                    return BadRequest(new { message = "El Ejercicio no puede ser nulo" });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al crear el Ejercicio: " + ex.Message });
            }

            

        }

        [HttpDelete]
        [Route("api/ejercicios/{id}")]
        public IActionResult DeleteEjercicio(int id)
        {
            try
            {
                if (id < 0)
                {
                    serviceEjercicio.DeleteEjercicio(id);
                    return Ok(new { message = "El Ejercicio fue eliminado correctamente" });
                }
                else
                {
                    return BadRequest(new { message = "El id del Ejercicio no puede ser negativo o 0" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al eliminar el Ejercicio: " + ex.Message });
            }


        }
    }
}
