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
    public class PacienteDAL
    {
        /// <summary>
        /// Método para obtener todos los items
        /// </summary>
        /// <param name="cantidad">Cantidad de elementos a obtener</param>
        /// <param name="pagina">Número de página de resultados</param>
        /// <param name="textoBusqueda">Texto para filtrar la búsqueda</param>
        public static ListadoPaginadoVMR<PacienteVMR> Gets(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<PacienteVMR> result = new ListadoPaginadoVMR<PacienteVMR>();

            using (var db = DbConexion.Create())
            {
                // Consulta base
                var query = db.Paciente.Where(x => !x.borrado).Select(x => new PacienteVMR()
                {
                    id = x.id,
                    cedula = x.cedula,
                    nombre = x.nombre + " " + x.apellidoPaterno + (x.apellidoMaterno != null ? (" " + x.apellidoMaterno) : ""),
                    celular = x.celular,
                    correoElectronico = x.correoElectronico
                });

                // Filtrado por texto de búsqueda
                if (textoBusqueda != null && textoBusqueda.Length > 0)
                {
                    query = query.Where(x =>
                        x.cedula.Contains(textoBusqueda)
                        || x.nombre.Contains(textoBusqueda)
                        || x.celular.Contains(textoBusqueda)
                        || x.correoElectronico.Contains(textoBusqueda)
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
        public static PacienteVMR Get(long id)
        {
            PacienteVMR item = null;

            using (var db = DbConexion.Create())
            {
                item = db.Paciente.Where(x => x.id == id && !x.borrado).Select(x => new PacienteVMR()
                {
                    id = x.id,
                    cedula = x.cedula,
                    nombre = x.nombre,
                    apellidoPaterno = x.apellidoPaterno,
                    apellidoMaterno = x.apellidoMaterno,
                    celular = x.celular,
                    correoElectronico = x.correoElectronico,
                    direccion = x.direccion
                }).FirstOrDefault();
            }

            return item;
        }

        /// <summary>
        /// Método para crear un nuevo item y retornar su identificador
        /// </summary>
        /// <param name="item">Datos del nuevo item</param>
        public static long Post(Paciente item)
        {
            using (var db = DbConexion.Create())
            {
                item.borrado = false;
                db.Paciente.Add(item);
                db.SaveChanges();
            }

            return item.id;
        }

        /// <summary>
        /// Método para editar un item
        /// </summary>
        /// <param name="item">Datos del item a editar</param>
        public static void Put(PacienteVMR item)
        {
            using (var db = DbConexion.Create())
            {
                Paciente itemUpdate = db.Paciente.Find(item.id);

                itemUpdate.cedula = item.cedula;
                itemUpdate.nombre = item.nombre;
                itemUpdate.apellidoPaterno = item.apellidoPaterno;
                itemUpdate.apellidoMaterno = item.apellidoMaterno;
                itemUpdate.celular = item.celular;
                itemUpdate.correoElectronico = item.correoElectronico;
                itemUpdate.direccion = item.direccion;

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
                var items = db.Paciente.Where(x => ids.Contains(x.id));

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
                result = db.Paciente.Any(x => x.id == id && !x.borrado);
            }

            return result;
        }

        /// <summary>
        /// Devuelve un listado paginado de los pacientes ingresados actualmente.
        /// </summary>
        /// <param name="cantidad">Cantidad de elementos a visulalizar por pagina</param>
        /// <param name="pagina">Pagina actual en el paginado</param>
        /// <param name="textoBusqueda">Texto para filtrar los resultados</param>
        /// <returns></returns>
        public static ListadoPaginadoVMR<PacienteIngresoVMR> PacientesIngresados(
            int cantidad, 
            int pagina, 
            string textoBusqueda
        )
        {
            ListadoPaginadoVMR<PacienteIngresoVMR> result = new ListadoPaginadoVMR<PacienteIngresoVMR>();

            using (var db = DbConexion.Create())
            {
                var query = (from p in db.Paciente
                             join i in db.Ingreso on p.id equals i.pacienteId
                             where
                                !p.borrado
                                && !i.borrado
                                && !db.Egreso.Any(e => !e.borrado && e.ingresoId == i.id)
                             select new PacienteIngresoVMR
                             {
                                 pacienteId = p.id,
                                 ingresoId = i.id,
                                 cedula = p.cedula,
                                 nombreYApellidos = p.nombre + " " + p.apellidoPaterno + (p.apellidoMaterno == null ? "" : (" " + p.apellidoMaterno)),
                                 fechaIngreso = i.fecha
                             }
                            );

                /*var query2 = db.Ingreso.Where(i => 
                    !i.borrado 
                    && !i.Egreso.Any(e => !e.borrado) 
                     //!db.Egreso.Any(e => !e.borrado && e.ingresoId == i.id)
                ).Select(i => new PacienteIngresoVMR {
                    pacienteId = i.Paciente.id,
                    ingresoId = i.id,
                    cedula = i.Paciente.cedula,
                    nombreYApellidos = i.Paciente.nombre + " " + i.Paciente.apellidoPaterno + (i.Paciente.apellidoMaterno == null ? "" : (" " + i.Paciente.apellidoMaterno)),
                    fechaIngreso = i.fecha
                });*/
                    
                 

                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(p => p.cedula.Contains(textoBusqueda) || p.nombreYApellidos.Contains(textoBusqueda));
                }

                result.cantidadTotal= query.Count();

                result.elementos = query
                    .OrderBy(p => p.pacienteId)
                    .Skip(cantidad * pagina)
                    .Take(cantidad)
                    .ToList();
            }

            return result;
        }
    }
}
