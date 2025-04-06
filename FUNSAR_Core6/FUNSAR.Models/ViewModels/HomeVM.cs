using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace FUNSAR.Models.ViewModels
{
    public class HomeVM
    {
        //public tipoDocumento tipoDocumento { get; set; }
        public Certificado? certificado { get; set; }
        public VoluntarioVM? voluntarioVM { get; set; }
        public Voluntario? voluntario { get; set; }
        public IEnumerable<Slider>? Slider { get; set; }
        public IEnumerable<Articulo>? ListaArticulos { get; set; }
        public IEnumerable<Categoria>? ListaCategorias { get; set; }
        //public IEnumerable<SelectListItem> tipoDoc { get; set; }
        public IEnumerable<Certificado>? Certificado { get; set; }

        public tDocumento SelectTdocumento { get; set; }

        public string? resultado { get; set; }
        public string? DetallePfe { get; set; }

        public PFE? PFE { get; set; }

        [Display(Name = "Archivo PDF")]
        public IFormFile? filePFE { get; set; }

    }
    public enum tDocumento
    {
        CC,TI,CE,NUIP,NIT,PEP,NU,TP,PPT,Otro
    }

}
