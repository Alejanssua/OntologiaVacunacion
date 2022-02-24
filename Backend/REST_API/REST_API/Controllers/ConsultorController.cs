using DataSource;
using Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace REST_API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ConsultorController : ApiController
    {
        [HttpPost]
        [Route("api/consulta/")]
        public IHttpActionResult Idioma(ParametroBusqueda parametro)
        {
            Ontologia service = new Ontologia();

            Persona a = new Persona();
            a.status_vacunacion = parametro.status_vacunacion;
            a.ciudad = parametro.ciudad;
            a.dosis_aplicadas = parametro.dosis_aplicadas;
            if (!string.IsNullOrEmpty(parametro.enfermedad))
            {
                string[] enfermedades;
                if (parametro.enfermedad.IndexOf(',') > 0)
                {
                    enfermedades = parametro.enfermedad.Split(',');
                }
                else
                {
                    enfermedades = new string[1];
                    enfermedades[0] = parametro.enfermedad;
                }
                List<Enfermedad> m = new List<Enfermedad>();
                foreach (var item in enfermedades)
                {
                    Enfermedad curr = (Enfermedad)service.getRecurso(item);
                    if (curr != null) {
                        m.Add(curr);
                    }
                }
                a.enfermedad = m;

            }

            if (!string.IsNullOrEmpty(parametro.vacuna))
            {
                string[] vacunas;
                if (parametro.vacuna.IndexOf(',') > 0)
                {
                    vacunas = parametro.vacuna.Split(',');
                }
                else {
                    vacunas = new string[1];
                    vacunas[0] = parametro.vacuna;
                }
                List<Vacuna> m = new List<Vacuna>();
                foreach (var item in vacunas)
                {
                    Vacuna curr = (Vacuna)service.getRecurso(item);
                    if (curr != null)
                    {
                        m.Add(curr);
                    }
                }
                a.vacuna = m;
            }
            List<Persona> recursos = service.consulta(a);
            return Ok(recursos);
        }

        [HttpGet]
        [Route("api/resource/{resource}")]
        public IHttpActionResult getResource(string resource)
        {

            Ontologia service = new Ontologia();
            RecursoRDF res = service.getRecurso(resource);
            if (res != null)
            {
                return Ok(res);
            }
            return NotFound();
        }

        [HttpPost]
        [Route("api/resource/persona/")]
        public IHttpActionResult addNewFruta(Persona resource)
        {
            if (resource != null)
            {
                Ontologia service = new Ontologia();
                if (service.crearFruta(resource))
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }
            }
            return BadRequest("Insufisiencia de datos");
        }

        [HttpDelete]
        [Route("api/resource/persona/{resourceName}")]
        public IHttpActionResult deleteFruta(string resourceName)
        {
            new Ontologia().deleteFruta(resourceName);
            return Ok();
        }
    }
}