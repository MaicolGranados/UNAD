using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.Models.Reportes
{
    public class ProcesoParticipante
    {
        public Models.Pagos? datoPagos { get; set; }
        public Models.Voluntario? datoParticipante { get; set; }
        public Models.PFE? datoPfe { get; set; }
        public List<string>? salidas { get; set; }
        public List<string>? fechaAsistencia { get; set; }
        public List<string>? estadoAsistencia { get; set; }
    }
}
