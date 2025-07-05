
using System.Runtime.CompilerServices;

namespace FUNSAR.HerramientasComunes
{
    public class Log
    {
        public async Task escribirLog (string tipo, string mensaje, string clase)
        {
            string date = DateTime.Now.ToString();
            string path = "wwwroot/Logs";
            string file = "Log" + tipo + date.Replace("/", "").Replace(" ", "").Replace(":","").Substring(0,14) + ".txt";
            string fullpath = Path.Combine(path, file);
            string asunto = "";
            try
            {
                using (StreamWriter wr = new StreamWriter(fullpath,true))
                {
                    wr.WriteLine(date);
                    wr.WriteLine("CLASE: " + clase);
                    wr.WriteLine("MENSAJE: " + mensaje);
                    wr.WriteLine("");
                    wr.Close();
                }
                string cuerpoMail = string.Format("{0} {1} {2} {3}",
                    tipo + "<p> Pagina web: ",
                    DateTime.Now.ToString(),
                    mensaje,
                    clase);
                Correo correo = new Correo();
                if (tipo.Equals("INFO"))
                {
                    asunto = "INFORMACION PAGINA WEB";
                }
                else
                {
                    asunto = "ERROR PAGINA WEB";
                }
                await correo.EnvioGmail("no-reply", "desarrollo.web@funsar.org.co", asunto, cuerpoMail);

            }
            catch (Exception ex)
            {
                string cuerpoMail = "<p>ERROR LOGS:" + ex.ToString() ;
                Correo correo = new Correo();
                await correo.EnvioGmail("no-reply","desarrollo.web@funsar.org.co","ERROR CRITICO PAGINA WEB",cuerpoMail);

            }
            
        }
    }
}
