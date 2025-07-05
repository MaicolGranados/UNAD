using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.Models.ViewModels
{
    public class CertificadoVM
    {
        public Certificado? Certificado { get; set; }
        public CertificadoTemp? CertificadoTemp { get; set; }
        public IEnumerable<SelectListItem>? ListaBrigadas { get; set; }
        public IEnumerable<SelectListItem>? ListaProceso { get; set; }
        public IEnumerable<SelectListItem>? tipoDoc { get; set; }
        public IEnumerable<SelectListItem>? semestre { get; set; }
        public IFormFile? file { get; set; }
    }
}
