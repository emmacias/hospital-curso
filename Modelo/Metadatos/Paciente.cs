using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Modelos
{
    [MetadataType(typeof(PacienteMetadata))]
    public partial class Paciente
    {
    }

    public class PacienteMetadata
    {
        [Required]
        [StringLength(10)]
        public string cedula { get; set; }
        [Required]
        [StringLength(50)]
        public string nombre { get; set; }
        [Required]
        [StringLength(50)]
        public string apellidoPaterno { get; set; }
        [StringLength(50)]
        public string apellidoMaterno { get; set; }
        [Required]
        [StringLength(10)]
        public string celular { get; set; }
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string correoElectronico { get; set; }
        [Required]
        [StringLength(500)]
        public string direccion { get; set; }
    }
}