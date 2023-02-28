using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public class MedicosDeshabilitadosConIngresos
    {
        public long id { get; set; }
        public string cedula { get; set; }
        public string nombre { get; set; }
        public int cantidadPacientesIngresados { get; set; }
    }
}
