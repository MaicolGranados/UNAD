using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El documento es obligatorio")]
        public string Documento { get; set; }

        public int RangoId { get; set; }
        [ForeignKey("RangoId")]
        public Rango? rango { get; set; }

        public int BrigadaId { get; set; }
        [ForeignKey("BrigadaId")]
        public Brigada? brigada { get; set; }
    }
}
