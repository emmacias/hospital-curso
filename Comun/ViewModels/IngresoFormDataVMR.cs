using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public class IngresoFormDataVMR
    {
        public int numeroSala { get; set; }
        public int numeroCama { get; set; }

        public List<PacienteVMR> PacienteList { get; set; }
        public List<MedicoVMR> MedicoList { get; set; }

        public IngresoFormDataVMR()
        {
            PacienteList = new List<PacienteVMR>();
            MedicoList = new List<MedicoVMR>();
        }
    }
}
