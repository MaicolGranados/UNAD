using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.Models.ViewModels
{
    public class ColegioVM
    {
        public Colegio? Colegio { get; set; }
        public DatoColegio? DatoColegio { get; set; }
        public IEnumerable<SelectListItem>? ListaBrigadas { get; set; }
        public IEnumerable<SelectListItem>? ListaJornada { get; set; }
    }
}
