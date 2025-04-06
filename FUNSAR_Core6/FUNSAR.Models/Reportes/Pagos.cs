using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.Models.Reportes
{
    public class Pagos
    {
        public Models.Pagos? datoPagos { get; set; }
        public Models.Certificado? datoParticipante { get; set; }
        public Models.Acudiente? datoAcudiente { get; set; }
    }
}
