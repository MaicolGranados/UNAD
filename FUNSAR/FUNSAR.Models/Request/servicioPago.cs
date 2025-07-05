using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.Models.Request
{
    public class servicioPago
    {
        public int id { get; set; }
        public string? nombreR { get; set; }
        public string? apellidoR { get; set; }
        public string? tipoPersona { get ; set; }
        public string? tipoDocumento { get; set; }
        public string? documentoR { get; set; }
        public string? correoR { get; set; }
        public string? celularR { get; set; }
        public string? direccionR { get; set; }
        public string? banco { get; set; }
        public string? documentoP { get; set ; }
    }
}
