using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace FUNSAR.HerramientasComunes
{
    public class Correo
    {
        public async Task EnvioGmail(string de, string para, string asunto, string cuerpo)
        {
			try
			{
				using (MailMessage mail = new MailMessage())
				{

					mail.To.Add(para);
					//mail.Bcc.Add("representacion.institucional@funsar.org.co");
					mail.Subject = asunto;

					mail.Body = cuerpo;
					mail.IsBodyHtml = true;

					mail.From = new MailAddress("no-reply@funsar.org.co", de);

					using (SmtpClient smtp = new SmtpClient())
					{
						smtp.UseDefaultCredentials = false;
						smtp.Credentials = new NetworkCredential("no-reply@funsar.org.co", "ysji wqwg ocwo rnvc");
						smtp.Port = 587;
						smtp.EnableSsl = true;
						smtp.Host = "smtp.gmail.com";
						await smtp.SendMailAsync(mail);
					}

				}
			}
			catch (Exception)
			{

				throw;
			}
        }

        public async Task EnvioGmailAdjunto(string de, string para, string asunto, string cuerpo, string path)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {

                    mail.To.Add(para);
                    //mail.Bcc.Add("representacion.institucional@funsar.org.co");
                    mail.Subject = asunto;
                    mail.Attachments.Add(new Attachment(path));

                    mail.Body = cuerpo;
                    mail.IsBodyHtml = true;

                    mail.From = new MailAddress(de, "Fundación FUNSAR");

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("no-reply@funsar.org.co", "ysji wqwg ocwo rnvc");
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(mail);
                    }

                }

                File.Delete(path);

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
