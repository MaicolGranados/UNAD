using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.Models.ViewModels
{
    public class ArticuloVM
    {
        public Articulo? Articulo { get; set; }
        public Categoria? Categoria { get; set; }
        public AsistenteSalida? AsistenteSalida { get; set; }
        public IEnumerable<SelectListItem>? ListaTipoDocumento { get; set; }
        public IEnumerable<SelectListItem>? ListaCategorias { get; set; }
        public IEnumerable<SelectListItem>? ListaServicio { get; set; }
        public IEnumerable<SelectListItem>? ListaBrigadas { get; set; }
        public IEnumerable<SelectListItem>? ListaArticulo { get; set; }
        public tDocumento SelectTdocumento { get; set; }
        public string? Documento { get; set; }
        public int voluntario { get; set; }
    }

}
