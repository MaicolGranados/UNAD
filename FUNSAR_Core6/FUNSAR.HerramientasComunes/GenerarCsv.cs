using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.HerramientasComunes
{
    public class GenerarCsv
    {
        public string Generar(string data)
        {
            string filename = DateTime.Today.ToString("ddMMyy");

            string uniqueIdentifier = Guid.NewGuid().ToString();

            string path = $"wwwroot/Files/reports/report_{filename}_{uniqueIdentifier}.csv";

            using (StreamWriter wr = new StreamWriter(path))
            {
                wr.WriteLine(data);
            }
            return path;
        }
    }
}
