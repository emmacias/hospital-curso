using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public class PacienteIngresoVMR
    {
        public long pacienteId { get; set; }
        public long ingresoId { get; set; }
        public string cedula { get; set; }
        public string nombreYApellidos { get; set; }
        public DateTime fechaIngreso { get; set; }
    }
}
