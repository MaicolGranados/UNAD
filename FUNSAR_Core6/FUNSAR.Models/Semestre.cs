using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.Models
{
    public class Semestre
    {
        [Key]
        public int Id { get; set; }
        public string semestre { get; set; }
    }
}
