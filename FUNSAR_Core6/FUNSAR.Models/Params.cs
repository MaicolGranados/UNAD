using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.Models
{
    public class Params
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        [Display(Name = "Concepto Parametro")]
        public string? Concepto { get; set; }

        [Required(ErrorMessage = "El valor del parametro es obligatorio")]
        [Display(Name = "Valor Parametro")]
        public string? Valor { get; set; }
    }
}
