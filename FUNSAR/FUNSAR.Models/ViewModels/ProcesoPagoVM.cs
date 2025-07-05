using FUNSAR.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.Models.ViewModels
{
    public class ProcesoPagoVM
    {
        public Pagos pagos { get; set; }
        public Servicio Servicio { get; set; }
        public IEnumerable<SelectListItem>? ListaTipoDoc { get; set; }
    }
}
