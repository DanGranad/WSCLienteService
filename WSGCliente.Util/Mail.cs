using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Util
{
    public class Message
    {
        public string Address { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class Mail
    {

       /* public  void EnviarEmail(string Correo)
        {
            // InsertaCore InsertaCore = new InsertaCore();
            string mstr_presentacion = ConfigurationManager.AppSettings["wstr_Presentacion"];
            string mstr_Body = ConfigurationManager.AppSettings["wstr_Body"];
            string mstr_Body2 = ConfigurationManager.AppSettings["wstr_Body2"];
            //string mstr_Titulo = ConfigurationManager.AppSettings["wstr_Titulo"];
            string mstr_DisplayName = ConfigurationManager.AppSettings["wstr_DisplayName"];
            //string mstr_Firma = "";
            string mstr_Email = ConfigurationManager.AppSettings["wstr_Email"];
            string mstr_EmailPassw = ConfigurationManager.AppSettings["wstr_EmailPassw"];
            string mstr_SmtpClient = ConfigurationManager.AppSettings["wstr_SmtpClient"];
            string mstr_SmtpPort = ConfigurationManager.AppSettings["wstr_SmtpPort"];
            //string mstr_Body_Excel = ConfigurationManager.AppSettings["wstr_Body_Excel"];

            //ResponseViewModel responseCorreo = new ResponseViewModel();
            //responseCorreo = InsertaCore.DestinatarioEmail(NUSERCODE);

            System.Net.Mail.Attachment archivo = null;
            try
            {
                //Configuración del Mensaje
                string html =
           "<div style='background-color:bgcolor=#FFFFFF; margin:0; padding:0; font-family: 'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif; -webkit-font-smoothing:antialiased;" +
           "-webkit-text-size-adjust:none;" +
           "width: 100%!important;" +
           "height: 100%;'>" +
               "<table class='head-wrap' style='width: 750px;background-color:#ff6e00'>" +
                   "<tr>" +
                   "<td></td>" +
                   "<td class='header container' style='display: block!important; max-width: 600px!important; margin: 0 auto!important; clear: both!important;'>" +

                       "<div class='content' style='height: 30px; max-width: 750pxs; margin: 0 auto; display: block;'>" +
                           "<table style='width: 100%;'>" +
                               "<tr>" +
                                   "<td style='text-align:right; align-content:center' >" +
                                       "<h2 class='collapse' style='padding-left: 0px; color: #FFF; margin: 0!important; text-align:center'>" + mstr_DisplayName + "</h2>" +
                                   "</td>" +
                               "</tr>" +
                           "</table>" +
                       "</div>" +

                   "</td>" +
                   "<td></td>" +
                   "</tr>" +
               "</table>" +
               "<table class='body-wrap' style='width: 750px;'>" +
                   "<tr>" +
                     "<td class='container' style='display: block!important; max-width: 600px!important; margin: 0 auto!important; clear: both!important; background-color: #FFFFFF'>" +
                       "<!-- content -->" +
                           "<div class='content' style='padding: 15px; max-width: 600px; margin: 0 auto; display: block;'>" +
                               "<table>" +
                                   "<tr>" +
                                       "<td>" +
                                          "<br/>" +
                                          "<b style='font-size: 16px' id='textPasiente'>  " + mstr_presentacion + "</b>" +
                                          "<br/>" +
                                          "<br/>" +
                                          "<span> " + mstr_Body +
                                          " </span>" +
                                          "<br/>" +
                                           mstr_Body2 +
                                          "<br />" +
                                          "<br />" +
                                          "Saludos Cordiales!" +
                                       "</td>" +
                                   "</tr>" +
                               "</table>" +
                           "</div>" +
                       "</td>" +
                   "</tr>" +
               "</table>" +
           "</div>";

                //Imagen*******************************************************************************************
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html, null, "text/html");
                //LinkedResource logo = new LinkedResource(urlLogo);
                //logo.ContentId = "companylogo";
                //htmlView.LinkedResources.Add(logo);
                //*************************************************************************************************

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(mstr_SmtpClient);
                //Especificamos el correo desde el que se enviará el Email y el nombre de la persona que lo envía
                mail.From = new MailAddress(mstr_Email, "Carga Masiva - Gestor de Clientes", Encoding.UTF8);
                mail.Subject = "Detalle de Carga Masiva";
                mail.AlternateViews.Add(htmlView); 

                //archivo.Dispose(); 
                mail.IsBodyHtml = true;

                mail.To.Add(Correo);

                

                //string[] destinatario = dtblparametros.Rows[0]["DES_CORREO_ENVIADO"].ToString().Split(';');
                //foreach (string destinos in destinatario)
                //{
                //    mail.To.Add(destinos);
                //}
                //Especificamos a quien enviaremos el Email, no es necesario que sea Gmail, puede ser cualquier otro proveedor
                //**********************Para enviar copias***********
                //string[] copias = vobj_Pedido.Demacop_.Split(',');
                //foreach (string item2 in copias)
                //{
                //    if (!item2.Equals(""))
                //    {
                //        mail.CC.Add(item2);
                //    }
                //}
                //****************************************************

                //Configuracion del SMTP
                int port = Convert.ToInt16(mstr_SmtpPort);
                SmtpServer.Port = port; //Puerto que utiliza Gmail para sus servicios
                                        //Especificamos las credenciales con las que enviaremos el mail
                SmtpServer.Credentials = new System.Net.NetworkCredential(mstr_Email, mstr_EmailPassw);
                SmtpServer.EnableSsl = true;
                //SmtpServer.UseDefaultCredentials = true;
                SmtpServer.Send(mail);
                //archivo.Dispose();

               

                //return "Correcto";
            }
            catch (Exception ex)
            {
                archivo.Dispose();
             
                //log.Info(string.Format("Estado Email: {0}", "No Enviado"));
                //  log.Info(string.Format("Error: {0}", ex.Message));
                throw ex;
            }
        }*/
            public List<Message> ComposeMail(string process, List<Config> mailList)
            {
                List<Message> list = new List<Message>();

                string subject = string.Empty;
                string keySubject = string.Empty;
                string body = string.Empty;
                string keyBody = string.Empty;
                string cliente = string.Empty;

                try
                {
                    foreach (Config item in mailList)
                    {
                        keySubject = "sLAFT";
                        keyBody = "bLAFT";

                            cliente = string.Format("{0} {1}, {2}", item.ApellidoPaterno, item.ApellidoMaterno, item.Nombres);
                            subject = ComposeSubject(keySubject,cliente);
                            body = ComposeBody(keyBody,cliente,item.Documento,item.aplicativo);
                        
                        list.Add(new Message
                        {
                            Address = item.Correo,
                            Subject = subject,
                            Body = body
                        });
                    }
                }
                catch (Exception ex)
                {
                   //  LogHelper.Exception(process, LogHelper.Paso.ComposeMail, ex.Message);
                }

                return list;
            }

            private string ComposeSubject(string key, string cliente)
            {
                return string.Format(GetValueConfig(key), cliente);
            }

            private string ComposeSubject(string key, string producto1, string nroPoliza1, string producto2, string nroPoliza2, string cliente, bool oneProduct)
            {
                string subject = string.Empty;

                key += oneProduct ? "1" : "2";

                if (oneProduct)
                {
                    subject = string.Format(GetValueConfig(key), producto1, nroPoliza1, cliente);
                }
                else
                {
                    subject = string.Format(GetValueConfig(key), producto1, nroPoliza1, producto2, nroPoliza2, cliente);
                }

                return subject;
            }

            private string ComposeBody(string key, string nroCotizacion, string producto1, string producto2, string usuario, bool oneProduct)
            {
                string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig(key));

                string productos = producto1 + (oneProduct ? string.Empty : " y " + producto2);

                string readText = File.ReadAllText(path);

                return readText
                    .Replace("[usuario]", usuario)
                    .Replace("[nroCotizacion]", nroCotizacion)
                    .Replace("[productos]", productos);
            }

            private string ComposeBody(string key,string nombre,string documento,string aplicativo)
            {
                string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig(key));

                

                string readText = File.ReadAllText(path);

            return readText
                .Replace("[Nombre]", nombre)
                .Replace("[Documento]", string.Format("<br /><strong>{0}</strong>", documento))
                .Replace("[Aplicativo]", string.Format("<br /><strong>{0}</strong>", aplicativo));
            }

            private string GetValueConfig(string key)
            {
                return ConfigurationManager.AppSettings[key];
            }

            public void SendMail(string process, string address, string subject, string body, string[] fileEntries = null)
            {
                var message = new MailMessage();

                message.To.Add(new MailAddress(address));
                message.Subject = subject;
                message.IsBodyHtml = true;

                string fileName = string.Format("{0}{1}", GetValueConfig("Templates"), "logo.png");

                AlternateView av = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
                LinkedResource lr = new LinkedResource(fileName, MediaTypeNames.Image.Jpeg);

                lr.ContentId = "Logo";
                av.LinkedResources.Add(lr);

                message.AlternateViews.Add(av);
                message.Body = body;

                if (fileEntries != null)
                {
                    foreach (string file in fileEntries)
                    {
                        Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = File.GetCreationTime(file);
                        disposition.ModificationDate = File.GetLastWriteTime(file);
                        disposition.ReadDate = File.GetLastAccessTime(file);
                        message.Attachments.Add(data);
                    }
                }

                try
                {
                    using (var smtp = new SmtpClient())
                    {
                        smtp.Send(message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
   

}
}
