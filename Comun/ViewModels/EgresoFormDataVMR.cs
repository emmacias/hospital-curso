using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public class EgresoFormDataVMR
    {
        public List<IngresoVMR> IngresoList { get; set; }
        public List<MedicoVMR> MedicoList { get; set; }

        public EgresoFormDataVMR()
        {
            IngresoList = new List<IngresoVMR>();
            MedicoList = new List<MedicoVMR>();
        }
    }
}
