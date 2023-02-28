using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public class ListadoPaginadoVMR<T>
    {
        /// <summary> 
        /// Cantidad total de elementos
        /// </summary> 
        public int cantidadTotal { get; set; }

        /// <summary> 
        /// Elementos devueltos para la página solicitada
        /// </summary> 
        public IEnumerable<T> elementos { get; set; }

        public ListadoPaginadoVMR()
        {
            cantidadTotal = 0;
            elementos = new List<T>();
        }
    }
}
