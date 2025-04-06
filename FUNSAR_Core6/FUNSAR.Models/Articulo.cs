using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.Models
{
    public class Articulo
    {
        [Key]
        public int Id { get; set; }
        
        [Display(Name = "Nombre del articulo")]
        public string? Nombre { get; set; }
        [Display(Name = "Descripcion del articulo")]
        public string? Descripcion { get; set; }
        [Display(Name = "Fecha de Creación")]
        public string? FechaCreacion { get; set; }
        [DataType(DataType.ImageUrl)]
        [Display(Name ="Imagen")]
        public string? UrlImagen { get; set; }
        [DataType(DataType.Upload)]
        [Display(Name = "Documento")]
        public string? UrlDocumento { get; set; }
        [Required]
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public Categoria? Categoria { get; set; }
        public Servicio? Servicio { get; set; }
        public int ServicioId { get; set; }
        public string? BrigadaId { get; set; }
        [Required]
        [Display(Name = "Activo")]
        public bool Activo { get; set; }

    }
}
