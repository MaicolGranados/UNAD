using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FUNSAR.Models
{
    public class tipoDocumento
    {
        [Key]
        public int Id { get; set; }
        public string tDocumento { get; set; }
    }
}
