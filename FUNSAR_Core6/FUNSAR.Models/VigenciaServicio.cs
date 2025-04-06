using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.Models
{
    public class VigenciaServicio
    {
        [Key]
        public int Id { get; set; }
        public int? ServicioId { get; set; }
        [ForeignKey("ServicioId")]
        public Servicio? Servicio { get; set; }
        public string? Vigencia {get; set;}
        public string? Valor { get;set;}
    }
}
