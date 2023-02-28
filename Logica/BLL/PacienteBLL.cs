using Comun.ViewModels;
using Datos.DAL;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.BLL
{
    /// <summary>
    /// Clase de lógica de negocio
    /// </summary>
    public class PacienteBLL
    {
        /// <summary>
        /// Método para obtener todos los items
        /// </summary>
        /// <param name="cantidad">Cantidad de elementos a obtener</param>
        /// <param name="pagina">Número de página de resultados</param>
        /// <param name="textoBusqueda">Texto para filtrar la búsqueda</param>
        public static ListadoPaginadoVMR<PacienteVMR> Gets(int cantidad, int pagina, string textoBusqueda)
        {
            return PacienteDAL.Gets(cantidad, pagina, textoBusqueda);
        }

        /// <summary>
        /// Método para obtener item por su identificador
        /// </summary>
        /// <param name="id">Identificador del item que se desea obtener</param>
        public static PacienteVMR Get(long id)
        {
            return PacienteDAL.Get(id);
        }

        /// <summary>
        /// Método para crear un nuevo item y retornar su identificador
        /// </summary>
        /// <param name="item">Datos del nuevo item</param>
        public static long Post(Paciente item)
        {
            return PacienteDAL.Post(item);
        }

        /// <summary>
        /// Método para editar un item
        /// </summary>
        /// <param name="item">Datos del item a editar</param>
        public static void Put(PacienteVMR item)
        {
            PacienteDAL.Put(item);
        }

        /// <summary>
        /// Método para borrar varios items por sus identificadores
        /// </summary>
        /// <param name="ids">Identificadores de los items que se desean borrar</param>
        public static void Delete(List<long> ids)
        {
            PacienteDAL.Delete(ids);
        }

        /// <summary>
        /// Método para comprobar la existencia de un item por su identificador
        /// </summary>
        /// <param name="id">Identificador del item que se desea comprobar su existencia</param>
        public static bool ItemExists(long id)
        {
            return PacienteDAL.ItemExists(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cantidad"></param>
        /// <param name="pagina"></param>
        /// <param name="textoBusqueda"></param>
        /// <returns></returns>
        public static ListadoPaginadoVMR<PacienteIngresoVMR> PacientesIngresados(
            int cantidad,
            int pagina,
            string textoBusqueda
        )
        {
            return PacienteDAL.PacientesIngresados(cantidad, pagina, textoBusqueda);
        }
    }
}
