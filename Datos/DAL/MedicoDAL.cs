using Comun.ViewModels;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Datos.DAL
{
    /// <summary>
    /// Clase de acceso a datos
    /// </summary>
    public class MedicoDAL
    {
        /// <summary>
        /// Método para obtener todos los items
        /// </summary>
        /// <param name="cantidad">Cantidad de elementos a obtener</param>
        /// <param name="pagina">Número de página de resultados</param>
        /// <param name="textoBusqueda">Texto para filtrar la búsqueda</param>
        public static ListadoPaginadoVMR<MedicoVMR> Gets(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<MedicoVMR> result = new ListadoPaginadoVMR<MedicoVMR>();

            using (var db = DbConexion.Create())
            {
                // Consulta base
                var query = db.Medico.Where(x => !x.borrado).Select(x => new MedicoVMR()
                {
                    id = x.id,
                    cedula = x.cedula,
                    nombre= x.nombre + " " + x.apellidoPaterno + (x.apellidoMaterno != null ? (" " + x.apellidoMaterno) : ""),
                    esEspecialista = x.esEspecialista
                });

                // Filtrado por texto de búsqueda
                if (textoBusqueda != null && textoBusqueda.Length > 0)
                {
                    query = query.Where(x =>
                        x.cedula.Contains(textoBusqueda)
                        || x.nombre.Contains(textoBusqueda)
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
        public static MedicoVMR Get(long id)
        {
            MedicoVMR item = null;

            using (var db = DbConexion.Create())
            {
                item = db.Medico.Where(x => x.id == id && !x.borrado).Select(x => new MedicoVMR()
                {
                    id = x.id,
                    cedula = x.cedula,
                    nombre = x.nombre,
                    apellidoPaterno = x.apellidoPaterno,
                    apellidoMaterno = x.apellidoMaterno,
                    habilitado = x.habilitado,
                    esEspecialista = x.esEspecialista
                }).FirstOrDefault();
            }

            return item;
        }

        /// <summary>
        /// Método para crear un nuevo item y retornar su identificador
        /// </summary>
        /// <param name="item">Datos del nuevo item</param>
        public static long Post(Medico item)
        {
            using (var db = DbConexion.Create())
            {
                item.borrado = false;
                db.Medico.Add(item);
                db.SaveChanges();
            }

            return item.id;
        }

        /// <summary>
        /// Método para editar un item
        /// </summary>
        /// <param name="item">Datos del item a editar</param>
        public static void Put(MedicoVMR item)
        {
            using (var db = DbConexion.Create())
            {
                Medico itemUpdate = db.Medico.Find(item.id);

                itemUpdate.cedula = item.cedula;
                itemUpdate.nombre = item.nombre;
                itemUpdate.apellidoPaterno = item.apellidoPaterno;
                itemUpdate.apellidoMaterno = item.apellidoMaterno;
                itemUpdate.habilitado = item.habilitado;
                itemUpdate.esEspecialista = item.esEspecialista;

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
                var items = db.Medico.Where(x => ids.Contains(x.id));

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
                result = db.Medico.Any(x => x.id == id && !x.borrado);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cantidad"></param>
        /// <param name="pagina"></param>
        /// <param name="textoBusqueda"></param>
        /// <returns></returns>
        public static ListadoPaginadoVMR<MedicoVMR> EspecialistasSinIngresos(
            int cantidad,
            int pagina,
            string textoBusqueda
        )
        {
            ListadoPaginadoVMR<MedicoVMR> result = new ListadoPaginadoVMR<MedicoVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.Medico.Where(m => 
                    !m.borrado 
                    && m.esEspecialista 
                    && !m.Ingreso.Any(i => i.borrado)
                  //&& !db.Ingreso.Any(i => i.borrado && i.medicoId == m.id)
                ).Select(m => new MedicoVMR { 
                    id = m.id,
                    cedula = m.cedula,
                    nombre= m.nombre + " " + m.apellidoPaterno,
                    habilitado= m.habilitado
                });

                // Filtrado por texto de búsqueda
                if (textoBusqueda != null && textoBusqueda.Length > 0)
                {
                    query = query.Where(x =>
                        x.cedula.Contains(textoBusqueda)
                        || x.nombre.Contains(textoBusqueda)
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

        public static ListadoPaginadoVMR<MedicoSuplenteEgresoVMR> MedicosSuplentesEgresos(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<MedicoSuplenteEgresoVMR> result = new ListadoPaginadoVMR<MedicoSuplenteEgresoVMR>();

            using (var db = DbConexion.Create())
            {
                var query = (from me in db.Medico
                             join e in db.Egreso on me.id equals e.medicoId
                             join i in db.Ingreso on e.ingresoId equals i.id
                             join mi in db.Medico on i.medicoId equals mi.id
                             where
                                !me.borrado
                                && !e.borrado
                                && !i.borrado
                                && e.medicoId != i.medicoId
                            select new MedicoSuplenteEgresoVMR {
                                egreso = new EgresoVMR
                                {
                                    id = e.id,
                                    fecha = e.fecha,
                                    Medico = new MedicoVMR
                                    {
                                        id = me.id,
                                        cedula = me.cedula,
                                        nombre = me.nombre + " " + me.apellidoPaterno,
                                    }
                                },
                                ingreso = new IngresoVMR
                                {
                                    id = i.id,
                                    fecha = i.fecha,
                                    Medico = new MedicoVMR
                                    {
                                        id = mi.id,
                                        cedula = mi.cedula,
                                        nombre = mi.nombre + " " + mi.apellidoPaterno,
                                    }
                                }
                            });

                // Filtrado por texto de búsqueda
                if (textoBusqueda != null && textoBusqueda.Length > 0)
                {
                    query = query.Where(x =>
                        x.egreso.Medico.cedula.Contains(textoBusqueda)
                        || x.egreso.Medico.nombre.Contains(textoBusqueda)
                        || x.ingreso.Medico.cedula.Contains(textoBusqueda)
                        || x.ingreso.Medico.nombre.Contains(textoBusqueda)
                    );
                }

                // Conteo total de elementos
                result.cantidadTotal = query.Count();

                // Paginado
                result.elementos = query
                    .OrderBy(x => x.egreso.id)
                    .Skip(pagina * cantidad)
                    .Take(cantidad)
                    .ToList();
            }

            return result;
        }

        public static List<MedicosDeshabilitadosConIngresos> MedicosDeshabilitadosConIngresos(string textoBusqueda)
        {
            List<MedicosDeshabilitadosConIngresos> result = new List<MedicosDeshabilitadosConIngresos>();

            List<Medico> tem = null;
            List<Medico> tem2 = new List<Medico>();

            using (var db = DbConexion.Create())
            {
                var query = (from m in db.Medico
                             where 
                                !m.borrado
                                && !m.habilitado
                                && m.Ingreso.Any(i => 
                                    !i.borrado
                                    && !i.Egreso.Any(e => !e.borrado)
                                    //&& i.Egreso.Where(e => !e.borrado).Count() == 0
                                )
                            select new MedicosDeshabilitadosConIngresos
                            {
                                cedula = m.cedula,
                                nombre = m.nombre + " " + m.apellidoPaterno,
                                cantidadPacientesIngresados = m.Ingreso.Where(i =>
                                                                !i.borrado
                                                                && !i.Egreso.Any(e => !e.borrado)).Count()
                            }
                           );

                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x =>
                        x.cedula.Contains(textoBusqueda)
                        || x.nombre.Contains(textoBusqueda)
                    );
                }

                result = query.ToList();
            }

            return result;
        }
    }
}
