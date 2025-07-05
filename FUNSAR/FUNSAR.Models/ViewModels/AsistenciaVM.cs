using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.Models.ViewModels
{
    public class AsistenciaVM
    {
        public Voluntario? Voluntario { get; set; }
        public Asistencia? Asistencia { get; set; }
        public EstadoAsistencia? EstadoAsistencia { get; set; }
        public IEnumerable<SelectListItem>? ListaEstadoAsistencia { get; set; }

    }

}
