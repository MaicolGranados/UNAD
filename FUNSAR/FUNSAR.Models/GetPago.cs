using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.Models
{
    public class GetPago
    {
        public string? id { get; set; }
        public string? date_created { get; set; }
        public string? date_approved { get; set; }
        public string? paymentMethod { get; set; }
        public string? payment_method_id { get; set; }
        public string? status { get; set; }
        public string? description { get; set; }
        public payer? payer { get; set; }
        public string? validate { get; set; }

    }

    public class payer
    {
        public string? id { get; set; }
        public string? email { get; set; }
        public identification? identification { get; set; }
        public phone? phone { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
    }

    public class identification
    { 
        public string? type { get; set; }
        public string? number { get; set; }
    }
    public class phone
    {
        public string? number { get; set; }
    }

}
