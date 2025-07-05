using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Ingrese un Nombre para la categoria")]
        [Display(Name = "Nombre Categoría")]
        public string Nombre { get; set; }
        public int? Orden { get; set; }
    }
}
