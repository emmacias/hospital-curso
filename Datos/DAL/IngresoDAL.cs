using Comun.ViewModels;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.DAL
{
    /// <summary>
    /// Clase de acceso a datos
    /// </summary>
    public class IngresoDAL
    {
        /// <summary>
        /// Método para obtener todos los items
        /// </summary>
        /// <param name="cantidad">Cantidad de elementos a obtener</param>
        /// <param name="pagina">Número de página de resultados</param>
        /// <param name="textoBusqueda">Texto para filtrar la búsqueda</param>
        public static ListadoPaginadoVMR<IngresoVMR> Gets(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<IngresoVMR> result = new ListadoPaginadoVMR<IngresoVMR>();

            using (var db = DbConexion.Create())
            {
                // Consulta base
                var query = db.Ingreso.Where(x => !x.borrado).Select(x => new IngresoVMR()
                {
                    id = x.id,
                    fecha = x.fecha,
                    numeroSala = x.numeroSala,
                    numeroCama = x.numeroCama,
                    diagnostico = x.diagnostico,
                    Medico = db.Medico.Where(y => y.id == x.medicoId).Select(y => new MedicoVMR
                    {
                        cedula = y.cedula,
                        nombre = y.nombre + " " + y.apellidoPaterno + (y.apellidoMaterno != null ? (" " + y.apellidoMaterno) : "")
                    }).FirstOrDefault(),
                    Paciente = db.Paciente.Where(y => y.id == x.pacienteId).Select(y => new PacienteVMR
                    {
                        cedula = y.cedula,
                        nombre = y.nombre + " " + y.apellidoPaterno + (y.apellidoMaterno != null ? (" " + y.apellidoMaterno) : "")
                    }).FirstOrDefault()
                });

                // Filtrado por texto de búsqueda
                if (textoBusqueda != null && textoBusqueda.Length > 0)
                {
                    query = query.Where(x => 
                        x.diagnostico.Contains(textoBusqueda) 
                        || x.Medico.cedula.Contains(textoBusqueda) 
                        || x.Medico.nombre.Contains(textoBusqueda)
                        || x.Paciente.cedula.Contains(textoBusqueda)
                        || x.Paciente.nombre.Contains(textoBusqueda)
                    );
                }

                // Conteo total de elementos
                result.cantidadTotal = query.Count();

                // Paginado
                result.elementos = query
                    .OrderBy(x => x.id)
                    .Skip(pagina * cantidad)
                    .Take(cantidad)
                    .ToList();
            }

            return result;
        }

        /// <summary>
        /// Método para obtener item por su identificador
        /// </summary>
        /// <param name="id">Identificador del item que se desea obtener</param>
        public static IngresoVMR Get(long id)
        {
            IngresoVMR item = null;

            using (var db = DbConexion.Create())
            {
                item = db.Ingreso.Where(x => x.id == id && !x.borrado).Select(x => new IngresoVMR()
                {
                    id = x.id,
                    fecha = x.fecha,
                    diagnostico = x.diagnostico,
                    pacienteId = x.pacienteId,
                    medicoId = x.medicoId,
                    numeroCama = x.numeroCama,
                    numeroSala = x.numeroSala,
                    observacion = x.observacion
                }).FirstOrDefault();
            }

            return item;
        }

        /// <summary>
        /// Método para crear un nuevo item y retornar su identificador
        /// </summary>
        /// <param name="item">Datos del nuevo item</param>
        public static long Post(Ingreso item)
        {
            using (var db = DbConexion.Create())
            {
                item.fecha = DateTime.Now;
                item.borrado = false;
                db.Ingreso.Add(item);
                db.SaveChanges();
            }

            return item.id;
        }

        /// <summary>
        /// Método para editar un item
        /// </summary>
        /// <param name="item">Datos del item a editar</param>
        public static void Put(IngresoVMR item)
        {
            using (var db = DbConexion.Create())
            {
                Ingreso itemUpdate = db.Ingreso.Find(item.id);

                itemUpdate.numeroSala = item.numeroSala;
                itemUpdate.numeroCama = item.numeroCama;
                itemUpdate.medicoId = item.medicoId;
                itemUpdate.diagnostico = item.diagnostico;
                itemUpdate.observacion = item.observacion;

                db.Entry(itemUpdate).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Método para borrar varios items por sus identificadores
        /// </summary>
        /// <param name="ids">Identificadores de los items que se desean borrar</param>
        public static void Delete(List<long> ids)
        {
            using (var db = DbConexion.Create())
            {
                var items = db.Ingreso.Where(x => ids.Contains(x.id));

                foreach (var item in items)
                {
                    item.borrado = true;
                    db.Entry(item).State = EntityState.Modified;
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Método para comprobar la existencia de un item por su identificador
        /// </summary>
        /// <param name="id">Identificador del item que se desea comprobar su existencia</param>
        public static bool ItemExists(long id)
        {
            var result = false;

            using (var db = DbConexion.Create())
            {
                result = db.Ingreso.Any(x => x.id == id && !x.borrado);
            }

            return result;
        }

        /// <summary>
        /// Método para obtener los datos necesarios para el formulario
        /// </summary>
        /// <param name="id">Identificador del item (0 cuando el formulario es de creación)</param>
        /// <returns></returns>
        public static IngresoFormDataVMR GetFormData(long id)
        {
            IngresoFormDataVMR datos = new IngresoFormDataVMR();

            using (var db = DbConexion.Create())
            {
                long pacienteId = 0;
                long medicoId = 0;

                if (id != 0)
                {
                    var item = db.Ingreso.Where(x => x.id == id).Select(x => new
                    {
                        x.pacienteId,
                        x.medicoId
                    }).FirstOrDefault();

                    pacienteId = item.pacienteId;
                    medicoId = item.medicoId;
                }

                datos.PacienteList = db.Paciente.Where(x => !x.borrado || x.id == pacienteId).Select(x => new PacienteVMR
                {
                    id = x.id,
                    cedula = x.cedula,
                    nombre = x.nombre + " " + x.apellidoPaterno + (x.apellidoMaterno != null ? (" " + x.apellidoMaterno) : "")
                }).ToList();

                datos.MedicoList = db.Medico.Where(x => (!x.borrado && x.habilitado && x.esEspecialista) || x.id == medicoId).Select(x => new MedicoVMR
                {
                    id = x.id,
                    cedula = x.cedula,
                    nombre = x.nombre + " " + x.apellidoPaterno + (x.apellidoMaterno != null ? (" " + x.apellidoMaterno) : "")
                }).ToList();
            }

            return datos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ListadoPaginadoVMR<IngresoVMR> IngresosEspeciales(
            int cantidad,
            int pagina,
            string textoBusqueda,
            DateTime fechaIni,
            DateTime fechaFin
        )
        {
            ListadoPaginadoVMR<IngresoVMR> result = new ListadoPaginadoVMR<IngresoVMR>();

            using (var db = DbConexion.Create())
            {
                // Consulta base
                var query = db.Ingreso.Where(i =>
                    !i.borrado
                    && i.fecha >= fechaIni
                    && i.fecha <= fechaFin
                    && i.diagnostico.Contains("covid")
                    && i.numeroSala >= 1
                    && i.numeroSala <= 20
                ).Select(x => new IngresoVMR
                {
                    id = x.id,
                    fecha = x.fecha,
                    numeroSala = x.numeroSala,
                    numeroCama = x.numeroCama,
                    diagnostico = x.diagnostico,
                    observacion= x.observacion,
                    Medico = db.Medico.Where(y => y.id == x.medicoId).Select(y => new MedicoVMR
                    {
                        cedula = y.cedula,
                        nombre = y.nombre + " " + y.apellidoPaterno + (y.apellidoMaterno != null ? (" " + y.apellidoMaterno) : "")
                    }).FirstOrDefault(),
                    Paciente = db.Paciente.Where(y => y.id == x.pacienteId).Select(y => new PacienteVMR
                    {
                        cedula = y.cedula,
                        nombre = y.nombre + " " + y.apellidoPaterno + (y.apellidoMaterno != null ? (" " + y.apellidoMaterno) : "")
                    }).FirstOrDefault()
                });

                // Filtrado por texto de búsqueda
                if (textoBusqueda != null && textoBusqueda.Length > 0)
                {
                    query = query.Where(x =>
                        x.Medico.cedula.Contains(textoBusqueda)
                        || x.Medico.nombre.Contains(textoBusqueda)
                        || x.Paciente.cedula.Contains(textoBusqueda)
                        || x.Paciente.nombre.Contains(textoBusqueda)
                    );
                }

                // Conteo total de elementos
                result.cantidadTotal = query.Count();

                // Paginado
                result.elementos = query
                    .OrderBy(x => x.id)
                    .Skip(pagina * cantidad)
                    .Take(cantidad)
                    .ToList();
            }

            return result;
        }
    }
}
