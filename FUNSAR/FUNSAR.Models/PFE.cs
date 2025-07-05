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
    public class PFE
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El Detalle del PFE es obligatorio")]
        [Display(Name = "Detalle PFE")]
        public string? Detalle { get; set; }

        public int EstadoPFEId { get; set; }
        [ForeignKey("EstadoPFEId")]
        public EstadoPFE? EstadoPFE { get; set; }

        [Required]
        public int? VoluntarioId { get; set; }
        [ForeignKey("VoluntarioId")]
        public Voluntario? Voluntario { get; set; }

    }
}
