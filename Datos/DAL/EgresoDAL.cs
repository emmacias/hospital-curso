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
    public class EgresoDAL
    {
        /// <summary>
        /// Método para obtener todos los items
        /// </summary>
        /// <param name="cantidad">Cantidad de elementos a obtener</param>
        /// <param name="pagina">Número de página de resultados</param>
        /// <param name="textoBusqueda">Texto para filtrar la búsqueda</param>
        public static ListadoPaginadoVMR<EgresoVMR> Gets(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<EgresoVMR> result = new ListadoPaginadoVMR<EgresoVMR>();

            using (var db = DbConexion.Create())
            {
                // Consulta base
                var query = db.Egreso.Where(x => !x.borrado).Select(x => new EgresoVMR()
                {
                    id = x.id,
                    fecha = x.fecha,
                    monto = x.monto,
                    tratamiento = x.tratamiento,
                    Medico = db.Medico.Where(y => y.id == x.medicoId).Select(y => new MedicoVMR {
                        cedula = y.cedula,
                        nombre = y.nombre + " " + y.apellidoPaterno + (y.apellidoMaterno != null ? (" " + y.apellidoMaterno) : "")
                    }).FirstOrDefault()
                });

                // Filtrado por texto de búsqueda
                if (textoBusqueda != null && textoBusqueda.Length > 0)
                {
                    query = query.Where(x => (x.tratamiento != null && x.tratamiento.Contains(textoBusqueda)) || x.Medico.nombre.Contains(textoBusqueda) || x.Medico.cedula.Contains(textoBusqueda));
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
        public static EgresoVMR Get(long id)
        {
            EgresoVMR item = null;

            using (var db = DbConexion.Create())
            {
                item = db.Egreso.Where(x => x.id == id && !x.borrado).Select(x => new EgresoVMR()
                {
                    id = x.id,
                    fecha = x.fecha,
                    monto = x.monto,
                    tratamiento = x.tratamiento,
                    ingresoId = x.ingresoId,
                    medicoId = x.medicoId
                }).FirstOrDefault();
            }

            return item;
        }

        /// <summary>
        /// Método para crear un nuevo item y retornar su identificador
        /// </summary>
        /// <param name="item">Datos del nuevo item</param>
        public static long Post(Egreso item)
        {
            using (var db = DbConexion.Create())
            {
                item.fecha = DateTime.Now;
                item.borrado = false;
                db.Egreso.Add(item);
                db.SaveChanges();
            }

            return item.id;
        }

        /// <summary>
        /// Método para editar un item
        /// </summary>
        /// <param name="item">Datos del item a editar</param>
        public static void Put(EgresoVMR item)
        {
            using (var db = DbConexion.Create())
            {
                Egreso itemUpdate = db.Egreso.Find(item.id);

                itemUpdate.tratamiento = item.tratamiento;
                itemUpdate.monto = item.monto;
                itemUpdate.medicoId = item.medicoId;

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
                var items = db.Egreso.Where(x => ids.Contains(x.id));

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
                result = db.Egreso.Any(x => x.id == id && !x.borrado);
            }

            return result;
        }

        /// <summary>
        /// Método para obtener los datos necesarios para el formulario
        /// </summary>
        /// <param name="id">Identificador del item (0 cuando el formulario es de creación)</param>
        /// <returns></returns>
        public static EgresoFormDataVMR GetFormData(long id)
        {
            EgresoFormDataVMR datos = new EgresoFormDataVMR();

            using (var db = DbConexion.Create())
            {
                long ingresoId = 0;
                long medicoId = 0;

                if (id != 0)
                {
                    var item = db.Egreso.Where(x => x.id == id).Select(x => new
                    {
                        x.ingresoId,
                        x.medicoId
                    }).FirstOrDefault();

                    ingresoId = item.ingresoId;
                    medicoId = item.medicoId;
                }

                datos.IngresoList = db.Ingreso.Include("Paciente").Where(x => 
                    (
                        !x.borrado
                        && !db.Egreso.Any(y => !y.borrado && y.ingresoId == x.id)
                    ) 
                    || x.id == ingresoId
                ).Select(x => new IngresoVMR
                {
                    id = x.id,
                    numeroSala = x.numeroSala,
                    numeroCama = x.numeroCama,
                    Paciente = db.Paciente.Where(y => y.id == x.pacienteId).Select(y => new PacienteVMR
                    {
                        cedula = y.cedula,
                        nombre = y.nombre + " " + y.apellidoPaterno + (y.apellidoMaterno != null ? (" " + y.apellidoMaterno) : "")
                    }).FirstOrDefault()
                }).ToList();

                datos.MedicoList = db.Medico.Where(x => (!x.borrado && x.habilitado) || x.id == medicoId).Select(x => new MedicoVMR
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
        /// <param name="textoBusqueda"></param>
        /// <param name="fechaIni"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        public static List<EgresoVMR> EgresosEnRangoFecha(
            string textoBusqueda, 
            DateTime fechaIni, 
            DateTime fechaFin
        )
        {
            List<EgresoVMR> result = new List<EgresoVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.Egreso.Where(e =>
                    !e.borrado
                    && e.fecha >= fechaIni
                    && e.fecha <= fechaFin
                ).Select(e => new EgresoVMR
                {
                    id = e.id,
                    fecha= e.fecha,
                    monto= e.monto,
                    Medico = new MedicoVMR { 
                        id = e.Medico.id,
                        cedula = e.Medico.cedula,
                        nombre = e.Medico.nombre + " " + e.Medico.apellidoPaterno
                    }
                });

                if (textoBusqueda != null && textoBusqueda.Length > 0)
                {
                    query = query.Where(x => 
                        x.Medico.cedula.Contains(textoBusqueda)
                        || x.Medico.nombre.Contains(textoBusqueda)
                    );
                }

                result = query.ToList();
            }

            return result;
        }

        public static ListadoPaginadoVMR<EgresoVMR> EgresosDeMedicosNoEspecialistasDeshabilitados(
            int cantidad,
            int pagina,
            string textoBusqueda
        )
        {
            ListadoPaginadoVMR<EgresoVMR> result = new ListadoPaginadoVMR<EgresoVMR>();

            using (var db = DbConexion.Create())
            {
                // Consulta base
                var query = (from e in db.Egreso
                             join m in db.Medico on e.medicoId equals m.id
                             where
                                !e.borrado
                                && !m.esEspecialista
                                && !m.habilitado
                             select new EgresoVMR { 
                                id = e.id,
                                fecha= e.fecha,
                                tratamiento= e.tratamiento,
                                monto= e.monto,
                                Medico = new MedicoVMR
                                {
                                    id = m.id,
                                    cedula = m.cedula,
                                    nombre = m.nombre + " " + m.apellidoPaterno
                                }
                             });

                // Filtrado por texto de búsqueda
                if (textoBusqueda != null && textoBusqueda.Length > 0)
                {
                    query = query.Where(x => 
                        x.Medico.cedula.Contains(textoBusqueda)
                        || x.Medico.nombre.Contains(textoBusqueda)
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
