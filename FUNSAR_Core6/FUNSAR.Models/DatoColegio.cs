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
    public class DatoColegio
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del encargado es obligatorio")]
        [Display(Name = "Nombre del encargado")]
        public string? NombreEncargado { get; set; }

        [Required(ErrorMessage = "El correo del encargado es obligatorio")]
        [Display(Name = "Correo del encargado")]
        public string? Correo { get; set; }

        [Required(ErrorMessage = "El numero del encargado es obligatorio")]
        [Display(Name = "Numero de contacto")]
        public string? NumeroContacto { get; set; }

        [Required(ErrorMessage = "El cargo del encargado es obligatorio")]
        [Display(Name = "Cargo del encargado")]
        public string? Cargo { get; set; }

        [Required(ErrorMessage = "La jornada del colegio es obligatoria")]
        [Display(Name = "Jornada del colegio")]
        public int JornadaId { get; set; }
        [ForeignKey("JornadaId")]
        public JornadaColegio? Jornada { get; set; }

        [Required]
        public int ColegioId { get; set; }
        [ForeignKey("ColegioId")]
        public Colegio? Colegio { get; set; }
    }
}
