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
    public class Brigada
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre de la brigada es obligatorio")]
        [Display(Name = "Nombre de la brigada")]
        public string Nombre { get; set; }
        public int EstadoBrigadaId { get; set; }
        public DateTime fechaActualizacion { get; set; }
    }
}
