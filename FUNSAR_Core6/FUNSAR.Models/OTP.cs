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
    public class OTP
    {
        public string? id { get; set; }
        public string? Mail { get; set; }
        public string? Code { get; set; }

    }
}
