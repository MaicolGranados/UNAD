using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.Models.ViewModels
{
    public class VoluntarioVM
    {
        public Voluntario? Voluntario { get; set; }
        public Acudiente? Acudiente { get; set; }
        public Brigada? Brigada { get; set; }
        public Proceso? Proceso { get; set; }
        public Colegio? Colegio { get; set; }
        public PFE? PFE { get; set; }
        public JornadaColegio? JornadaColegio { get; set; }
        public IEnumerable<SelectListItem>? ListaColegio { get; set; }
        public IEnumerable<SelectListItem>? ListaBrigada { get; set; }
        public IEnumerable<SelectListItem>? ListaTipoDocumento { get; set; }
        public IEnumerable<SelectListItem>? ListaEstado { get; set; }
        public IEnumerable<SelectListItem>? ListaEstadoPFE { get; set; }
        public IEnumerable<SelectListItem>? ListaProceso { get; set; }
        public IEnumerable<SelectListItem>? ListaJornada { get; set; }
        public string? valida { get; set; }

    }

}
