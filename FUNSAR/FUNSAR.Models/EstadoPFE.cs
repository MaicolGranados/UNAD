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
    public class EstadoPFE
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Detalle PFE")]
        public string? Detalle { get; set; }

        

    }
}
