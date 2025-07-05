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
    public class CertificadoTemp
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
        public int BrigadaId { get; set; }
        [ForeignKey("BrigadaId")]
        public Brigada? Brigada { get; set; }

        public string? codCertificado { get; set; }

        [Required(ErrorMessage = "El año es obligatorio")]
        [Display(Name = "Año proceso")]
        public string? AnoProceso { get; set; }

        [Required]
        public int SemestreId { get; set; }
        [ForeignKey("SemestreId")]
        public Semestre? Semestre { get; set; }
        public string? FechaExpedicion { get; set; }
        [Required]
        public int EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public EstadoPersona? estadoPersona { get; set; }

        [Required]
        public int ProcesoId { get; set; }
        [ForeignKey("ProcesoId")]
        public Proceso? proceso { get; set; }

    }
}
