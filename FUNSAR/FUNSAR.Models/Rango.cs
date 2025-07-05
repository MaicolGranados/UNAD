using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FUNSAR.Models
{
    public class Rango
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El rango es obligatorio")]
        [Display(Name = "Rango")]
        public string RangoNombre { get; set; }
        
    }
}
