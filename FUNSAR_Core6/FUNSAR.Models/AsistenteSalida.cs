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
    public class AsistenteSalida
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

        [Required(ErrorMessage = "El correo es obligatorio")]
        [Display(Name = "Correo Personal")]
        public string? correo { get; set; }

        [Required(ErrorMessage = "El numero de contacto es obligatorio")]
        [Display(Name = "Numero contacto")]
        public string? telefono { get; set; }
        public DateTime? FechaRegistro { get; set; }

        [Required]
        public int ServicioId { get; set; }
        [ForeignKey("ServicioId")]
        public Servicio? Servicio { get; set; }
        public string? EstadoPago { get; set; }
        public string? ReferenciaPago { get; set; }
    }
}
