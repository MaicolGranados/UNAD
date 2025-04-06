using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FUNSAR.Models
{
    public class Colegio
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del colegio es obligatorio")]
        [Display(Name = "Nombre del colegio")]
        public string? Nombre { get; set; }
        [Required]
        public int BrigadaId { get; set; }
        [ForeignKey("BrigadaId")]
        public Brigada? Brigada { get; set; }
    }
}
