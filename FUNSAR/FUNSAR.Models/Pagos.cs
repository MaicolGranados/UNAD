using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace FUNSAR.Models
{
    public class Pagos
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Estado { get; set; }

        [Required]
        public string? Secuencia { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fechapago")]
        public string? Fechapago { get; set; }

        [Required(ErrorMessage = "El documento del participante es obligatorio")]
        [Display(Name = "Numero de documento participante")]
        public string? DocumentoP { get; set; }

        [Required(ErrorMessage = "El tipo de documento es obligatorio")]
        public int DocumentoIdR { get; set; }
        [ForeignKey("DocumentoIdR")]
        public tipoDocumento? TDocumentoR { get; set; }

        [Required(ErrorMessage = "El documento del responsable de pago es obligatorio")]
        [Display(Name = "Numero de documento responsable pago")]
        public string? DocumentoR { get; set; }

        [Required(ErrorMessage = "El documento es obligatorio")]
        public int ServicioId { get; set; }
        [ForeignKey("ServicioId")]
        public Servicio? Servicio { get; set; }

        [Required(ErrorMessage = "El correo del responsable de pago es obligatorio")]
        [Display(Name = "Correo responsable pago")]
        [EmailValidation(ErrorMessage = "Por favor, introduce un correo electrónico válido.")]
        public string? CorreoR { get; set; }

        [Required(ErrorMessage = "El celular del responsable de pago es obligatorio")]
        [Display(Name = "Celular responsable pago")]
        public string? CelularR { get; set; }

        [Required(ErrorMessage = "El nombre del responsable de pago es obligatorio")]
        [Display(Name = "Nombres responsable pago")]
        public string? NombreR { get; set; }

        [Required(ErrorMessage = "El apellido del responsable de pago es obligatorio")]
        [Display(Name = "Apellidos responsable pago")]
        public string? ApellidoR { get; set; }

        [Required(ErrorMessage = "La dirección del responsable de pago es obligatorio")]
        [Display(Name = "Dirección residencia responsable pago")]
        public string? DireccionR { get; set; }

        [Required(ErrorMessage = "El Banco es obligatorio")]
        [Display(Name = "Seleccion Banco")]
        public string? Banco { get; set; }

        [Required(ErrorMessage = "El tipo persona es obligatorio")]
        [Display(Name = "Tipo persona responsable pago")]
        public string? TipoPersona { get; set; }

        public double? Valor { get; set; }

    }

    public class EmailValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string email = Convert.ToString(value);

            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Expresión regular para validar el formato de correo electrónico
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, emailPattern);
        }
    }
    
}
