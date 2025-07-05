using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FUNSAR.Models
{
    public class Asistencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VoluntarioId { get; set; }

        [ForeignKey("VoluntarioId")]
        public Voluntario? Voluntario { get; set; }

        [Required(ErrorMessage = "La fecha de asistencia es obligatoria")]
        [Display(Name = "Fecha asistencia")]
        public DateTime Fecha { get; set; }

        [Required]
        public int EstadoAsistenciaId { get; set; }

        [ForeignKey("EstadoAsistenciaId")]
        public EstadoAsistencia? Estado { get; set; }
        
    }
}
