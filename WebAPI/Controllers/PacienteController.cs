using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Comun.ViewModels;
using Logica.BLL;
using Modelo.Modelos;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controlador WebAPI
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PacienteController : ApiController
    {
        // GET: api/paciente
        /// <summary>
        /// Servicio para obtener todos los items
        /// </summary>
        /// <param name="cantidad">Cantidad de elementos a obtener</param>
        /// <param name="pagina">Número de página de resultados</param>
        /// <param name="textoBusqueda">Texto para filtrar la búsqueda</param>
        /// <remarks>Servicio para obtener todos los items</remarks>
        /// <response code="200">Ejecución exitosa</response>
        /// <response code="400">Error en los datos enviados</response>
        /// <response code="401">Token no válido o caducado</response>
        /// <response code="404">Recurso no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [ResponseType(typeof(RespuestaVMR<ListadoPaginadoVMR<PacienteVMR>>))]
        public IHttpActionResult Gets(int cantidad = 20, int pagina = 0, string textoBusqueda = null)
        {
            var respuesta = new RespuestaVMR<ListadoPaginadoVMR<PacienteVMR>>();

            try
            {
                respuesta.datos = PacienteBLL.Gets(cantidad, pagina, textoBusqueda);
            }
            catch (Exception e)
            {
                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = null;
                respuesta.mensajes.Add(e.Message);
                respuesta.mensajes.Add(e.ToString());
            }

            return Content(respuesta.codigo, respuesta);
        }

        // GET: api/paciente/{id}
        /// <summary>
        /// Servicio para obtener un item por su identificador
        /// </summary>
        /// <param name="id">Identificador del item que se desea obtener</param>
        /// <remarks>Servicio para obtener un item por su identificador</remarks>
        /// <response code="200">Ejecución exitosa</response>
        /// <response code="400">Error en los datos enviados</response>
        /// <response code="401">Token no válido o caducado</response>
        /// <response code="404">Recurso no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [ResponseType(typeof(RespuestaVMR<PacienteVMR>))]
        public IHttpActionResult Get(long id)
        {
            var respuesta = new RespuestaVMR<PacienteVMR>();

            try
            {
                respuesta.datos = PacienteBLL.Get(id);
            }
            catch (Exception e)
            {
                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = null;
                respuesta.mensajes.Add(e.Message);
                respuesta.mensajes.Add(e.ToString());
            }

            if (respuesta.datos == null && respuesta.mensajes.Count() == 0)
            {
                respuesta.codigo = HttpStatusCode.NotFound;
                respuesta.mensajes.Add("Elemento no encontrado.");
            }

            return Content(respuesta.codigo, respuesta);
        }

        // POST: api/paciente
        /// <summary>
        /// Servicio para crear un nuevo item y retornar su identificador
        /// </summary>
        /// <param name="item">Datos del nuevo item</param>
        /// <remarks>Servicio para crear un nuevo item</remarks>
        /// <response code="200">Ejecución exitosa</response>
        /// <response code="400">Error en los datos enviados</response>
        /// <response code="401">Token no válido o caducado</response>
        /// <response code="404">Recurso no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [ResponseType(typeof(RespuestaVMR<long?>))]
        public IHttpActionResult Post(Paciente item)
        {
            var respuesta = new RespuestaVMR<long?>();

            try
            {
                respuesta.datos = PacienteBLL.Post(item);
            }
            catch (Exception e)
            {
                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = null;
                respuesta.mensajes.Add(e.Message);
                respuesta.mensajes.Add(e.ToString());
            }

            return Content(respuesta.codigo, respuesta);
        }

        // PUT: api/paciente/{id}
        /// <summary>
        /// Servicio para editar un item por su identificador
        /// </summary>
        /// <param name="id">Identificador del item que se desea editar</param>
        /// <param name="item">Nuevos datos del item</param>
        /// <remarks>Servicio para editar un item por su identificador</remarks>
        /// <response code="200">Ejecución exitosa</response>
        /// <response code="400">Error en los datos enviados</response>
        /// <response code="401">Token no válido o caducado</response>
        /// <response code="404">Recurso no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [ResponseType(typeof(RespuestaVMR<bool>))]
        public IHttpActionResult Put(long id, PacienteVMR item)
        {
            var respuesta = new RespuestaVMR<bool>();

            try
            {
                item.id = id;
                PacienteBLL.Put(item);
                respuesta.datos = true;
            }
            catch (Exception e)
            {
                try
                {
                    if (!PacienteBLL.ItemExists(id))
                    {
                        respuesta.codigo = HttpStatusCode.NotFound;
                        respuesta.datos = false;
                        respuesta.mensajes.Add("Elemento no encontrado.");
                        return Content(respuesta.codigo, respuesta);
                    }
                }
                catch (Exception) { }

                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = false;
                respuesta.mensajes.Add(e.Message);
                respuesta.mensajes.Add(e.ToString());
            }

            return Content(respuesta.codigo, respuesta);
        }

        // DELETE: api/paciente
        /// <summary>
        /// Servicio para borrar varios items por su identificador
        /// </summary>
        /// <param name="ids">Ids de los items que se desean borrar</param>
        /// <remarks>Servicio para borrar varios items por su identificador</remarks>
        /// <response code="200">Ejecución exitosa</response>
        /// <response code="400">Error en los datos enviados</response>
        /// <response code="401">Token no válido o caducado</response>
        /// <response code="404">Recurso no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [ResponseType(typeof(RespuestaVMR<bool>))]
        public IHttpActionResult Delete(List<long> ids)
        {
            var respuesta = new RespuestaVMR<bool>();

            try
            {
                PacienteBLL.Delete(ids);
                respuesta.datos = true;
            }
            catch (Exception e)
            {
                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = false;
                respuesta.mensajes.Add(e.Message);
                respuesta.mensajes.Add(e.ToString());
            }

            return Content(respuesta.codigo, respuesta);
        }

        /// <summary>
        /// Liberar los recursos de la aplicación
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        // GET: api/paciente/pacientes-ingresados
        /// <summary>
        /// Servicio para obtener los pacientes ingresados
        /// </summary>
        /// <param name="cantidad">Cantidad de elementos a obtener</param>
        /// <param name="pagina">Número de página de resultados</param>
        /// <param name="textoBusqueda">Texto para filtrar la búsqueda</param>
        /// <remarks>Servicio para obtener los pacientes ingresados</remarks>
        /// <response code="200">Ejecución exitosa</response>
        /// <response code="400">Error en los datos enviados</response>
        /// <response code="401">Token no válido o caducado</response>
        /// <response code="404">Recurso no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [ResponseType(typeof(RespuestaVMR<ListadoPaginadoVMR<PacienteIngresoVMR>>))]
        [Route("api/paciente/pacientes-ingresados")]
        public IHttpActionResult GetPacientesIngresados(int cantidad = 20, int pagina = 0, string textoBusqueda = null)
        {
            var respuesta = new RespuestaVMR<ListadoPaginadoVMR<PacienteIngresoVMR>>();

            try
            {
                respuesta.datos = PacienteBLL.PacientesIngresados(cantidad, pagina, textoBusqueda);
            }
            catch (Exception e)
            {
                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = null;
                respuesta.mensajes.Add(e.Message);
                respuesta.mensajes.Add(e.ToString());
            }

            return Content(respuesta.codigo, respuesta);
        }
    }
}
