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
    public class Voluntario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre persona")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [Display(Name = "Apellido persona")]
        public string? Apellido { get; set; }

        [Required]
        public int DocumentoId { get; set; }
        [ForeignKey("DocumentoId")]
        public tipoDocumento? TDocumento { get; set; }

        [Required(ErrorMessage = "El documento es obligatorio")]
        [Display(Name = "Numero de Documento")]
        public string? Documento { get; set; }
        
        [Required]
        public int ColegioId { get; set; }
        [ForeignKey("ColegioId")]
        public Colegio? Colegio { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [Display(Name = "Correo Personal")]
        public string? correo { get; set; }

        [Required(ErrorMessage = "El numero de contacto es obligatorio")]
        [Display(Name = "Numero contacto")]
        public string? telefono { get; set; }

        [Required]
        public int EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public EstadoPersona? estadoPersona { get; set; }

        [Required]
        public int ProcesoId { get; set; }
        [ForeignKey("ProcesoId")]
        public Proceso? proceso { get; set; }

        [Required]
        public int JornadaId { get; set; }
        [ForeignKey("JornadaId")]
        public JornadaColegio? Jornada { get; set; }
        
        public DateTime? FechaRegistro { get; set; }
    }
}
