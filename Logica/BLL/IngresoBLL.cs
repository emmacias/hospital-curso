using Comun.ViewModels;
using Datos.DAL;
using Modelo.Modelos;
using Rest.REST;
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
    public class IngresoBLL
    {
        /// <summary>
        /// Método para obtener todos los items
        /// </summary>
        /// <param name="cantidad">Cantidad de elementos a obtener</param>
        /// <param name="pagina">Número de página de resultados</param>
        /// <param name="textoBusqueda">Texto para filtrar la búsqueda</param>
        public static ListadoPaginadoVMR<IngresoVMR> Gets(int cantidad, int pagina, string textoBusqueda)
        {
            return IngresoDAL.Gets(cantidad, pagina, textoBusqueda);
        }

        /// <summary>
        /// Método para obtener item por su identificador
        /// </summary>
        /// <param name="id">Identificador del item que se desea obtener</param>
        public static IngresoVMR Get(long id)
        {
            return IngresoDAL.Get(id);
        }

        /// <summary>
        /// Método para crear un nuevo item y retornar su identificador
        /// </summary>
        /// <param name="item">Datos del nuevo item</param>
        public static long Post(Ingreso item)
        {
            return IngresoDAL.Post(item);
        }

        /// <summary>
        /// Método para editar un item
        /// </summary>
        /// <param name="item">Datos del item a editar</param>
        public static void Put(IngresoVMR item)
        {
            IngresoDAL.Put(item);
        }

        /// <summary>
        /// Método para borrar varios items por sus identificadores
        /// </summary>
        /// <param name="ids">Identificadores de los items que se desean borrar</param>
        public static void Delete(List<long> ids)
        {
            IngresoDAL.Delete(ids);
        }

        /// <summary>
        /// Método para comprobar la existencia de un item por su identificador
        /// </summary>
        /// <param name="id">Identificador del item que se desea comprobar su existencia</param>
        public static bool ItemExists(long id)
        {
            return IngresoDAL.ItemExists(id);
        }

        /// <summary>
        /// Método para obtener los datos necesarios para el formulario
        /// </summary>
        /// <param name="id">Identificador del item (0 cuando el formulario es de creación)</param>
        /// <returns></returns>
        public static IngresoFormDataVMR GetFormData(long id)
        {
            var formData = IngresoDAL.GetFormData(id);

            if (id == 0)
            {
                var salaCama = IngresoRES.ObtenerSalaYCama();

                formData.numeroSala = salaCama.sala;
                formData.numeroCama = salaCama.cama;
            }

            return formData;
        }
    }
}
