using System;
using System.Configuration;
using System.Net.Mail;

namespace PagosGranChapur.Services.Helpers
{

    public static class MailService
    {
        //Método que envia un correo electrónico con la configuración establecida en el Web.config
        public static void SendMessage(string emailClient, string htmlBody, string subject) {

            try
            {
                htmlBody = htmlBody.Replace("[detalle]", ConfigurationManager.AppSettings[$"API.Email.Detail.{subject}"]);
                htmlBody = htmlBody.Replace("[subTitle]", ConfigurationManager.AppSettings[$"API.Email.SubTitle.{subject}"]);


                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient();

                mail.From = new MailAddress(ConfigurationManager.AppSettings["API.Email.From"],"Tiendas GRAN CHAPUR");
                mail.To.Add(emailClient);

                mail.Subject    = ConfigurationManager.AppSettings[$"API.Email.Subject.{subject}"];
                mail.Body       = htmlBody;
                mail.IsBodyHtml = true;

                //smtpServer.UseDefaultCredentials = false    ;    
                smtpServer.Host        = ConfigurationManager.AppSettings["API.Email.Servidor.SMTP"];
                smtpServer.Port        = int.Parse(ConfigurationManager.AppSettings["API.Email.Servidor.Port"].ToString());
                smtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["API.Email.Usuario"], ConfigurationManager.AppSettings["API.Email.Password"]);
                smtpServer.EnableSsl   = true;
                
                smtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                throw new Exception( $"Error al intentar de mandar el correo electrónico al correo {emailClient}: {ex.Message}");
            }           

        }
    }
}
