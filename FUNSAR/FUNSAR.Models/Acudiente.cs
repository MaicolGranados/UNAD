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
    public class Acudiente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre Acudiente")]
        public string? nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [Display(Name = "Apellido Acudiente")]
        public string? apellido { get; set; }

        [Required]
        public int DocumentoId { get; set; }
        [ForeignKey("DocumentoId")]
        public tipoDocumento? TDocumento { get; set; }

        [Required(ErrorMessage = "El documento es obligatorio")]
        [Display(Name = "Numero de Documento")]
        public string? Documento { get; set; }

        [Required(ErrorMessage = "El numero de contacto es obligatorio")]
        [Display(Name = "Numero contacto")]
        public string? telefono { get; set; }

        [Required(ErrorMessage = "El correo obligatorio")]
        [Display(Name = "Correo")]
        public string? correo { get; set; }

        [Required(ErrorMessage = "El parentesco es obligatorio")]
        [Display(Name = "Parentesco")]
        public string? parentesco { get; set; }

        public int? VoluntarioId { get; set; }
        [ForeignKey("VoluntarioId")]
        public Voluntario? Voluntario { get; set; }

    }
}
