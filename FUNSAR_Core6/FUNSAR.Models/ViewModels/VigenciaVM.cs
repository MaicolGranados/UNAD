using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.Models.ViewModels
{
    public class VigenciaVM
    {
        public VigenciaServicio? VigenciaServicio { get; set; }
        public IEnumerable<SelectListItem>? ListaServicio { get; set; }
    }

}
