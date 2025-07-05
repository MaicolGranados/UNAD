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
    public class EstadoBrigada
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El estado de la brigada es obligatorio")]
        [Display(Name = "Estado de la brigada")]
        public string Estado { get; set; }
        
    }
}
