using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.Models.ViewModels
{
    public class ReporteVM
    {
        public IEnumerable<SelectListItem>? ListaReporte { get; set; }
        public string? ReporteSeleccionado { get; set; }
        public DateTime? fechaIni { get; set; }
        public DateTime? fechaFin { get; set; }

        //Campos para proceso estudiante
        public bool asistenciaDia { get; set; }
        public bool salida { get; set; }
        public bool pfe { get; set; }
        public bool pago { get; set; }

        //Campos para pago
        public bool responsablePago { get; set; }
        public bool rTipoDoc { get; set; }
        public bool rDocumento { get; set; }
        public bool rCorreo { get; set; }
        public bool rNombre { get; set; }
        public bool rApellidos { get; set; }
        public bool rBanco { get; set; }
        public bool acudiente { get; set; }
        public bool aTipoDoc { get; set; }
        public bool aDocumento { get; set; }
        public bool aCorreo { get; set; }
        public bool aCelular { get; set; }
        public bool aNombre { get; set; }
        public bool aApellidos { get; set; }

        //Filtro por brigada
        public Brigada brigada { get; set; }
        public IEnumerable<SelectListItem> listaBrigada { get; set; }
        public bool filtraBrigada { get; set; }
        public int? error { get; set; }
    }
}
