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
    public class EgresoBLL
    {
        /// <summary>
        /// Método para obtener todos los items
        /// </summary>
        /// <param name="cantidad">Cantidad de elementos a obtener</param>
        /// <param name="pagina">Número de página de resultados</param>
        /// <param name="textoBusqueda">Texto para filtrar la búsqueda</param>
        public static ListadoPaginadoVMR<EgresoVMR> Gets(int cantidad, int pagina, string textoBusqueda)
        {
            return EgresoDAL.Gets(cantidad, pagina, textoBusqueda);
        }

        /// <summary>
        /// Método para obtener item por su identificador
        /// </summary>
        /// <param name="id">Identificador del item que se desea obtener</param>
        public static EgresoVMR Get(long id)
        {
            return EgresoDAL.Get(id);
        }

        /// <summary>
        /// Método para crear un nuevo item y retornar su identificador
        /// </summary>
        /// <param name="item">Datos del nuevo item</param>
        public static long Post(Egreso item)
        {
            return EgresoDAL.Post(item);
        }

        /// <summary>
        /// Método para editar un item
        /// </summary>
        /// <param name="item">Datos del item a editar</param>
        public static void Put(EgresoVMR item)
        {
            EgresoDAL.Put(item);
        }

        /// <summary>
        /// Método para borrar varios items por sus identificadores
        /// </summary>
        /// <param name="ids">Identificadores de los items que se desean borrar</param>
        public static void Delete(List<long> ids)
        {
            EgresoDAL.Delete(ids);
        }

        /// <summary>
        /// Método para comprobar la existencia de un item por su identificador
        /// </summary>
        /// <param name="id">Identificador del item que se desea comprobar su existencia</param>
        public static bool ItemExists(long id)
        {
            return EgresoDAL.ItemExists(id);
        }

        /// <summary>
        /// Método para obtener los datos necesarios para el formulario
        /// </summary>
        /// <param name="id">Identificador del item (0 cuando el formulario es de creación)</param>
        /// <returns></returns>
        public static EgresoFormDataVMR GetFormData(long id)
        {
            return EgresoDAL.GetFormData(id);
        }
    }
}
