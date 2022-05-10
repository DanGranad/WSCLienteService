﻿using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSGCliente.Core;
using WSGCliente.Entities.BindingModel;
using WSGCliente.Entities.BindingModel.CE;
using WSGCliente.Entities.BindingModel.LAFT;
using WSGCliente.Entities.ViewModel;
using WSGCliente.Util;
using static WSGCliente.Util.GlobalEnum;

namespace WSGCliente.Controllers
{
    [RoutePrefix("Api/Cliente")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ClienteController : ApiController
    {

        string[] ArrayAplicacion = new ConsultaCore().ConsultarAplicacionesGC().Select(x => x.SCOD_APPLICATION).ToArray();

        [Route("ValidarClienteMasivoSCTR")]
        [HttpPost]
        public IHttpActionResult ValidarClienteMasivoSCTR(List<ClientBindingModel> request)
        {
            ResponseViewModel response = new ResponseViewModel();
            List<ListViewErrores> listErrores = new List<ListViewErrores>();
            InsertaCore InsertaCore = new InsertaCore();

            try
            {
                if (request != null)
                {
                    foreach (var item in request)
                    {
                        if (ArrayAplicacion.Contains(item.P_CodAplicacion.ToUpper()))
                        {
                            if (item.P_CodAplicacion.ToUpper() != "REGISTRORM" || item.P_CodAplicacion.ToUpper() != "REGISTROR")
                            {
                                if (item.EListAddresClient != null)
                                {
                                    item.EListAddresClient = SetRecowner(item.EListAddresClient);
                                }
                            }

                            response = ValidarCamposCliente(item);

                            if (response.EListErrores.Count > 0)
                            {
                                foreach (var error in response.EListErrores)
                                {
                                    listErrores.Add(error);
                                }
                            }
                        }
                        else
                        {
                            //response.P_NCODE = "1";
                            //response.P_SMESSAGE = "El tipo de operación enviado no es el correcto.";
                        }
                    }

                    response.EListErrores = listErrores;

                    return Ok(response);

                }
                else
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El request que se ha enviado no tiene el formato correcto.";
                    return Ok(response);
                }
            }

            catch (Exception ex)
            {
                response.P_SMESSAGE = ex.Message;
                response.P_NCODE = "1";
                return Ok(response);
            }

        }

        [Route("ValidarClienteMasivo")]
        [HttpPost]
        public IHttpActionResult ValidarClienteMasivo(List<ClientBindingModel> request)
        {
            ResponseViewModel response = new ResponseViewModel();
            List<ListViewErrores> listErrores = new List<ListViewErrores>();
            InsertaCore InsertaCore = new InsertaCore();

            string V_SPROCESO = "";

            try
            {
                if (request != null)
                {
                    foreach (var item in request)
                    {
                        V_SPROCESO = item.P_SNOPROCESO;

                        if (ArrayAplicacion.Contains(item.P_CodAplicacion.ToUpper()))
                        {
                            if (item.P_CodAplicacion.ToUpper() != "REGISTRORM" || item.P_CodAplicacion.ToUpper() != "REGISTROR")
                            {
                                if (item.EListAddresClient != null)
                                {
                                    item.EListAddresClient = SetRecowner(item.EListAddresClient);
                                }
                            }
                            response = ValidarCamposCliente(item);

                            if (response.EListErrores.Count > 0)
                            {
                                foreach (var error in response.EListErrores)
                                {
                                    if (error.SCAMPO == "ESSEACSA")
                                    {
                                        ResponseViewModel datosReniec = new ResponseViewModel();
                                        datosReniec = consultarReniec(item);
                                        ReniecBindingModel reniecItem = new ReniecBindingModel
                                        {
                                            P_SNOPROCESO = item.P_SNOPROCESO,
                                            P_NNUMREG = item.P_NNUMREG,
                                            P_SFILENAME = item.P_SFILENAME,
                                            P_NIDDOC_TYPE = Convert.ToInt64(datosReniec.EListClient[0].P_NIDDOC_TYPE),
                                            P_SIDDOC = item.P_SIDDOC,
                                            P_SFIRSTNAME = item.P_SFIRSTNAME,
                                            P_SLASTNAME = item.P_SLASTNAME,
                                            P_SLASTNAME2 = item.P_SLASTNAME2,
                                            P_SSEXCLIEN = item.P_SSEXCLIEN,
                                            P_NCIVILSTA = Convert.ToInt64(item.P_NCIVILSTA),
                                            P_DBIRTHDAT = item.P_DBIRTHDAT,
                                            P_STI_DIRE = item.EListAddresClient[0].P_STI_DIRE,
                                            P_SNOM_DIRECCION = item.EListAddresClient[0].P_SNOM_DIRECCION,
                                            P_SNUM_DIRECCION = item.EListAddresClient[0].P_SNUM_DIRECCION,
                                            P_STI_BLOCKCHALET = item.EListAddresClient[0].P_STI_BLOCKCHALET,
                                            P_SBLOCKCHALET = item.EListAddresClient[0].P_SBLOCKCHALET,
                                            P_STI_INTERIOR = item.EListAddresClient[0].P_STI_INTERIOR,
                                            P_SNUM_INTERIOR = item.EListAddresClient[0].P_SNUM_INTERIOR,
                                            P_STI_CJHT = item.EListAddresClient[0].P_STI_CJHT,
                                            P_SNOM_CJHT = item.EListAddresClient[0].P_SNOM_CJHT,
                                            P_SETAPA = item.EListAddresClient[0].P_SETAPA,
                                            P_SMANZANA = item.EListAddresClient[0].P_SMANZANA,
                                            P_SLOTE = item.EListAddresClient[0].P_SLOTE,
                                            P_SREFERENCIA = item.EListAddresClient[0].P_SREFERENCE,
                                            P_NMUNICIPALITY = item.EListAddresClient[0].P_NMUNICIPALITY,
                                            P_SFIRSTNAME_R = datosReniec.EListClient[0].P_SFIRSTNAME,
                                            P_SLASTNAME_R = datosReniec.EListClient[0].P_SLASTNAME,
                                            P_SLASTNAME2_R = datosReniec.EListClient[0].P_SLASTNAME2,
                                            P_SSEXCLIEN_R = datosReniec.EListClient[0].P_SSEXCLIEN,
                                            P_NCIVILSTA_R = Convert.ToInt64(datosReniec.EListClient[0].P_NCIVILSTA),
                                            P_DBIRTHDAT_R = datosReniec.EListClient[0].P_DBIRTHDAT,
                                            P_STI_DIRE_R = datosReniec.EListClient[0].EListAddresClient[0].P_STI_DIRE,
                                            P_SNOM_DIRECCION_R = datosReniec.EListClient[0].EListAddresClient[0].P_SNOM_DIRECCION,
                                            P_SNUM_DIRECCION_R = datosReniec.EListClient[0].EListAddresClient[0].P_SNUM_DIRECCION,
                                            P_STI_BLOCKCHALET_R = datosReniec.EListClient[0].EListAddresClient[0].P_STI_BLOCKCHALET,
                                            P_SBLOCKCHALET_R = datosReniec.EListClient[0].EListAddresClient[0].P_SBLOCKCHALET,
                                            P_STI_INTERIOR_R = datosReniec.EListClient[0].EListAddresClient[0].P_STI_INTERIOR,
                                            P_SNUM_INTERIOR_R = datosReniec.EListClient[0].EListAddresClient[0].P_SNUM_INTERIOR,
                                            P_STI_CJHT_R = datosReniec.EListClient[0].EListAddresClient[0].P_STI_CJHT,
                                            P_SNOM_CJHT_R = datosReniec.EListClient[0].EListAddresClient[0].P_SNOM_CJHT,
                                            P_SETAPA_R = datosReniec.EListClient[0].EListAddresClient[0].P_SETAPA,
                                            P_SMANZANA_R = datosReniec.EListClient[0].EListAddresClient[0].P_SMANZANA,
                                            P_SLOTE_R = datosReniec.EListClient[0].EListAddresClient[0].P_SLOTE,
                                            P_NUSERCODE = Convert.ToInt64(item.P_NUSERCODE)
                                        };
                                        var responseReniec = InsertaCore.InsertarReniec(reniecItem);
                                    }
                                    else
                                    {
                                        ErrorValBindingModel errorItem = new ErrorValBindingModel
                                        {
                                            P_SNOPROCESO = item.P_SNOPROCESO,
                                            P_NNUMREG = item.P_NNUMREG,
                                            P_SFILENAME = item.P_SFILENAME,
                                            P_SDESERROR = error.SMENSAJE,
                                            P_SCOLUMNA = error.SCAMPO,
                                            P_NUSERCODE = Convert.ToInt64(item.P_NUSERCODE),
                                            P_NIDDOC_TYPE = Convert.ToInt64(item.P_NIDDOC_TYPE),
                                            P_SIDDOC = item.P_SIDDOC
                                        };
                                        var responseErrores = InsertaCore.InsertarErrores(errorItem);
                                    }


                                    listErrores.Add(error);
                                }
                            }
                            else
                            {
                                var responseCliente = EjecutarStore(item);
                                if (responseCliente.P_NCODE == "0")
                                {
                                    ExitoValBindingModel exitoItem = new ExitoValBindingModel
                                    {
                                        P_SNOPROCESO = item.P_SNOPROCESO,
                                        P_NNUMREG = item.P_NNUMREG,
                                        P_SFILENAME = item.P_SFILENAME,
                                        P_NIDDOC_TYPE = Convert.ToInt64(item.P_NIDDOC_TYPE),
                                        P_SIDDOC = item.P_SIDDOC,
                                        P_SFIRSTNAME = item.P_SFIRSTNAME,
                                        P_SLASTNAME = item.P_SLASTNAME,
                                        P_SLASTNAME2 = item.P_SLASTNAME2,
                                        P_SLEGALNAME = item.P_SLEGALNAME,
                                        P_SSEXCLIEN = item.P_SSEXCLIEN,
                                        P_NCIVILSTA = Convert.ToInt64(item.P_NCIVILSTA),
                                        P_DBIRTHDAT = item.P_DBIRTHDAT,
                                        P_STI_DIRE = item.EListAddresClient[0].P_STI_DIRE,
                                        P_SNOM_DIRECCION = item.EListAddresClient[0].P_SNOM_DIRECCION,
                                        P_SNUM_DIRECCION = item.EListAddresClient[0].P_SNUM_DIRECCION,
                                        P_STI_BLOCKCHALET = item.EListAddresClient[0].P_STI_BLOCKCHALET,
                                        P_SBLOCKCHALET = item.EListAddresClient[0].P_SBLOCKCHALET,
                                        P_TIPO_DIRE = item.EListAddresClient[0].P_SRECTYPE,
                                        P_STI_INTERIOR = item.EListAddresClient[0].P_STI_INTERIOR,
                                        P_SNUM_INTERIOR = item.EListAddresClient[0].P_SNUM_INTERIOR,
                                        P_STI_CJHT = item.EListAddresClient[0].P_STI_CJHT,
                                        P_SNOM_CJHT = item.EListAddresClient[0].P_SNOM_CJHT,
                                        P_SETAPA = item.EListAddresClient[0].P_SETAPA,
                                        P_SMANZANA = item.EListAddresClient[0].P_SMANZANA,
                                        P_SLOTE = item.EListAddresClient[0].P_SLOTE,
                                        P_SREFERENCIA = item.EListAddresClient[0].P_SREFERENCIA,
                                        P_NMUNICIPALITY = Convert.ToInt64(item.EListAddresClient[0].P_NMUNICIPALITY),
                                        P_NPHONE_TYPE = Convert.ToInt64(item.EListPhoneClient[0].P_NPHONE_TYPE),
                                        P_SPHONE = item.EListPhoneClient[0].P_SPHONE,
                                        P_TIPO_EMAIL = item.EListEmailClient[0].P_SRECTYPE,
                                        P_SE_MAIL = item.EListEmailClient[0].P_SE_MAIL,
                                        P_SCLIENT = responseCliente.P_SCOD_CLIENT,
                                        P_NUSERCODE = Convert.ToInt64(item.P_NUSERCODE)
                                    };
                                    var resExitoso = InsertaCore.InsertarExitosos(exitoItem);
                                }
                            }
                        }
                        else
                        {
                            //response.P_NCODE = "1";
                            //response.P_SMESSAGE = "El tipo de operación enviado no es el correcto.";
                        }
                    }

                    response.EListErrores = listErrores;


                    //Llamar para crear Excel
                    ReportBindingModel reportRequest = new ReportBindingModel
                    {
                        P_SNOPROCESO = V_SPROCESO
                    };
                    var ListErrores = InsertaCore.ListarError(reportRequest);
                    var ListExitosos = InsertaCore.ListarExitoso(reportRequest);
                    var ListReniec = InsertaCore.ListarReniec(reportRequest);

                    string pathErrores = "";
                    string pathExitosos = "";
                    string pathReniec = "";

                    if (ListErrores.Count > 0)
                    {
                        pathErrores = GenerarExcelError(ListErrores, V_SPROCESO);
                    }

                    if (ListExitosos.Count > 0)
                    {
                        pathExitosos = GenerarExcelExitoso(ListExitosos, V_SPROCESO);
                    }

                    if (ListReniec.Count > 0)
                    {
                        pathReniec = GenerarExcelReniec(ListReniec, V_SPROCESO);
                    }

                    EnviarEmail(pathErrores, pathExitosos, pathReniec, V_SPROCESO, request[0].P_NUSERCODE);
                    return Ok(response);

                }
                else
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El request que se ha enviado no tiene el formato correcto.";
                    return Ok(response);
                }
            }

            catch (Exception ex)
            {
                response.P_SMESSAGE = ex.Message;
                response.P_NCODE = "1";
                return Ok(response);
            }
        }


        [Route("ValidarEmail")]
        [HttpPost]
        public IHttpActionResult PruebaMail(ClientBindingModel request)
        {
            ResponseViewModel response = new ResponseViewModel();
            List<ListViewErrores> listErrores = new List<ListViewErrores>();
            InsertaCore InsertaCore = new InsertaCore();


            string V_SPROCESO = "";

            try
            {
                EnviarEmail2();
                return Ok(response);

            }

            catch (Exception ex)
            {
                response.P_SMESSAGE = ex.Message;
                response.P_NCODE = "1";
                return Ok(response);
            }
        }
        private void EnviarEmail2()
        {
            InsertaCore InsertaCore = new InsertaCore();
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

            ResponseViewModel responseCorreo = new ResponseViewModel();


            System.Net.Mail.Attachment archivo = null;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(mstr_SmtpClient);
                //Especificamos el correo desde el que se enviará el Email y el nombre de la persona que lo envía
                mail.From = new MailAddress(mstr_Email, "Prueba Email", Encoding.UTF8);
                mail.Subject = "Prueba Envio Email";

                mail.To.Add(ConfigurationManager.AppSettings["wstr_EmailAdmin"]);


                //archivo.Dispose(); 

                mail.Body = "This is for testing SMTP mail from GMAIL";
                int port = Convert.ToInt16(mstr_SmtpPort);
                SmtpServer.Port = port; //Puerto que utiliza Gmail para sus servicios
                                        //Especificamos las credenciales con las que enviaremos el mail
                SmtpServer.Credentials = new System.Net.NetworkCredential(mstr_Email, mstr_EmailPassw);
                SmtpServer.EnableSsl = true;
                //SmtpServer.UseDefaultCredentials = true;
                SmtpServer.Send(mail);




                //return "Correcto";
            }
            catch (Exception ex)
            {

                //log.Info(string.Format("Estado Email: {0}", "No Enviado"));
                //  log.Info(string.Format("Error: {0}", ex.Message));
                throw ex;
            }
        }

        private void EnviarEmail(string pathErrores, string pathExitosos, string pathReniec, string V_SPROCESO, string NUSERCODE)
        {
            InsertaCore InsertaCore = new InsertaCore();
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

            ResponseViewModel responseCorreo = new ResponseViewModel();
            responseCorreo = InsertaCore.DestinatarioEmail(NUSERCODE);

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

                if (pathErrores != "")
                {
                    archivo = new System.Net.Mail.Attachment(pathErrores)
                    {
                        Name = V_SPROCESO + "Errores.xls"
                    };
                    mail.Attachments.Add(archivo);
                }

                if (pathExitosos != "")
                {
                    archivo = new System.Net.Mail.Attachment(pathExitosos)
                    {
                        Name = V_SPROCESO + "Exitosos.xls"
                    };
                    mail.Attachments.Add(archivo);
                }

                if (pathReniec != "")
                {
                    archivo = new System.Net.Mail.Attachment(pathReniec)
                    {
                        Name = V_SPROCESO + "Reniec.xls"
                    };
                    mail.Attachments.Add(archivo);
                }

                //archivo.Dispose(); 
                mail.IsBodyHtml = true;
                if (responseCorreo.P_SEMAIL != "")
                {
                    mail.To.Add(responseCorreo.P_SEMAIL);
                }
                else
                {
                    mail.To.Add(ConfigurationManager.AppSettings["wstr_EmailAdmin"]);
                }

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
                archivo.Dispose();

                if (pathErrores != "" || pathExitosos != "" || pathReniec != "")
                {
                    System.IO.Directory.Delete(ConfigurationManager.AppSettings["RutaArchivo"] + V_SPROCESO, true);
                }

                //return "Correcto";
            }
            catch (Exception ex)
            {
                archivo.Dispose();
                if (pathErrores != "" || pathExitosos != "" || pathReniec != "")
                {
                    System.IO.Directory.Delete(ConfigurationManager.AppSettings["RutaArchivo"] + V_SPROCESO, true);
                }
                //log.Info(string.Format("Estado Email: {0}", "No Enviado"));
                //  log.Info(string.Format("Error: {0}", ex.Message));
                throw ex;
            }
        }

        private string GenerarExcelReniec(List<ReniecViewModel> listReniec, string v_SPROCESO)
        {
            dynamic pathReniec = "";
            try
            {
                ReportDataSource dsOBJ = new ReportDataSource
                {
                    Name = "dsReniec",
                    Value = listReniec
                };
                IEnumerable<ReportDataSource> datasets = new List<ReportDataSource> { dsOBJ };
                LocalReport localReport = new LocalReport
                {
                    ReportPath = HttpContext.Current.Server.MapPath("~/Reports/ReportReniec.rdlc")
                };
                foreach (ReportDataSource datasource in datasets)
                {
                    localReport.DataSources.Add(datasource);
                }

                //Renderizado
                string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat></DeviceInfo>";
                byte[] renderedBytes;

                string path = ConfigurationManager.AppSettings["RutaArchivo"] + v_SPROCESO;
                //string path = @"D:\Report\" + v_SPROCESO;
                pathReniec = (path + "/" + v_SPROCESO + "Reniec.xls");

                renderedBytes = localReport.Render("EXCEL", deviceInfo, out string mimeType, out string encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings);

                FileStream file =
                    default(FileStream);

                file = new FileStream(pathReniec, FileMode.Create);
                file.Write(renderedBytes, 0, renderedBytes.Length);

                file.Close();
                file.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pathReniec;
        }

        private string GenerarExcelExitoso(List<ExitoViewModel> listExitosos, string v_SPROCESO)
        {
            dynamic pathExitosos = "";
            try
            {
                ReportDataSource dsOBJ = new ReportDataSource
                {
                    Name = "dsExitoso",
                    Value = listExitosos
                };
                IEnumerable<ReportDataSource> datasets = new List<ReportDataSource> { dsOBJ };
                LocalReport localReport = new LocalReport
                {
                    ReportPath = HttpContext.Current.Server.MapPath("~/Reports/ReportExitoso.rdlc")
                };
                foreach (ReportDataSource datasource in datasets)
                {
                    localReport.DataSources.Add(datasource);
                }

                //Renderizado
                string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat></DeviceInfo>";
                byte[] renderedBytes;

                string path = ConfigurationManager.AppSettings["RutaArchivo"] + v_SPROCESO;
                //string path = @"D:\Report\" + v_SPROCESO;
                pathExitosos = (path + "/" + v_SPROCESO + "Exitosos.xls");

                renderedBytes = localReport.Render("EXCEL", deviceInfo, out string mimeType, out string encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings);

                FileStream file = default(FileStream);

                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                file = new FileStream(pathExitosos, FileMode.Create);
                file.Write(renderedBytes, 0, renderedBytes.Length);

                file.Close();
                file.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pathExitosos;
        }

        private string GenerarExcelError(List<ErrorViewModel> listErrores, string v_SPROCESO)
        {
            dynamic pathErrores = "";
            try
            {
                ReportDataSource dsOBJ = new ReportDataSource
                {
                    Name = "dsError",
                    Value = listErrores
                };
                IEnumerable<ReportDataSource> datasets = new List<ReportDataSource> { dsOBJ };
                LocalReport localReport = new LocalReport
                {
                    ReportPath = HttpContext.Current.Server.MapPath("~/Reports/ReportError.rdlc")
                };
                foreach (ReportDataSource datasource in datasets)
                {
                    localReport.DataSources.Add(datasource);
                }

                //Renderizado
                string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat></DeviceInfo>";
                byte[] renderedBytes;

                string path = ConfigurationManager.AppSettings["RutaArchivo"] + v_SPROCESO;
                //string path = @"D:\Report\" + v_SPROCESO;
                pathErrores = (path + "/" + v_SPROCESO + "Errores.xls");

                renderedBytes = localReport.Render("EXCEL", deviceInfo, out string mimeType, out string encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings);

                FileStream file = default(FileStream);

                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                file = new FileStream(pathErrores, FileMode.Create);
                file.Write(renderedBytes, 0, renderedBytes.Length);

                file.Close();
                file.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pathErrores;
        }

        private ResponseViewModel consultarReniec(ClientBindingModel item)
        {
            ServiceCore ServiceCore = new ServiceCore();

            ResponseViewModel response = new ResponseViewModel();
            //ResponsePViewModel responseP = new ResponsePViewModel();
            ResponseReniecViewModel responseReniec = new ResponseReniecViewModel();
            ClientViewModel itemCliente = new ClientViewModel();

            try
            {
                string responseR = "";
                responseR = ServiceCore.ConsultarCliente(item.P_SIDDOC, "UrlService");
                responseReniec = JsonConvert.DeserializeObject<ResponseReniecViewModel>(responseR, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                if (responseReniec.CODIGOERROR != "0000")
                {
                    responseReniec.CODIGOERROR = "0004";
                    response.P_NCODE = "3";
                }
            }
            catch (Exception ex)
            {
                responseReniec.CODIGOERROR = "0004";
                response.P_NCODE = "3";
                response.P_SMESSAGE = "No se encontro informacion .";

            }

            if (responseReniec.CODIGOERROR == "0000")
            {
                itemCliente.P_NIDDOC_TYPE = "2";
                itemCliente.P_SIDDOC = responseReniec.NUMERODNI;
                itemCliente.P_DIG_VERIFICACION = responseReniec.DIGITOVERIFICACION;
                itemCliente.P_SLASTNAME = responseReniec.APELLIDOPATERNO.Trim();
                itemCliente.P_SLASTNAME2 = responseReniec.APELLIDOMATERNO.Trim();
                itemCliente.P_APELLIDO_CASADA = responseReniec.APELLIDOCASADA.Trim();
                itemCliente.P_SFIRSTNAME = responseReniec.NOMBRES.Trim();
                if (responseReniec.ESTADOCIVILCIUDADANO == "1")
                {
                    itemCliente.P_NCIVILSTA = "2";
                }
                else if (responseReniec.SEXO == "2")
                {
                    itemCliente.P_NCIVILSTA = "1";
                }
                else
                {
                    itemCliente.P_NCIVILSTA = responseReniec.ESTADOCIVILCIUDADANO;
                }
                itemCliente.P_SGRADO_INSTRUCCION = responseReniec.CODIGOGRADOINSTRUCCION;
                itemCliente.P_NHEIGHT = responseReniec.ESTATURA;
                if (responseReniec.SEXO == "1")
                {
                    itemCliente.P_SSEXCLIEN = "2";
                }
                else if (responseReniec.SEXO == "2")
                {
                    itemCliente.P_SSEXCLIEN = "1";
                }
                else
                {
                    itemCliente.P_SSEXCLIEN = responseReniec.SEXO;
                }
                itemCliente.P_ORIGEN_DATA = "RENIEC";
                itemCliente.P_NNATIONALITY = "1";
                itemCliente.P_SSISTEMA = item.P_SSISTEMA;
                itemCliente.P_TI_DOC_SUSTENT = responseReniec.TIPODOCUMENTOIDENTIDAD;
                itemCliente.P_NU_DOC_SUSTENT = responseReniec.NUMERODOCUMENTOIDENTIDAD.Trim();
                itemCliente.P_COD_UBIG_DEP_NAC = responseReniec.CODIGOUBIGEODEPARTAMENTONACIMIENTO;
                itemCliente.P_COD_UBIG_PROV_NAC = responseReniec.CODIGOUBIGEOPROVINCIANACIMIENTO;
                itemCliente.P_COD_UBIG_DIST_NAC = responseReniec.CODIGOUBIGEODISTRITONACIMIENTO;
                itemCliente.P_DEPARTAMENTO_NACIMIENTO = responseReniec.DEPARTAMENTONACIMIENTO.Trim();
                itemCliente.P_PROVINCIA_NACIMIENTO = responseReniec.PROVINCIANACIMIENTO.Trim();
                itemCliente.P_DISTRITO_NACIMIENTO = responseReniec.DISTRITONACIMIENTO.Trim();
                var fechaNacimiento = DateTime.ParseExact(responseReniec.FECHANACIMIENTO, "yyyyMMdd", CultureInfo.InvariantCulture);
                itemCliente.P_DBIRTHDAT = fechaNacimiento.ToString("dd/MM/yyyy");
                itemCliente.P_NOMBRE_PADRE = responseReniec.NOMBRESPADRE.Trim();
                itemCliente.P_NOMBRE_MADRE = responseReniec.NOMBRESMADRE.Trim();
                var fechaInscripcion = DateTime.ParseExact(responseReniec.FECHAINSCRIPCION, "yyyyMMdd", CultureInfo.InvariantCulture);
                itemCliente.P_FECHA_INSC = fechaInscripcion.ToString("dd/MM/yyyy");
                var fechaEmision = DateTime.ParseExact(responseReniec.FECHAEMISION, "yyyyMMdd", CultureInfo.InvariantCulture);
                itemCliente.P_FECHA_EXPEDICION = fechaEmision.ToString("dd/MM/yyyy");
                itemCliente.P_CONSTANCIA_VOTACION = responseReniec.CONSTANCIAVOTACION;
                itemCliente.P_RESTRICCION = responseReniec.RESTRICCIONES.Trim();
                itemCliente.P_NTITLE = "99";
                itemCliente.P_NSPECIALITY = "99";
                itemCliente.P_SISRENIEC_IND = "1";

                itemCliente.EListAddresClient = new List<AddressViewModel>();
                var itemDireccion = new AddressViewModel
                {
                    P_NCOUNTRY = "1",
                    P_DESTIDIRE = "Particular",
                    P_SRECTYPE = "2",
                    P_SCOD_DEP_UBI_DOM = responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO,
                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                    P_SCOD_PRO_UBI_DOM = responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                    P_SCOD_DIS_UBI_DOM = responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                    P_SDES_DEP_DOM = responseReniec.DEPARTAMENTODOMICILIO.Trim(),
                    P_SDES_PRO_DOM = responseReniec.PROVINCIADOMICILIO.Trim(),
                    P_SDES_DIS_DOM = responseReniec.DISTRITODOMICILIO.Trim(),
                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                    P_SBLOCKCHALET = responseReniec.BLOCKCHALET.Trim(),
                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                    P_SETAPA = responseReniec.ETAPA.Trim(),
                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                    P_SLOTE = responseReniec.LOTE.Trim(),
                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim()
                };
                //Direccion completa
                var param = new DireccionCompletaBindingModel
                {
                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                    P_SBLOCKCHALET = responseReniec.BLOCKCHALET.Trim(),
                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim(),
                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                    P_SETAPA = responseReniec.ETAPA.Trim(),
                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                    P_SLOTE = responseReniec.LOTE.Trim(),
                    P_SREFERENCIA = "",
                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO
                };
                //itemDireccion.P_SDESDIREBUSQ = result.P_SDESDIREBUSQ;
                itemCliente.EListAddresClient.Add(itemDireccion);
                itemCliente.EListPhoneClient = new List<PhoneViewModel>();
                itemCliente.EListEmailClient = new List<EmailViewModel>();
                itemCliente.EListContactClient = new List<ContactViewModel>();
                itemCliente.EListCIIUClient = new List<CiiuViewModel>();
                response.EListClient = new List<ClientViewModel>
                {
                    itemCliente
                };
            }

            return response;
        }

        [Route("ValidarCliente")]
        [HttpPost]
        public IHttpActionResult ValidarCliente(ClientBindingModel request)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                if (request != null)
                {
                    if (ArrayAplicacion.Contains(request.P_CodAplicacion.ToUpper()))
                    {
                        if (request.P_CodAplicacion.ToUpper() != "REGISTRORM" && request.P_CodAplicacion.ToUpper() != "REGISTROR")
                        {
                            if (request.EListAddresClient != null)
                            {
                                request.EListAddresClient = SetRecowner(request.EListAddresClient);
                            }
                        }

                        response = ValidarCamposCliente(request);
                        if (response.P_NCODE == "0")
                        {

                            var _Aplicattion = new ConsultaCore().ConsultarAplicacionesGC().Where(x => x.SCOD_APPLICATION == request.P_CodAplicacion).FirstOrDefault();
                            if (_Aplicattion.NIND_USE_LAFT == "1")
                            {
                                var ClientLAFT = ConsultaClientLAFT(request.P_SIDDOC);
                                if (ClientLAFT.Count > 0)
                                {
                                    if (ClientLAFT[0].liberado == false)
                                    {
                                        if (ClientLAFT[0].aplicaciones.Where(x => x.aplicacion.id == _Aplicattion.NID_SISTEMA_LAFT && x.activo).ToList().Count <= 0 &&
                                            ClientLAFT[0].configRegistro.aplicaciones.Where(x => x.aplicacion.id == _Aplicattion.NID_SISTEMA_LAFT && x.activo).ToList().Count <= 0)
                                        {
                                            if (ClientLAFT[0].senial.indAlert)
                                            {
                                                var Cuerpo = listMessage(request, _Aplicattion);
                                                foreach (Message item in Cuerpo)
                                                {
                                                    new Mail().SendMail("0", item.Address, item.Subject, item.Body, null);
                                                }

                                            }
                                            if (ClientLAFT[0].senial.indError)
                                            {
                                                response.EListErrores = new List<ListViewErrores> { new ListViewErrores { SMENSAJE = WSGCliente.Util.GlobalMessage.ClientLAFT } };
                                                response.P_NCODE = "1";
                                                response.P_SMESSAGE = WSGCliente.Util.GlobalMessage.ClientLAFT;
                                                return Ok(response);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        response.P_NCODE = "1";
                        response.P_SMESSAGE = "El tipo de operación enviado no es el correcto.";
                    }
                    return Ok(response);

                }
                else
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El request que se ha enviado no tiene el formato correcto.";
                    return Ok(response);
                }
            }

            catch (Exception ex)
            {
                response.P_SMESSAGE = ex.Message;
                response.P_NCODE = "1";
                return Ok(response);
            }
        }

        public string ConsultarReniec(string Documento)
        {
            return null;
        }

        [Route("GestionarCliente")]
        [HttpPost]
        public IHttpActionResult GestionarCliente(ClientBindingModel request)
        {
            ConsultaCore ConsultaCore = new ConsultaCore();
            InsertaCore InsertaCore = new InsertaCore();
            ServiceCore ServiceCore = new ServiceCore();

            ResponseViewModel response = new ResponseViewModel();
            ResponsePViewModel responseP = new ResponsePViewModel();
            ResponseReniecViewModel responseReniec = new ResponseReniecViewModel();
            string TipoDoc;
            try
            {
                //Se homologa el tipo de documento de acuerdo al codigo de aplicacion
                TipoDoc = HomologarCampo(request.P_NIDDOC_TYPE, request.P_CodAplicacion, "DOCIDE");

                if (request != null)
                {
                    switch (request.P_CodAplicacion.ToUpper())
                    {
                        case "EXTERNO1GCI":
                            response = ConsultaCore.Consultar(request);
                            if (response.P_NCODE == "0")
                            {
                                responseP = ConsultaCore.ConsultarProveedor(request);
                                if (responseP.P_NCODE == "0")
                                {
                                    responseP.EListClient = ConsultaCore.ConsultarClienteProveedor(request);
                                    if (responseP.EListClient.Count == 0)
                                    {
                                        responseP.P_SMESSAGE = "No existen registros con los criterios ingresados.";
                                    }
                                }
                            }
                            else
                            {
                                responseP.P_NCODE = response.P_NCODE;
                                responseP.P_SMESSAGE = response.P_SMESSAGE;
                            }

                            break;
                        default:

                            if (ArrayAplicacion.Contains(request.P_CodAplicacion.ToUpper()))
                            {
                                if (request.P_CodAplicacion.ToUpper() == "REGISTRORM" || request.P_CodAplicacion.ToUpper() == "REGISTROR")
                                {
                                }
                                else
                                {
                                    if (request.EListAddresClient != null)
                                    {
                                        request.EListAddresClient = SetRecowner(request.EListAddresClient);
                                    }
                                }
                                //response = ValidarCamposCliente(request);

                                switch (request.P_TipOper.ToUpper())
                                {
                                    case "CON":

                                        response = ConsultaCore.Consultar(request);
                                        if (response.P_NCODE == "0")
                                        {
                                            response.EListClient = new List<ClientViewModel>();
                                            if (request.P_CONSUMESERV_IND != "1")
                                            {
                                                response.EListClient = ConsultaCore.ConsultarCliente(request);
                                            }
                                            if (response.EListClient.Count == 0)
                                            {
                                                if (request.P_NROL != "104")
                                                {
                                                    if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "2")
                                                    {
                                                        try
                                                        {
                                                            try
                                                            {
                                                                string responseR = "";
                                                                responseReniec = ObtenerClientReniecLocal(request, out bool ExisteLocal);
                                                                if (ExisteLocal != true)
                                                                {
                                                                    responseR = ServiceCore.ConsultarCliente(request.P_SIDDOC, "UrlService");
                                                                    responseReniec = JsonConvert.DeserializeObject<ResponseReniecViewModel>(responseR, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                                                    AddClientReniec(responseReniec);
                                                                }
                                                                if (responseReniec.CODIGOERROR != "0000")
                                                                {
                                                                    responseReniec.CODIGOERROR = "0004";
                                                                    response.P_NCODE = "3";
                                                                    return Ok(response);
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                responseReniec.CODIGOERROR = "0004";
                                                                response.P_NCODE = "3";
                                                                response.P_SMESSAGE = "No se encontro informacion .";
                                                                return Ok(response);

                                                            }

                                                            if (responseReniec.CODIGOERROR == "0000")
                                                            {
                                                                var itemCliente = new ClientViewModel
                                                                {
                                                                    P_NIDDOC_TYPE = "2",
                                                                    P_SIDDOC = responseReniec.NUMERODNI,
                                                                    P_DIG_VERIFICACION = responseReniec.DIGITOVERIFICACION,
                                                                    P_SLASTNAME = responseReniec.APELLIDOPATERNO.Trim(),
                                                                    P_SLASTNAME2 = responseReniec.APELLIDOMATERNO.Trim(),
                                                                    P_APELLIDO_CASADA = responseReniec.APELLIDOCASADA.Trim(),
                                                                    P_SFIRSTNAME = responseReniec.NOMBRES.Trim(),
                                                                    P_SFOTO = responseReniec.FOTO,
                                                                    P_SFIRMA = responseReniec.FIRMA
                                                                };

                                                                if (responseReniec.ESTADOCIVILCIUDADANO == "1")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "2";
                                                                }
                                                                else if (responseReniec.ESTADOCIVILCIUDADANO == "2")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_NCIVILSTA = responseReniec.ESTADOCIVILCIUDADANO;
                                                                }

                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = HomologarCampo(responseReniec.ESTADOCIVILCIUDADANO, request.P_CodAplicacion, "RESTADOCIVIL");
                                                                }

                                                                itemCliente.P_SGRADO_INSTRUCCION = responseReniec.CODIGOGRADOINSTRUCCION;
                                                                itemCliente.P_NHEIGHT = responseReniec.ESTATURA;

                                                                if (responseReniec.SEXO == "1")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "2";
                                                                }
                                                                else if (responseReniec.SEXO == "2")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = responseReniec.SEXO;
                                                                }
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = HomologarCampo(responseReniec.SEXO, request.P_CodAplicacion, "RSEXO");
                                                                }
                                                                itemCliente.P_ORIGEN_DATA = "RENIEC";
                                                                itemCliente.P_NNATIONALITY = "1";
                                                                itemCliente.P_SSISTEMA = request.P_SSISTEMA;
                                                                itemCliente.P_TI_DOC_SUSTENT = responseReniec.TIPODOCUMENTOIDENTIDAD;
                                                                itemCliente.P_NU_DOC_SUSTENT = responseReniec.NUMERODOCUMENTOIDENTIDAD.Trim();
                                                                itemCliente.P_COD_UBIG_DEP_NAC = responseReniec.CODIGOUBIGEODEPARTAMENTONACIMIENTO;
                                                                itemCliente.P_COD_UBIG_PROV_NAC = responseReniec.CODIGOUBIGEOPROVINCIANACIMIENTO;
                                                                itemCliente.P_COD_UBIG_DIST_NAC = responseReniec.CODIGOUBIGEODISTRITONACIMIENTO;
                                                                itemCliente.P_DEPARTAMENTO_NACIMIENTO = responseReniec.DEPARTAMENTONACIMIENTO.Trim();
                                                                itemCliente.P_PROVINCIA_NACIMIENTO = responseReniec.PROVINCIANACIMIENTO.Trim();
                                                                itemCliente.P_DISTRITO_NACIMIENTO = responseReniec.DISTRITONACIMIENTO.Trim();
                                                                var fechaNacimiento = DateTime.ParseExact(responseReniec.FECHANACIMIENTO, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_DBIRTHDAT = fechaNacimiento.ToString("dd/MM/yyyy");
                                                                itemCliente.P_NOMBRE_PADRE = responseReniec.NOMBRESPADRE.Trim();
                                                                itemCliente.P_NOMBRE_MADRE = responseReniec.NOMBRESMADRE.Trim();
                                                                var fechaInscripcion = DateTime.ParseExact(responseReniec.FECHAINSCRIPCION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_INSC = fechaInscripcion.ToString("dd/MM/yyyy");
                                                                var fechaEmision = DateTime.ParseExact(responseReniec.FECHAEMISION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_EXPEDICION = fechaEmision.ToString("dd/MM/yyyy");
                                                                itemCliente.P_CONSTANCIA_VOTACION = responseReniec.CONSTANCIAVOTACION;
                                                                itemCliente.P_RESTRICCION = responseReniec.RESTRICCIONES.Trim();
                                                                itemCliente.P_NTITLE = "99";
                                                                itemCliente.P_NSPECIALITY = "99";
                                                                itemCliente.P_SISRENIEC_IND = "1";

                                                                itemCliente.EListAddresClient = new List<AddressViewModel>();
                                                                var itemDireccion = new AddressViewModel
                                                                {
                                                                    P_NCOUNTRY = "1",
                                                                    P_DESTIDIRE = "Particular",
                                                                    P_SRECTYPE = "2",
                                                                    P_SCOD_DEP_UBI_DOM = responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO,
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_SCOD_PRO_UBI_DOM = responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_SCOD_DIS_UBI_DOM = responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_SDES_DEP_DOM = responseReniec.DEPARTAMENTODOMICILIO.Trim(),
                                                                    P_SDES_PRO_DOM = responseReniec.PROVINCIADOMICILIO.Trim(),
                                                                    P_SDES_DIS_DOM = responseReniec.DISTRITODOMICILIO.Trim(),
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.BLOCKCHALET.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim()
                                                                };
                                                                //Direccion completa
                                                                var param = new DireccionCompletaBindingModel
                                                                {
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_SREFERENCIA = "",
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO
                                                                };
                                                                var result = InsertaCore.DireccionCompleta(param);
                                                                itemDireccion.P_SDESDIREBUSQ = result.P_SDESDIREBUSQ;
                                                                itemCliente.EListAddresClient.Add(itemDireccion);
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.EListAddresClient = HomologarUbigeo(itemCliente.EListAddresClient, request.P_CodAplicacion.ToString());
                                                                }
                                                                response.EListClient.Add(itemCliente);

                                                                itemCliente.EListPhoneClient = new List<PhoneViewModel>();
                                                                itemCliente.EListEmailClient = new List<EmailViewModel>();
                                                                itemCliente.EListContactClient = new List<ContactViewModel>();
                                                                itemCliente.EListCIIUClient = new List<CiiuViewModel>();
                                                            }
                                                            else
                                                            {
                                                                // response.P_NCODE = "1";
                                                                if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                                {
                                                                    response.P_NCODE = "1";
                                                                }
                                                                else
                                                                {
                                                                    response.P_NCODE = "3";
                                                                }
                                                                response.P_SMESSAGE = "no se encontro informacion!";
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                            {
                                                                response.P_NCODE = "1";
                                                            }
                                                            else
                                                            {
                                                                response.P_NCODE = "3";
                                                            }
                                                            response.P_SMESSAGE = "no se encontro informacion!";
                                                            //response.P_NCODE = "1";
                                                            //response.P_SMESSAGE = "El dni ingresado NO es válido.";
                                                        }


                                                    }
                                                    else if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "1")
                                                    {
                                                        return Ok(ConsultarSUNAT(request.P_SIDDOC));
                                                    }
                                                    else if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "4")
                                                    {
                                                        return Ok(ConsultarCE(request));
                                                    }

                                                }

                                            }
                                            else
                                            {
                                                foreach (var item in response.EListClient)
                                                {
                                                    item.P_SSISTEMA = request.P_SSISTEMA;
                                                    item.P_ORIGEN_DATA = "VTIME";
                                                    if (request.P_CodAplicacion == "FIDELIZACION")
                                                    {
                                                        item.P_SSEXCLIEN = HomologarCampo(item.P_SSEXCLIEN, request.P_CodAplicacion, "RSEXO");
                                                        item.P_NCIVILSTA = HomologarCampo(item.P_NCIVILSTA, request.P_CodAplicacion, "RESTADOCIVIL");
                                                        List<AddressViewModel> ListAddress = ConsultaCore.ConsultarClienteDireccion(item.P_SCLIENT);
                                                        item.EListAddresClient = HomologarUbigeo(ListAddress, request.P_CodAplicacion);
                                                    }
                                                    else
                                                    {
                                                        item.EListAddresClient = ConsultaCore.ConsultarClienteDireccion(item.P_SCLIENT);
                                                    }
                                                    item.EListPhoneClient = ConsultaCore.ConsultarClienteTelefono(item.P_SCLIENT);
                                                    item.EListEmailClient = ConsultaCore.ConsultarClienteEmail(item.P_SCLIENT);
                                                    item.EListContactClient = ConsultaCore.ConsultarClienteContacto(item.P_SCLIENT, item.P_NPERSON_TYP);
                                                    item.EListCIIUClient = ConsultaCore.ConsultarClienteCiiu(item.P_SCLIENT);
                                                    item.EListHistoryClient = ConsultaCore.ConsultarClienteHistory(item.P_SCLIENT);
                                                    item.ElistInfoBancariaClient = ConsultaCore.ConsultarInfoBancaria(item.P_SCLIENT);
                                                    item.ElistDocumentosClient = ConsultaCore.ConsultarDocumentosAdjunto(item.P_SCLIENT);
                                                }
                                            }
                                        }
                                        break;
                                    case "INS":
                                        try
                                        {
                                            var _Aplicattion = new ConsultaCore().ConsultarAplicacionesGC().Where(x => x.SCOD_APPLICATION == request.P_CodAplicacion).FirstOrDefault();

                                            if (_Aplicattion.NIND_USE_LAFT == "1")
                                            {
                                                var ClientLAFT = ConsultaClientLAFT(request.P_SIDDOC);
                                                if (ClientLAFT.Count > 0)
                                                {
                                                    if (ClientLAFT[0].liberado == false)
                                                    {
                                                        if (ClientLAFT[0].aplicaciones.Where(x => x.aplicacion.id == _Aplicattion.NID_SISTEMA_LAFT && x.activo).ToList().Count <= 0 &&
                                                            ClientLAFT[0].configRegistro.aplicaciones.Where(x => x.aplicacion.id == _Aplicattion.NID_SISTEMA_LAFT && x.activo).ToList().Count <= 0)
                                                        {
                                                            if (ClientLAFT[0].senial.indAlert)
                                                            {
                                                                var Cuerpo = listMessage(request, _Aplicattion);
                                                                foreach (Message item in Cuerpo)
                                                                {
                                                                    new Mail().SendMail("0", item.Address, item.Subject, item.Body, null);
                                                                }

                                                            }
                                                            if (ClientLAFT[0].senial.indError)
                                                            {
                                                                response.P_NCODE = "1";
                                                                response.P_SMESSAGE = WSGCliente.Util.GlobalMessage.ClientLAFT;
                                                                return Ok(response);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            response = EjecutarStore(request);

                                        }
                                        catch (Exception ex)
                                        {
                                            response = EjecutarStore(request);
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                response.P_NCODE = "1";
                                response.P_SMESSAGE = "El tipo de operación enviado no es el correcto.";
                            }
                            break;
                    }



                    if (request.P_CodAplicacion == "EXTERNO1GCI")
                    {
                        return Ok(responseP);
                    }
                    else
                    {
                        //return Ok(responseReniec);
                        return Ok(response);
                    }

                }
                else
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El request que se ha enviado no tiene el formato correcto.";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.P_NCODE = "-1";
                response.P_SMESSAGE = ex.Message;
                return Ok(response);
            }
        }
        //add  20220119
        [Route("GestionarCliente/SearchClientHistoryInformation")]
        [HttpPost]
        public IHttpActionResult SearchClientHistoryInformation(ClientHistoryBindingModel request)
        {
            ConsultaCore ConsultaCore = new ConsultaCore();

            ResponseClientHistoryViewModel response = new ResponseClientHistoryViewModel();
            List<HistoryInformationViewModel> EListHistoryInformationClient = new List<HistoryInformationViewModel>();


            try
            {
                response.EListHistoryInformationClient = ConsultaCore.ConsultarClienteHistoryInformation(request.P_NID, request.P_SCLIENT);
                response.EListHistoryPhoneBeforeClient = ConsultaCore.ConsultarClienteHistoryPhoneBefore(request.P_NID, request.P_SCLIENT);
                response.EListHistoryPhoneNowClient = ConsultaCore.ConsultarClienteHistoryPhoneNow(request.P_NID, request.P_SCLIENT);
                response.EListHistoryEmailBeforeClient = ConsultaCore.ConsultarClienteHistoryEmailBefore(request.P_NID, request.P_SCLIENT);
                response.EListHistoryEmailNowClient = ConsultaCore.ConsultarClienteHistoryEmailNow(request.P_NID, request.P_SCLIENT);
                response.EListHistoryAddressBeforeClient = ConsultaCore.ConsultarClienteHistoryAddressBefore(request.P_NID, request.P_SCLIENT);
                response.EListHistoryAddressNowClient = ConsultaCore.ConsultarClienteHistoryAddressNow(request.P_NID, request.P_SCLIENT);
                response.EListHistoryContactBeforeClient = ConsultaCore.ConsultarClienteHistoryContactBefore(request.P_NID, request.P_SCLIENT);
                response.EListHistoryContactNowClient = ConsultaCore.ConsultarClienteHistoryContactNow(request.P_NID, request.P_SCLIENT);
                response.P_NCODE = "0";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.P_NCODE = "-1";
                response.P_SMESSAGE = ex.Message;
                return Ok(response);
            }
        }
        //add

        [Route("GestionarClienteReniec")]
        [HttpPost]
        public IHttpActionResult GestionarClienteReniec(ClientBindingModel request)
        {
            ConsultaCore ConsultaCore = new ConsultaCore();
            InsertaCore InsertaCore = new InsertaCore();
            ServiceCore ServiceCore = new ServiceCore();

            ResponseViewModel response = new ResponseViewModel();
            ResponsePViewModel responseP = new ResponsePViewModel();
            ResponseReniecViewModel responseReniec = new ResponseReniecViewModel();
            string TipoDoc;
            try
            {
                //Se homologa el tipo de documento de acuerdo al codigo de aplicacion
                TipoDoc = HomologarCampo(request.P_NIDDOC_TYPE, request.P_CodAplicacion, "DOCIDE");

                if (request.P_SIDDOC.Length != 8)
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El número de documento debe tener 8 caracteres.";
                    return Ok(response);
                }

                if (request != null)
                {
                    switch (request.P_CodAplicacion.ToUpper())
                    {

                        default:

                            if (ArrayAplicacion.Contains(request.P_CodAplicacion.ToUpper()))
                            {
                                if (request.P_CodAplicacion.ToUpper() == "REGISTRORM" || request.P_CodAplicacion.ToUpper() == "REGISTROR")
                                {
                                }
                                else
                                {
                                    if (request.EListAddresClient != null)
                                    {
                                        request.EListAddresClient = SetRecowner(request.EListAddresClient);
                                    }
                                }
                                //response = ValidarCamposCliente(request);

                                switch (request.P_TipOper.ToUpper())
                                {
                                    case "CON":
                                        response = ConsultaCore.Consultar(request);
                                        if (response.P_NCODE == "0")
                                        {
                                            response.EListClient = new List<ClientViewModel>();
                                            if (request.P_CONSUMESERV_IND != "1")
                                            {
                                                //response.EListClient = ConsultaCore.ConsultarCliente(request);
                                            }
                                            if (response.EListClient.Count == 0)
                                            {
                                                if (request.P_NROL != "104")
                                                {
                                                    if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "2")
                                                    {
                                                        try
                                                        {
                                                            try
                                                            {
                                                                string responseR = "";
                                                                var ExisteLocal = false;
                                                                // responseReniec = ObtenerClientReniecLocal(request, out ExisteLocal);
                                                                if (ExisteLocal != true)
                                                                {
                                                                    responseR = ServiceCore.ConsultarCliente(request.P_SIDDOC, "UrlService");
                                                                    responseReniec = JsonConvert.DeserializeObject<ResponseReniecViewModel>(responseR, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                                                    AddClientReniec(responseReniec);
                                                                }
                                                                if (responseReniec.CODIGOERROR != "0000")
                                                                {
                                                                    responseReniec.CODIGOERROR = "0004";
                                                                    response.P_NCODE = "3";
                                                                    response.P_SMESSAGE = "No se encontro informacion .";
                                                                    return Ok(response);
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                responseReniec.CODIGOERROR = "0004";
                                                                response.P_NCODE = "3";
                                                                response.P_SMESSAGE = "No se encontro informacion .";
                                                                return Ok(response);

                                                            }

                                                            if (responseReniec.CODIGOERROR == "0000")
                                                            {
                                                                var itemCliente = new ClientViewModel
                                                                {
                                                                    P_NIDDOC_TYPE = "2",
                                                                    P_SIDDOC = responseReniec.NUMERODNI,
                                                                    P_DIG_VERIFICACION = responseReniec.DIGITOVERIFICACION,
                                                                    P_SLASTNAME = responseReniec.APELLIDOPATERNO.Trim(),
                                                                    P_SLASTNAME2 = responseReniec.APELLIDOMATERNO.Trim(),
                                                                    P_APELLIDO_CASADA = responseReniec.APELLIDOCASADA.Trim(),
                                                                    P_SFIRSTNAME = responseReniec.NOMBRES.Trim()
                                                                };

                                                                if (responseReniec.ESTADOCIVILCIUDADANO == "1")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "2";
                                                                }
                                                                else if (responseReniec.ESTADOCIVILCIUDADANO == "2")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_NCIVILSTA = responseReniec.ESTADOCIVILCIUDADANO;
                                                                }

                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = HomologarCampo(responseReniec.ESTADOCIVILCIUDADANO, request.P_CodAplicacion, "RESTADOCIVIL");
                                                                }

                                                                itemCliente.P_SGRADO_INSTRUCCION = responseReniec.CODIGOGRADOINSTRUCCION;
                                                                itemCliente.P_NHEIGHT = responseReniec.ESTATURA;

                                                                if (responseReniec.SEXO == "1")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "2";
                                                                }
                                                                else if (responseReniec.SEXO == "2")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = responseReniec.SEXO;
                                                                }
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = HomologarCampo(responseReniec.SEXO, request.P_CodAplicacion, "RSEXO");
                                                                }
                                                                itemCliente.P_ORIGEN_DATA = "RENIEC";
                                                                // itemCliente.P_NNATIONALITY = "604";
                                                                itemCliente.P_NNATIONALITY = "1";
                                                                itemCliente.P_SSISTEMA = request.P_SSISTEMA;
                                                                itemCliente.P_TI_DOC_SUSTENT = responseReniec.TIPODOCUMENTOIDENTIDAD;
                                                                itemCliente.P_NU_DOC_SUSTENT = responseReniec.NUMERODOCUMENTOIDENTIDAD.Trim();
                                                                itemCliente.P_COD_UBIG_DEP_NAC = responseReniec.CODIGOUBIGEODEPARTAMENTONACIMIENTO;
                                                                itemCliente.P_COD_UBIG_PROV_NAC = responseReniec.CODIGOUBIGEOPROVINCIANACIMIENTO;
                                                                itemCliente.P_COD_UBIG_DIST_NAC = responseReniec.CODIGOUBIGEODISTRITONACIMIENTO;
                                                                itemCliente.P_DEPARTAMENTO_NACIMIENTO = responseReniec.DEPARTAMENTONACIMIENTO.Trim();
                                                                itemCliente.P_PROVINCIA_NACIMIENTO = responseReniec.PROVINCIANACIMIENTO.Trim();
                                                                itemCliente.P_DISTRITO_NACIMIENTO = responseReniec.DISTRITONACIMIENTO.Trim();
                                                                var fechaNacimiento = DateTime.ParseExact(responseReniec.FECHANACIMIENTO, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_DBIRTHDAT = fechaNacimiento.ToString("dd/MM/yyyy");
                                                                itemCliente.P_NOMBRE_PADRE = responseReniec.NOMBRESPADRE.Trim();
                                                                itemCliente.P_NOMBRE_MADRE = responseReniec.NOMBRESMADRE.Trim();
                                                                var fechaInscripcion = DateTime.ParseExact(responseReniec.FECHAINSCRIPCION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_INSC = fechaInscripcion.ToString("dd/MM/yyyy");
                                                                var fechaEmision = DateTime.ParseExact(responseReniec.FECHAEMISION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_EXPEDICION = fechaEmision.ToString("dd/MM/yyyy");
                                                                itemCliente.P_CONSTANCIA_VOTACION = responseReniec.CONSTANCIAVOTACION;
                                                                itemCliente.P_RESTRICCION = responseReniec.RESTRICCIONES.Trim();
                                                                itemCliente.P_NTITLE = "99";
                                                                itemCliente.P_NSPECIALITY = "99";
                                                                itemCliente.P_SISRENIEC_IND = "1";
                                                                itemCliente.P_SFOTO = responseReniec.FOTO;
                                                                itemCliente.P_SFIRMA = responseReniec.FIRMA;

                                                                itemCliente.EListAddresClient = new List<AddressViewModel>();
                                                                var itemDireccion = new AddressViewModel
                                                                {
                                                                    // itemDireccion.P_NCOUNTRY = "604";
                                                                    P_NCOUNTRY = "1",
                                                                    P_DESTIDIRE = "Particular",
                                                                    P_SRECTYPE = "2",
                                                                    P_SCOD_DEP_UBI_DOM = responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO,
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_SCOD_PRO_UBI_DOM = responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_SCOD_DIS_UBI_DOM = responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_SDES_DEP_DOM = responseReniec.DEPARTAMENTODOMICILIO.Trim(),
                                                                    P_SDES_PRO_DOM = responseReniec.PROVINCIADOMICILIO.Trim(),
                                                                    P_SDES_DIS_DOM = responseReniec.DISTRITODOMICILIO.Trim(),
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.BLOCKCHALET.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim()
                                                                };
                                                                //Direccion completa
                                                                var param = new DireccionCompletaBindingModel
                                                                {
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_SREFERENCIA = "",
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO
                                                                };
                                                                var result = InsertaCore.DireccionCompleta(param);
                                                                itemDireccion.P_SDESDIREBUSQ = result.P_SDESDIREBUSQ;
                                                                itemCliente.EListAddresClient.Add(itemDireccion);
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.EListAddresClient = HomologarUbigeo(itemCliente.EListAddresClient, request.P_CodAplicacion.ToString());
                                                                }
                                                                response.EListClient.Add(itemCliente);

                                                                itemCliente.EListPhoneClient = new List<PhoneViewModel>();
                                                                itemCliente.EListEmailClient = new List<EmailViewModel>();
                                                                itemCliente.EListContactClient = new List<ContactViewModel>();
                                                                itemCliente.EListCIIUClient = new List<CiiuViewModel>();
                                                            }
                                                            else
                                                            {
                                                                // response.P_NCODE = "1";
                                                                if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                                {
                                                                    response.P_NCODE = "1";
                                                                }
                                                                else
                                                                {
                                                                    response.P_NCODE = "3";
                                                                }
                                                                response.P_SMESSAGE = "no se encontro informacion!";
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                            {
                                                                response.P_NCODE = "1";
                                                            }
                                                            else
                                                            {
                                                                response.P_NCODE = "3";
                                                            }
                                                            response.P_SMESSAGE = "no se encontro informacion!";
                                                            //response.P_NCODE = "1";
                                                            //response.P_SMESSAGE = "El dni ingresado NO es válido.";
                                                        }


                                                    }
                                                    else if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "1")
                                                    {
                                                        return Ok(ConsultarSUNAT(request.P_SIDDOC));
                                                    }
                                                    else if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "4")
                                                    {
                                                        return Ok(ConsultarCE(request));
                                                    }

                                                }

                                            }

                                        }
                                        break;
                                }
                            }
                            else
                            {
                                response.P_NCODE = "1";
                                response.P_SMESSAGE = "El tipo de operación enviado no es el correcto.";
                            }
                            break;
                    }



                    if (request.P_CodAplicacion == "EXTERNO1GCI")
                    {
                        return Ok(responseP);
                    }
                    else
                    {
                        //return Ok(responseReniec);
                        return Ok(response);
                    }

                }
                else
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El request que se ha enviado no tiene el formato correcto.";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.P_NCODE = "-1";
                response.P_SMESSAGE = ex.Message;
                return Ok(response);
            }
        }


        [Route("GestionarClienteReniec_Inter")]
        [HttpPost]
        public IHttpActionResult GestionarClienteReniec_Inter(ClientBindingModel request)
        {
            ConsultaCore ConsultaCore = new ConsultaCore();
            InsertaCore InsertaCore = new InsertaCore();
            ServiceCore ServiceCore = new ServiceCore();

            ResponseViewModel response = new ResponseViewModel();
            ResponsePViewModel responseP = new ResponsePViewModel();
            ResponseReniecViewModel responseReniec = new ResponseReniecViewModel();
            string TipoDoc;
            try
            {
                //Se homologa el tipo de documento de acuerdo al codigo de aplicacion
                TipoDoc = HomologarCampo(request.P_NIDDOC_TYPE, request.P_CodAplicacion, "DOCIDE");

                if (request.P_SIDDOC.Length != 8)
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El número de documento debe tener 8 caracteres.";
                    return Ok(response);
                }

                if (request != null)
                {
                    switch (request.P_CodAplicacion.ToUpper())
                    {

                        default:

                            if (ArrayAplicacion.Contains(request.P_CodAplicacion.ToUpper()))
                            {
                                if (request.P_CodAplicacion.ToUpper() == "REGISTRORM" || request.P_CodAplicacion.ToUpper() == "REGISTROR")
                                {
                                }
                                else
                                {
                                    if (request.EListAddresClient != null)
                                    {
                                        request.EListAddresClient = SetRecowner(request.EListAddresClient);
                                    }
                                }
                                //response = ValidarCamposCliente(request);

                                switch (request.P_TipOper.ToUpper())
                                {
                                    case "CON":
                                        response = ConsultaCore.Consultar(request);
                                        if (response.P_NCODE == "0")
                                        {
                                            response.EListClient = new List<ClientViewModel>();
                                            if (request.P_CONSUMESERV_IND != "1")
                                            {
                                                //response.EListClient = ConsultaCore.ConsultarCliente(request);
                                            }
                                            if (response.EListClient.Count == 0)
                                            {
                                                if (request.P_NROL != "104")
                                                {
                                                    if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "2")
                                                    {
                                                        try
                                                        {
                                                            try
                                                            {
                                                                string responseR = "";
                                                                responseReniec = ObtenerClientReniecLocal(request, out bool ExisteLocal);
                                                                if (ExisteLocal != true)
                                                                {
                                                                    responseR = ServiceCore.ConsultarCliente(request.P_SIDDOC, "UrlService");
                                                                    responseReniec = JsonConvert.DeserializeObject<ResponseReniecViewModel>(responseR, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                                                    AddClientReniec(responseReniec);
                                                                }
                                                                if (responseReniec.CODIGOERROR != "0000")
                                                                {
                                                                    responseReniec.CODIGOERROR = "0004";
                                                                    response.P_NCODE = "3";
                                                                    response.P_SMESSAGE = "No se encontro informacion .";
                                                                    return Ok(response);
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                responseReniec.CODIGOERROR = "0004";
                                                                response.P_NCODE = "3";
                                                                response.P_SMESSAGE = "No se encontro informacion .";
                                                                return Ok(response);

                                                            }

                                                            if (responseReniec.CODIGOERROR == "0000")
                                                            {
                                                                var itemCliente = new ClientViewModel
                                                                {
                                                                    P_NIDDOC_TYPE = "2",
                                                                    P_SIDDOC = responseReniec.NUMERODNI,
                                                                    P_DIG_VERIFICACION = responseReniec.DIGITOVERIFICACION,
                                                                    P_SLASTNAME = responseReniec.APELLIDOPATERNO.Trim(),
                                                                    P_SLASTNAME2 = responseReniec.APELLIDOMATERNO.Trim(),
                                                                    P_APELLIDO_CASADA = responseReniec.APELLIDOCASADA.Trim(),
                                                                    P_SFIRSTNAME = responseReniec.NOMBRES.Trim()
                                                                };

                                                                if (responseReniec.ESTADOCIVILCIUDADANO == "1")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "2";
                                                                }
                                                                else if (responseReniec.ESTADOCIVILCIUDADANO == "2")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_NCIVILSTA = responseReniec.ESTADOCIVILCIUDADANO;
                                                                }

                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = HomologarCampo(responseReniec.ESTADOCIVILCIUDADANO, request.P_CodAplicacion, "RESTADOCIVIL");
                                                                }

                                                                itemCliente.P_SGRADO_INSTRUCCION = responseReniec.CODIGOGRADOINSTRUCCION;
                                                                itemCliente.P_NHEIGHT = responseReniec.ESTATURA;

                                                                if (responseReniec.SEXO == "1")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "2";
                                                                }
                                                                else if (responseReniec.SEXO == "2")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = responseReniec.SEXO;
                                                                }
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = HomologarCampo(responseReniec.SEXO, request.P_CodAplicacion, "RSEXO");
                                                                }
                                                                itemCliente.P_ORIGEN_DATA = "RENIEC";
                                                                // itemCliente.P_NNATIONALITY = "604";
                                                                itemCliente.P_NNATIONALITY = "1";
                                                                itemCliente.P_SSISTEMA = request.P_SSISTEMA;
                                                                itemCliente.P_TI_DOC_SUSTENT = responseReniec.TIPODOCUMENTOIDENTIDAD;
                                                                itemCliente.P_NU_DOC_SUSTENT = responseReniec.NUMERODOCUMENTOIDENTIDAD.Trim();
                                                                itemCliente.P_COD_UBIG_DEP_NAC = responseReniec.CODIGOUBIGEODEPARTAMENTONACIMIENTO;
                                                                itemCliente.P_COD_UBIG_PROV_NAC = responseReniec.CODIGOUBIGEOPROVINCIANACIMIENTO;
                                                                itemCliente.P_COD_UBIG_DIST_NAC = responseReniec.CODIGOUBIGEODISTRITONACIMIENTO;
                                                                itemCliente.P_DEPARTAMENTO_NACIMIENTO = responseReniec.DEPARTAMENTONACIMIENTO.Trim();
                                                                itemCliente.P_PROVINCIA_NACIMIENTO = responseReniec.PROVINCIANACIMIENTO.Trim();
                                                                itemCliente.P_DISTRITO_NACIMIENTO = responseReniec.DISTRITONACIMIENTO.Trim();
                                                                var fechaNacimiento = DateTime.ParseExact(responseReniec.FECHANACIMIENTO, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_DBIRTHDAT = fechaNacimiento.ToString("dd/MM/yyyy");
                                                                itemCliente.P_NOMBRE_PADRE = responseReniec.NOMBRESPADRE.Trim();
                                                                itemCliente.P_NOMBRE_MADRE = responseReniec.NOMBRESMADRE.Trim();
                                                                var fechaInscripcion = DateTime.ParseExact(responseReniec.FECHAINSCRIPCION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_INSC = fechaInscripcion.ToString("dd/MM/yyyy");
                                                                var fechaEmision = DateTime.ParseExact(responseReniec.FECHAEMISION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_EXPEDICION = fechaEmision.ToString("dd/MM/yyyy");
                                                                itemCliente.P_CONSTANCIA_VOTACION = responseReniec.CONSTANCIAVOTACION;
                                                                itemCliente.P_RESTRICCION = responseReniec.RESTRICCIONES.Trim();
                                                                itemCliente.P_NTITLE = "99";
                                                                itemCliente.P_NSPECIALITY = "99";
                                                                itemCliente.P_SISRENIEC_IND = "1";

                                                                itemCliente.EListAddresClient = new List<AddressViewModel>();
                                                                var itemDireccion = new AddressViewModel
                                                                {
                                                                    // itemDireccion.P_NCOUNTRY = "604";
                                                                    P_NCOUNTRY = "1",
                                                                    P_DESTIDIRE = "Particular",
                                                                    P_SRECTYPE = "2",
                                                                    P_SCOD_DEP_UBI_DOM = responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO,
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_SCOD_PRO_UBI_DOM = responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_SCOD_DIS_UBI_DOM = responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_SDES_DEP_DOM = responseReniec.DEPARTAMENTODOMICILIO.Trim(),
                                                                    P_SDES_PRO_DOM = responseReniec.PROVINCIADOMICILIO.Trim(),
                                                                    P_SDES_DIS_DOM = responseReniec.DISTRITODOMICILIO.Trim(),
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.BLOCKCHALET.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim()
                                                                };
                                                                //Direccion completa
                                                                var param = new DireccionCompletaBindingModel
                                                                {
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_SREFERENCIA = "",
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO
                                                                };
                                                                var result = InsertaCore.DireccionCompleta(param);
                                                                itemDireccion.P_SDESDIREBUSQ = result.P_SDESDIREBUSQ;
                                                                itemCliente.EListAddresClient.Add(itemDireccion);
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.EListAddresClient = HomologarUbigeo(itemCliente.EListAddresClient, request.P_CodAplicacion.ToString());
                                                                }
                                                                response.EListClient.Add(itemCliente);

                                                                itemCliente.EListPhoneClient = new List<PhoneViewModel>();
                                                                itemCliente.EListEmailClient = new List<EmailViewModel>();
                                                                itemCliente.EListContactClient = new List<ContactViewModel>();
                                                                itemCliente.EListCIIUClient = new List<CiiuViewModel>();
                                                            }
                                                            else
                                                            {
                                                                // response.P_NCODE = "1";
                                                                if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                                {
                                                                    response.P_NCODE = "1";
                                                                }
                                                                else
                                                                {
                                                                    response.P_NCODE = "3";
                                                                }
                                                                response.P_SMESSAGE = "no se encontro informacion!";
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                            {
                                                                response.P_NCODE = "1";
                                                            }
                                                            else
                                                            {
                                                                response.P_NCODE = "3";
                                                            }
                                                            response.P_SMESSAGE = "no se encontro informacion!";
                                                            //response.P_NCODE = "1";
                                                            //response.P_SMESSAGE = "El dni ingresado NO es válido.";
                                                        }


                                                    }
                                                    else if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "1")
                                                    {
                                                        return Ok(ConsultarSUNAT(request.P_SIDDOC));
                                                    }
                                                    else if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "4")
                                                    {
                                                        return Ok(ConsultarCE(request));
                                                    }

                                                }

                                            }

                                        }
                                        break;
                                }
                            }
                            else
                            {
                                response.P_NCODE = "1";
                                response.P_SMESSAGE = "El tipo de operación enviado no es el correcto.";
                            }
                            break;
                    }



                    if (request.P_CodAplicacion == "EXTERNO1GCI")
                    {
                        return Ok(responseP);
                    }
                    else
                    {
                        //return Ok(responseReniec);
                        return Ok(response);
                    }

                }
                else
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El request que se ha enviado no tiene el formato correcto.";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.P_NCODE = "-1";
                response.P_SMESSAGE = ex.Message;
                return Ok(response);
            }
        }


        private List<Message> listMessage(ClientBindingModel request, ApplicationsBindingModel applications)
        {

            List<Config> configs = new List<Config>();

            List<string> Emails = ConfigurationManager.AppSettings["CorreoLAFT"].Split('|').ToList();

            foreach (string email in Emails)
            {

                configs.Add(new Config { Documento = request.P_SIDDOC, Nombres = request.P_SFIRSTNAME, ApellidoMaterno = request.P_SLASTNAME2, ApellidoPaterno = request.P_SLASTNAME, aplicativo = applications.SCOD_APPLICATION, Correo = email.ToString() });
            }
            return new Mail().ComposeMail("0", configs);
        }



        [Route("ValidateClientLAFT")]
        [HttpPost]
        public IHttpActionResult ValidateClientLAFT(ClientBindingModel request)
        {

            LaftRegistroBindingModel _LaftClient = new LaftRegistroBindingModel()
            {
                locked_App = false,
                locked_Message = "",
            };

            var _Aplicattion = new ConsultaCore().ConsultarAplicacionesGC().Where(x => x.SCOD_APPLICATION == request.P_CodAplicacion).FirstOrDefault();
            if (_Aplicattion.NIND_USE_LAFT == "1")
            {
                var ClientLAFT = ConsultaClientLAFT(request.P_SIDDOC);
                if (ClientLAFT.Count > 0)
                {
                    if (ClientLAFT[0].liberado == false)
                    {
                        if (ClientLAFT[0].aplicaciones.Where(x => x.aplicacion.id == _Aplicattion.NID_SISTEMA_LAFT && x.activo).ToList().Count <= 0 &&
                        ClientLAFT[0].configRegistro.aplicaciones.Where(x => x.aplicacion.id == _Aplicattion.NID_SISTEMA_LAFT && x.activo).ToList().Count <= 0)
                        {

                            if (ClientLAFT[0].senial.indAlert)
                            {
                                request.P_SFIRSTNAME = string.IsNullOrEmpty(ClientLAFT[0].nombre) ? "" : ClientLAFT[0].nombre;
                                request.P_SLASTNAME = string.IsNullOrEmpty(ClientLAFT[0].apellidoPaterno) ? "" : ClientLAFT[0].apellidoPaterno;
                                request.P_SLASTNAME2 = string.IsNullOrEmpty(ClientLAFT[0].apellidoMaterno) ? "" : ClientLAFT[0].apellidoMaterno;
                                var Cuerpo = listMessage(request, _Aplicattion);
                                foreach (Message item in Cuerpo)
                                {
                                    try
                                    {

                                        new Mail().SendMail("0", item.Address, item.Subject, item.Body, null);

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                            if (ClientLAFT[0].senial.indError)
                            {
                                ClientLAFT[0].locked_App = true;
                                ClientLAFT[0].locked_Message = WSGCliente.Util.GlobalMessage.ClientLAFT;
                                //response.P_NCODE = "1";
                                //response.P_SMESSAGE = WSGCliente.Util.GlobalMessage.ClientLAFT;
                            }
                        }
                    }
                    _LaftClient = ClientLAFT[0];
                }

            }
            return Ok(_LaftClient);
        }



        public List<LaftRegistroBindingModel> ConsultaClientLAFT(string NroDocumento)
        {
            List<LaftRegistroBindingModel> laftRegistroBindings = new List<LaftRegistroBindingModel>();
            try
            {

                var ListClientLAFT = new ServiceCore().ConsumeServiceComum(Service.LAFT, Method.GET,
                    new string[] { NroDocumento }, true);

                laftRegistroBindings =
                    JsonConvert.DeserializeObject<List<LaftRegistroBindingModel>>(ListClientLAFT, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


                foreach (LaftRegistroBindingModel ClientLAFT in laftRegistroBindings)
                {
                    if (ClientLAFT.id != 0)
                    {
                        var AppClientLAFT = new ServiceCore().ConsumeServiceComum(Service.RegistroLAFT, Method.GET,
                            new string[] { ClientLAFT.id.ToString() }, true);

                        var ClientLAFTBindings =
                            JsonConvert.DeserializeObject<LaftRegistroBindingModel>(AppClientLAFT, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        if (ClientLAFTBindings != null)
                        {
                            ClientLAFT.configRegistro = ClientLAFTBindings.configRegistro;
                            ClientLAFT.aplicaciones = ClientLAFTBindings.aplicaciones;
                        }
                    }
                }
                return laftRegistroBindings;
            }
            catch (Exception ex)
            {
                return laftRegistroBindings;
            }
        }

        [Route("SendSMS")]
        [HttpPost]
        public IHttpActionResult SendSMS(EmBlueBindingModel request)
        {
            ResponseViewModelEmBlue response = new ResponseViewModelEmBlue();
            var rutaEmblue = "http://api.embluemail.com/Services/Emblue3Service.svc/"; //COLOCAR EN APPSETTINGS
            ServiceCore ServiceCore = new ServiceCore();
            var emBlueRes = ServiceCore.SendSMS(request, rutaEmblue);
            return Ok(emBlueRes);
        }


        [Route("GestionarClienteSegmento")]
        [HttpPost]

        public IHttpActionResult GestionarClienteSegmento(ClientBindingModel request)
        {
            ConsultaCore ConsultaCore = new ConsultaCore();
            InsertaCore InsertaCore = new InsertaCore();
            ServiceCore ServiceCore = new ServiceCore();

            ResponseViewModel response = new ResponseViewModel();
            ResponsePViewModel responseP = new ResponsePViewModel();
            ResponseReniecViewModel responseReniec = new ResponseReniecViewModel();
            ResponseViewModelSegmento responseMovigoo = new ResponseViewModelSegmento();
            string TipoDoc;
            try
            {
                //Se homologa el tipo de documento de acuerdo al codigo de aplicacion
                TipoDoc = HomologarCampo(request.P_NIDDOC_TYPE, request.P_CodAplicacion, "DOCIDE");

                if (request != null)
                {
                    switch (request.P_CodAplicacion.ToUpper())
                    {
                        case "EXTERNO1GCI":
                            response = ConsultaCore.Consultar(request);
                            if (response.P_NCODE == "0")
                            {
                                responseP = ConsultaCore.ConsultarProveedor(request);
                                if (responseP.P_NCODE == "0")
                                {
                                    responseP.EListClient = ConsultaCore.ConsultarClienteProveedor(request);
                                    if (responseP.EListClient.Count == 0)
                                    {
                                        responseP.P_SMESSAGE = "No existen registros con los criterios ingresados.";
                                    }
                                }
                            }
                            else
                            {
                                responseP.P_NCODE = response.P_NCODE;
                                responseP.P_SMESSAGE = response.P_SMESSAGE;
                            }

                            break;
                        default:

                            if (ArrayAplicacion.Contains(request.P_CodAplicacion.ToUpper()))
                            {
                                if (request.P_CodAplicacion.ToUpper() == "REGISTRORM" || request.P_CodAplicacion.ToUpper() == "REGISTROR")
                                {
                                }
                                else
                                {
                                    if (request.EListAddresClient != null)
                                    {
                                        request.EListAddresClient = SetRecowner(request.EListAddresClient);
                                    }
                                }
                                //response = ValidarCamposCliente(request);

                                switch (request.P_TipOper.ToUpper())
                                {
                                    case "CON":
                                        response = ConsultaCore.Consultar(request);
                                        if (response.P_NCODE == "0")
                                        {
                                            response.EListClient = new List<ClientViewModel>();
                                            response.EListClient = ConsultaCore.ConsultarCliente(request);

                                            if (response.EListClient.Count == 0)
                                            {
                                                if (request.P_NROL != "104")
                                                {
                                                    if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "2")
                                                    {
                                                        try
                                                        {
                                                            try
                                                            {
                                                                string responseR = "";
                                                                responseReniec = ObtenerClientReniecLocal(request, out bool ExisteLocal);
                                                                if (ExisteLocal != true)
                                                                {
                                                                    responseR = ServiceCore.ConsultarCliente(request.P_SIDDOC, "UrlService");
                                                                    responseReniec = JsonConvert.DeserializeObject<ResponseReniecViewModel>(responseR, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                                                    AddClientReniec(responseReniec);
                                                                }
                                                                if (responseReniec.CODIGOERROR != "0000")
                                                                {
                                                                    responseReniec.CODIGOERROR = "0004";
                                                                    response.P_NCODE = "3";
                                                                    return Ok(response);
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                responseReniec.CODIGOERROR = "0004";
                                                                response.P_NCODE = "3";
                                                                response.P_SMESSAGE = "No se encontro informacion .";
                                                                return Ok(response);

                                                            }

                                                            if (responseReniec.CODIGOERROR == "0000")
                                                            {
                                                                var itemCliente = new ClientViewModel
                                                                {
                                                                    P_NIDDOC_TYPE = "2",
                                                                    P_SIDDOC = responseReniec.NUMERODNI,
                                                                    P_DIG_VERIFICACION = responseReniec.DIGITOVERIFICACION,
                                                                    P_SLASTNAME = responseReniec.APELLIDOPATERNO.Trim(),
                                                                    P_SLASTNAME2 = responseReniec.APELLIDOMATERNO.Trim(),
                                                                    P_APELLIDO_CASADA = responseReniec.APELLIDOCASADA.Trim(),
                                                                    P_SFIRSTNAME = responseReniec.NOMBRES.Trim()
                                                                };

                                                                if (responseReniec.ESTADOCIVILCIUDADANO == "1")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "2";
                                                                }
                                                                else if (responseReniec.ESTADOCIVILCIUDADANO == "2")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_NCIVILSTA = responseReniec.ESTADOCIVILCIUDADANO;
                                                                }

                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = HomologarCampo(responseReniec.ESTADOCIVILCIUDADANO, request.P_CodAplicacion, "RESTADOCIVIL");
                                                                }

                                                                itemCliente.P_SGRADO_INSTRUCCION = responseReniec.CODIGOGRADOINSTRUCCION;
                                                                itemCliente.P_NHEIGHT = responseReniec.ESTATURA;

                                                                if (responseReniec.SEXO == "1")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "2";
                                                                }
                                                                else if (responseReniec.SEXO == "2")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = responseReniec.SEXO;
                                                                }
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = HomologarCampo(responseReniec.SEXO, request.P_CodAplicacion, "RSEXO");
                                                                }
                                                                itemCliente.P_ORIGEN_DATA = "RENIEC";
                                                                itemCliente.P_NNATIONALITY = "1";
                                                                itemCliente.P_SSISTEMA = request.P_SSISTEMA;
                                                                itemCliente.P_TI_DOC_SUSTENT = responseReniec.TIPODOCUMENTOIDENTIDAD;
                                                                itemCliente.P_NU_DOC_SUSTENT = responseReniec.NUMERODOCUMENTOIDENTIDAD.Trim();
                                                                itemCliente.P_COD_UBIG_DEP_NAC = responseReniec.CODIGOUBIGEODEPARTAMENTONACIMIENTO;
                                                                itemCliente.P_COD_UBIG_PROV_NAC = responseReniec.CODIGOUBIGEOPROVINCIANACIMIENTO;
                                                                itemCliente.P_COD_UBIG_DIST_NAC = responseReniec.CODIGOUBIGEODISTRITONACIMIENTO;
                                                                itemCliente.P_DEPARTAMENTO_NACIMIENTO = responseReniec.DEPARTAMENTONACIMIENTO.Trim();
                                                                itemCliente.P_PROVINCIA_NACIMIENTO = responseReniec.PROVINCIANACIMIENTO.Trim();
                                                                itemCliente.P_DISTRITO_NACIMIENTO = responseReniec.DISTRITONACIMIENTO.Trim();
                                                                var fechaNacimiento = DateTime.ParseExact(responseReniec.FECHANACIMIENTO, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_DBIRTHDAT = fechaNacimiento.ToString("dd/MM/yyyy");
                                                                itemCliente.P_NOMBRE_PADRE = responseReniec.NOMBRESPADRE.Trim();
                                                                itemCliente.P_NOMBRE_MADRE = responseReniec.NOMBRESMADRE.Trim();
                                                                var fechaInscripcion = DateTime.ParseExact(responseReniec.FECHAINSCRIPCION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_INSC = fechaInscripcion.ToString("dd/MM/yyyy");
                                                                var fechaEmision = DateTime.ParseExact(responseReniec.FECHAEMISION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_EXPEDICION = fechaEmision.ToString("dd/MM/yyyy");
                                                                itemCliente.P_CONSTANCIA_VOTACION = responseReniec.CONSTANCIAVOTACION;
                                                                itemCliente.P_RESTRICCION = responseReniec.RESTRICCIONES.Trim();
                                                                itemCliente.P_NTITLE = "99";
                                                                itemCliente.P_NSPECIALITY = "99";
                                                                itemCliente.P_SISRENIEC_IND = "1";

                                                                itemCliente.EListAddresClient = new List<AddressViewModel>();
                                                                var itemDireccion = new AddressViewModel
                                                                {
                                                                    P_NCOUNTRY = "1",
                                                                    P_DESTIDIRE = "Particular",
                                                                    P_SRECTYPE = "2",
                                                                    P_SCOD_DEP_UBI_DOM = responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO,
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_SCOD_PRO_UBI_DOM = responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_SCOD_DIS_UBI_DOM = responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_SDES_DEP_DOM = responseReniec.DEPARTAMENTODOMICILIO.Trim(),
                                                                    P_SDES_PRO_DOM = responseReniec.PROVINCIADOMICILIO.Trim(),
                                                                    P_SDES_DIS_DOM = responseReniec.DISTRITODOMICILIO.Trim(),
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.BLOCKCHALET.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim()
                                                                };
                                                                //Direccion completa
                                                                var param = new DireccionCompletaBindingModel
                                                                {
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_SREFERENCIA = "",
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO
                                                                };
                                                                var result = InsertaCore.DireccionCompleta(param);
                                                                itemDireccion.P_SDESDIREBUSQ = result.P_SDESDIREBUSQ;
                                                                itemCliente.EListAddresClient.Add(itemDireccion);
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.EListAddresClient = HomologarUbigeo(itemCliente.EListAddresClient, request.P_CodAplicacion.ToString());
                                                                }
                                                                response.EListClient.Add(itemCliente);

                                                                itemCliente.EListPhoneClient = new List<PhoneViewModel>();
                                                                itemCliente.EListEmailClient = new List<EmailViewModel>();
                                                                itemCliente.EListContactClient = new List<ContactViewModel>();
                                                                itemCliente.EListCIIUClient = new List<CiiuViewModel>();
                                                            }
                                                            else
                                                            {
                                                                // response.P_NCODE = "1";
                                                                if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                                {
                                                                    response.P_NCODE = "1";
                                                                }
                                                                else
                                                                {
                                                                    response.P_NCODE = "3";
                                                                }
                                                                response.P_SMESSAGE = "no se encontro informacion!";
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                            {
                                                                response.P_NCODE = "1";
                                                            }
                                                            else
                                                            {
                                                                response.P_NCODE = "3";
                                                            }
                                                            response.P_SMESSAGE = "no se encontro informacion!";
                                                            //response.P_NCODE = "1";
                                                            //response.P_SMESSAGE = "El dni ingresado NO es válido.";
                                                        }


                                                    }
                                                    else if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "1")
                                                    {
                                                        return Ok(ConsultarSUNAT(request.P_SIDDOC));
                                                    }

                                                }

                                            }
                                            else
                                            {
                                                foreach (var item in response.EListClient)
                                                {
                                                    item.P_SSISTEMA = request.P_SSISTEMA;
                                                    item.P_ORIGEN_DATA = "VTIME";
                                                    if (request.P_CodAplicacion == "FIDELIZACION")
                                                    {
                                                        item.P_SSEXCLIEN = HomologarCampo(item.P_SSEXCLIEN, request.P_CodAplicacion, "RSEXO");
                                                        item.P_NCIVILSTA = HomologarCampo(item.P_NCIVILSTA, request.P_CodAplicacion, "RESTADOCIVIL");
                                                        List<AddressViewModel> ListAddress = ConsultaCore.ConsultarClienteDireccion(item.P_SCLIENT);
                                                        item.EListAddresClient = HomologarUbigeo(ListAddress, request.P_CodAplicacion);
                                                    }
                                                    else
                                                    {
                                                        item.EListAddresClient = ConsultaCore.ConsultarClienteDireccion(item.P_SCLIENT);
                                                    }
                                                    item.EListPhoneClient = ConsultaCore.ConsultarClienteTelefono(item.P_SCLIENT);
                                                    item.EListEmailClient = ConsultaCore.ConsultarClienteEmail(item.P_SCLIENT);
                                                    item.EListContactClient = ConsultaCore.ConsultarClienteContacto(item.P_SCLIENT, item.P_NPERSON_TYP);
                                                    item.EListCIIUClient = ConsultaCore.ConsultarClienteCiiu(item.P_SCLIENT);
                                                    item.EListHistoryClient = ConsultaCore.ConsultarClienteHistory(item.P_SCLIENT);
                                                }
                                            }

                                        }
                                        break;
                                    case "INS":
                                        response = EjecutarStore(request);
                                        break;
                                }
                            }
                            else
                            {
                                response.P_NCODE = "1";
                                response.P_SMESSAGE = "El tipo de operación enviado no es el correcto.";
                            }
                            break;
                    }



                    if (request.P_CodAplicacion == "EXTERNO1GCI")
                    {
                        responseMovigoo.P_NCODE = responseP.P_NCODE;
                        responseMovigoo.P_SMESSAGE = responseP.P_SMESSAGE;

                        if (responseP.EListClient != null)
                        {

                            if (responseP.EListClient.Count == 1)
                            {

                                if (responseMovigoo.EListClient_P_DESPRODUCTO == "COLABORADORES")
                                {

                                    responseMovigoo.P_SMESSAGE = "No existen registros con los criterios ingresados.";
                                }
                                else
                                {

                                    responseMovigoo.P_SMESSAGE = "";
                                    responseMovigoo.EListClient_P_DESDOC_TYPE = responseP.EListClient[0].P_DESDOC_TYPE;
                                    responseMovigoo.EListClient_P_SIDDOC = responseP.EListClient[0].P_SIDDOC;
                                    responseMovigoo.EListClient_P_SFIRSTNAME = responseP.EListClient[0].P_SFIRSTNAME;
                                    responseMovigoo.EListClient_P_SLASTNAME = responseP.EListClient[0].P_SLASTNAME;
                                    responseMovigoo.EListClient_P_SLASTNAME2 = responseP.EListClient[0].P_SLASTNAME2;
                                    responseMovigoo.EListClient_P_DESSEXCLIEN = responseP.EListClient[0].P_DESSEXCLIEN;
                                    responseMovigoo.EListClient_P_DESCIVILSTA = responseP.EListClient[0].P_DESCIVILSTA;
                                    responseMovigoo.EListClient_P_NCLIENT_SEG = responseP.EListClient[0].P_NCLIENT_SEG;
                                    responseMovigoo.EListClient_P_NCLIENT_SEG_DESCRIP = responseP.EListClient[0].P_NCLIENT_SEG_DESCRIP;

                                    /*AGRUPAMIENTO DE RENTAS*/
                                    responseMovigoo.EListClient_P_DESPRODUCTO = responseP.EListClient[0].P_DESPRODUCTO
                                        .Replace("COLABORADORES | ", "")
                                        .Replace(" | COLABORADORES | ", "")
                                        .Replace(" | COLABORADORES", "");

                                    var sepSegmentos = responseMovigoo.EListClient_P_DESPRODUCTO.Split('|');
                                    List<string> resAgrupado = new List<string>();
                                    foreach (var segmento in sepSegmentos)
                                    {
                                        if (segmento.Trim().Contains("RENTA"))
                                        {
                                            resAgrupado.Add("RENTAS");
                                        }
                                        else if (segmento.Trim().Contains("SOAT"))
                                        {
                                            resAgrupado.Add("SOAT");
                                        }
                                        else
                                        {
                                            resAgrupado.Add(segmento.Trim());
                                        }
                                    }
                                    resAgrupado.Sort();
                                    responseMovigoo.EListClient_P_DESPRODUCTO = string.Join(" | ", resAgrupado.ToArray());
                                }
                            }
                        }
                        return Ok(responseMovigoo);
                    }
                    else
                    {
                        responseMovigoo.P_NCODE = response.P_NCODE;
                        responseMovigoo.P_SMESSAGE = response.P_SMESSAGE;
                        responseMovigoo.EListClient_P_DESDOC_TYPE = response.EListClient[0].P_NIDDOC_TYPE;
                        responseMovigoo.EListClient_P_SIDDOC = response.EListClient[0].P_SIDDOC;
                        responseMovigoo.EListClient_P_SFIRSTNAME = response.EListClient[0].P_SFIRSTNAME;
                        responseMovigoo.EListClient_P_SLASTNAME = response.EListClient[0].P_SLASTNAME;
                        responseMovigoo.EListClient_P_SLASTNAME2 = response.EListClient[0].P_SLASTNAME2;
                        responseMovigoo.EListClient_P_DESSEXCLIEN = response.EListClient[0].P_SSEXCLIEN;
                        responseMovigoo.EListClient_P_DESCIVILSTA = response.EListClient[0].P_NCIVILSTA;
                        responseMovigoo.EListClient_P_DESPRODUCTO = "";
                        return Ok(responseMovigoo);
                    }

                }
                else
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El request que se ha enviado no tiene el formato correcto.";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.P_NCODE = "-1";
                response.P_SMESSAGE = ex.Message;
                return Ok(response);
            }
        }

        [Route("GestionarClienteMovigoo")]
        [HttpPost]

        public IHttpActionResult GestionarClienteMovigoo(ClientBindingModel request)
        {
            ConsultaCore ConsultaCore = new ConsultaCore();
            InsertaCore InsertaCore = new InsertaCore();
            ServiceCore ServiceCore = new ServiceCore();

            ResponseViewModel response = new ResponseViewModel();
            ResponsePViewModel responseP = new ResponsePViewModel();
            ResponseReniecViewModel responseReniec = new ResponseReniecViewModel();
            ResponseViewModelMovigoo responseMovigoo = new ResponseViewModelMovigoo();
            string TipoDoc;
            try
            {
                //Se homologa el tipo de documento de acuerdo al codigo de aplicacion
                TipoDoc = HomologarCampo(request.P_NIDDOC_TYPE, request.P_CodAplicacion, "DOCIDE");

                if (request != null)
                {
                    switch (request.P_CodAplicacion.ToUpper())
                    {
                        case "EXTERNO1GCI":
                            response = ConsultaCore.Consultar(request);
                            if (response.P_NCODE == "0")
                            {
                                responseP = ConsultaCore.ConsultarProveedor(request);
                                if (responseP.P_NCODE == "0")
                                {
                                    responseP.EListClient = ConsultaCore.ConsultarClienteProveedor(request);
                                    if (responseP.EListClient.Count == 0)
                                    {
                                        responseP.P_SMESSAGE = "No existen registros con los criterios ingresados.";
                                    }
                                }
                            }
                            else
                            {
                                responseP.P_NCODE = response.P_NCODE;
                                responseP.P_SMESSAGE = response.P_SMESSAGE;
                            }

                            break;
                        default:

                            if (ArrayAplicacion.Contains(request.P_CodAplicacion.ToUpper()))
                            {
                                if (request.P_CodAplicacion.ToUpper() == "REGISTRORM" || request.P_CodAplicacion.ToUpper() == "REGISTROR")
                                {
                                }
                                else
                                {
                                    if (request.EListAddresClient != null)
                                    {
                                        request.EListAddresClient = SetRecowner(request.EListAddresClient);
                                    }
                                }
                                //response = ValidarCamposCliente(request);

                                switch (request.P_TipOper.ToUpper())
                                {
                                    case "CON":
                                        response = ConsultaCore.Consultar(request);
                                        if (response.P_NCODE == "0")
                                        {
                                            response.EListClient = new List<ClientViewModel>();
                                            response.EListClient = ConsultaCore.ConsultarCliente(request);

                                            if (response.EListClient.Count == 0)
                                            {
                                                if (request.P_NROL != "104")
                                                {
                                                    if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "2")
                                                    {
                                                        try
                                                        {
                                                            try
                                                            {
                                                                string responseR = "";
                                                                responseReniec = ObtenerClientReniecLocal(request, out bool ExisteLocal);
                                                                if (ExisteLocal != true)
                                                                {
                                                                    responseR = ServiceCore.ConsultarCliente(request.P_SIDDOC, "UrlService");
                                                                    responseReniec = JsonConvert.DeserializeObject<ResponseReniecViewModel>(responseR, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                                                    AddClientReniec(responseReniec);
                                                                }
                                                                if (responseReniec.CODIGOERROR != "0000")
                                                                {
                                                                    responseReniec.CODIGOERROR = "0004";
                                                                    response.P_NCODE = "3";
                                                                    return Ok(response);
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                responseReniec.CODIGOERROR = "0004";
                                                                response.P_NCODE = "3";
                                                                response.P_SMESSAGE = "No se encontro informacion .";
                                                                return Ok(response);

                                                            }

                                                            if (responseReniec.CODIGOERROR == "0000")
                                                            {
                                                                var itemCliente = new ClientViewModel
                                                                {
                                                                    P_NIDDOC_TYPE = "2",
                                                                    P_SIDDOC = responseReniec.NUMERODNI,
                                                                    P_DIG_VERIFICACION = responseReniec.DIGITOVERIFICACION,
                                                                    P_SLASTNAME = responseReniec.APELLIDOPATERNO.Trim(),
                                                                    P_SLASTNAME2 = responseReniec.APELLIDOMATERNO.Trim(),
                                                                    P_APELLIDO_CASADA = responseReniec.APELLIDOCASADA.Trim(),
                                                                    P_SFIRSTNAME = responseReniec.NOMBRES.Trim()
                                                                };

                                                                if (responseReniec.ESTADOCIVILCIUDADANO == "1")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "2";
                                                                }
                                                                else if (responseReniec.ESTADOCIVILCIUDADANO == "2")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_NCIVILSTA = responseReniec.ESTADOCIVILCIUDADANO;
                                                                }

                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = HomologarCampo(responseReniec.ESTADOCIVILCIUDADANO, request.P_CodAplicacion, "RESTADOCIVIL");
                                                                }

                                                                itemCliente.P_SGRADO_INSTRUCCION = responseReniec.CODIGOGRADOINSTRUCCION;
                                                                itemCliente.P_NHEIGHT = responseReniec.ESTATURA;

                                                                if (responseReniec.SEXO == "1")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "2";
                                                                }
                                                                else if (responseReniec.SEXO == "2")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = responseReniec.SEXO;
                                                                }
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = HomologarCampo(responseReniec.SEXO, request.P_CodAplicacion, "RSEXO");
                                                                }
                                                                itemCliente.P_ORIGEN_DATA = "RENIEC";
                                                                itemCliente.P_NNATIONALITY = "1";
                                                                itemCliente.P_SSISTEMA = request.P_SSISTEMA;
                                                                itemCliente.P_TI_DOC_SUSTENT = responseReniec.TIPODOCUMENTOIDENTIDAD;
                                                                itemCliente.P_NU_DOC_SUSTENT = responseReniec.NUMERODOCUMENTOIDENTIDAD.Trim();
                                                                itemCliente.P_COD_UBIG_DEP_NAC = responseReniec.CODIGOUBIGEODEPARTAMENTONACIMIENTO;
                                                                itemCliente.P_COD_UBIG_PROV_NAC = responseReniec.CODIGOUBIGEOPROVINCIANACIMIENTO;
                                                                itemCliente.P_COD_UBIG_DIST_NAC = responseReniec.CODIGOUBIGEODISTRITONACIMIENTO;
                                                                itemCliente.P_DEPARTAMENTO_NACIMIENTO = responseReniec.DEPARTAMENTONACIMIENTO.Trim();
                                                                itemCliente.P_PROVINCIA_NACIMIENTO = responseReniec.PROVINCIANACIMIENTO.Trim();
                                                                itemCliente.P_DISTRITO_NACIMIENTO = responseReniec.DISTRITONACIMIENTO.Trim();
                                                                var fechaNacimiento = DateTime.ParseExact(responseReniec.FECHANACIMIENTO, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_DBIRTHDAT = fechaNacimiento.ToString("dd/MM/yyyy");
                                                                itemCliente.P_NOMBRE_PADRE = responseReniec.NOMBRESPADRE.Trim();
                                                                itemCliente.P_NOMBRE_MADRE = responseReniec.NOMBRESMADRE.Trim();
                                                                var fechaInscripcion = DateTime.ParseExact(responseReniec.FECHAINSCRIPCION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_INSC = fechaInscripcion.ToString("dd/MM/yyyy");
                                                                var fechaEmision = DateTime.ParseExact(responseReniec.FECHAEMISION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_EXPEDICION = fechaEmision.ToString("dd/MM/yyyy");
                                                                itemCliente.P_CONSTANCIA_VOTACION = responseReniec.CONSTANCIAVOTACION;
                                                                itemCliente.P_RESTRICCION = responseReniec.RESTRICCIONES.Trim();
                                                                itemCliente.P_NTITLE = "99";
                                                                itemCliente.P_NSPECIALITY = "99";
                                                                itemCliente.P_SISRENIEC_IND = "1";

                                                                itemCliente.EListAddresClient = new List<AddressViewModel>();
                                                                var itemDireccion = new AddressViewModel
                                                                {
                                                                    P_NCOUNTRY = "1",
                                                                    P_DESTIDIRE = "Particular",
                                                                    P_SRECTYPE = "2",
                                                                    P_SCOD_DEP_UBI_DOM = responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO,
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_SCOD_PRO_UBI_DOM = responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_SCOD_DIS_UBI_DOM = responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_SDES_DEP_DOM = responseReniec.DEPARTAMENTODOMICILIO.Trim(),
                                                                    P_SDES_PRO_DOM = responseReniec.PROVINCIADOMICILIO.Trim(),
                                                                    P_SDES_DIS_DOM = responseReniec.DISTRITODOMICILIO.Trim(),
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.BLOCKCHALET.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim()
                                                                };
                                                                //Direccion completa
                                                                var param = new DireccionCompletaBindingModel
                                                                {
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_SREFERENCIA = "",
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO
                                                                };
                                                                var result = InsertaCore.DireccionCompleta(param);
                                                                itemDireccion.P_SDESDIREBUSQ = result.P_SDESDIREBUSQ;
                                                                itemCliente.EListAddresClient.Add(itemDireccion);
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.EListAddresClient = HomologarUbigeo(itemCliente.EListAddresClient, request.P_CodAplicacion.ToString());
                                                                }
                                                                response.EListClient.Add(itemCliente);

                                                                itemCliente.EListPhoneClient = new List<PhoneViewModel>();
                                                                itemCliente.EListEmailClient = new List<EmailViewModel>();
                                                                itemCliente.EListContactClient = new List<ContactViewModel>();
                                                                itemCliente.EListCIIUClient = new List<CiiuViewModel>();
                                                            }
                                                            else
                                                            {
                                                                // response.P_NCODE = "1";
                                                                if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                                {
                                                                    response.P_NCODE = "1";
                                                                }
                                                                else
                                                                {
                                                                    response.P_NCODE = "3";
                                                                }
                                                                response.P_SMESSAGE = "no se encontro informacion!";
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                            {
                                                                response.P_NCODE = "1";
                                                            }
                                                            else
                                                            {
                                                                response.P_NCODE = "3";
                                                            }
                                                            response.P_SMESSAGE = "no se encontro informacion!";
                                                            //response.P_NCODE = "1";
                                                            //response.P_SMESSAGE = "El dni ingresado NO es válido.";
                                                        }


                                                    }
                                                    else if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "1")
                                                    {
                                                        return Ok(ConsultarSUNAT(request.P_SIDDOC));
                                                    }

                                                }

                                            }
                                            else
                                            {
                                                foreach (var item in response.EListClient)
                                                {
                                                    item.P_SSISTEMA = request.P_SSISTEMA;
                                                    item.P_ORIGEN_DATA = "VTIME";
                                                    if (request.P_CodAplicacion == "FIDELIZACION")
                                                    {
                                                        item.P_SSEXCLIEN = HomologarCampo(item.P_SSEXCLIEN, request.P_CodAplicacion, "RSEXO");
                                                        item.P_NCIVILSTA = HomologarCampo(item.P_NCIVILSTA, request.P_CodAplicacion, "RESTADOCIVIL");
                                                        List<AddressViewModel> ListAddress = ConsultaCore.ConsultarClienteDireccion(item.P_SCLIENT);
                                                        item.EListAddresClient = HomologarUbigeo(ListAddress, request.P_CodAplicacion);
                                                    }
                                                    else
                                                    {
                                                        item.EListAddresClient = ConsultaCore.ConsultarClienteDireccion(item.P_SCLIENT);
                                                    }
                                                    item.EListPhoneClient = ConsultaCore.ConsultarClienteTelefono(item.P_SCLIENT);
                                                    item.EListEmailClient = ConsultaCore.ConsultarClienteEmail(item.P_SCLIENT);
                                                    item.EListContactClient = ConsultaCore.ConsultarClienteContacto(item.P_SCLIENT, item.P_NPERSON_TYP);
                                                    item.EListCIIUClient = ConsultaCore.ConsultarClienteCiiu(item.P_SCLIENT);
                                                    item.EListHistoryClient = ConsultaCore.ConsultarClienteHistory(item.P_SCLIENT);
                                                }
                                            }

                                        }
                                        break;
                                    case "INS":
                                        response = EjecutarStore(request);
                                        break;
                                }
                            }
                            else
                            {
                                response.P_NCODE = "1";
                                response.P_SMESSAGE = "El tipo de operación enviado no es el correcto.";
                            }
                            break;
                    }



                    if (request.P_CodAplicacion == "EXTERNO1GCI")
                    {
                        responseMovigoo.P_NCODE = responseP.P_NCODE;
                        responseMovigoo.P_SMESSAGE = responseP.P_SMESSAGE;

                        if (responseP.EListClient != null)
                        {

                            if (responseP.EListClient.Count == 1)
                            {

                                if (responseMovigoo.EListClient_P_DESPRODUCTO == "COLABORADORES")
                                {

                                    responseMovigoo.P_SMESSAGE = "No existen registros con los criterios ingresados.";
                                }
                                else
                                {

                                    responseMovigoo.P_SMESSAGE = "";
                                    responseMovigoo.EListClient_P_DESDOC_TYPE = responseP.EListClient[0].P_DESDOC_TYPE;
                                    responseMovigoo.EListClient_P_SIDDOC = responseP.EListClient[0].P_SIDDOC;
                                    responseMovigoo.EListClient_P_SFIRSTNAME = responseP.EListClient[0].P_SFIRSTNAME;
                                    responseMovigoo.EListClient_P_SLASTNAME = responseP.EListClient[0].P_SLASTNAME;
                                    responseMovigoo.EListClient_P_SLASTNAME2 = responseP.EListClient[0].P_SLASTNAME2;
                                    responseMovigoo.EListClient_P_DESSEXCLIEN = responseP.EListClient[0].P_DESSEXCLIEN;
                                    responseMovigoo.EListClient_P_DESCIVILSTA = responseP.EListClient[0].P_DESCIVILSTA;
                                    //responseMovigoo.EListClient_P_NCLIENT_SEG = responseP.EListClient[0].P_NCLIENT_SEG;
                                    //responseMovigoo.EListClient_P_NCLIENT_SEG_DESCRIP = responseP.EListClient[0].P_NCLIENT_SEG_DESCRIP;

                                    /*AGRUPAMIENTO DE RENTAS*/
                                    responseMovigoo.EListClient_P_DESPRODUCTO = responseP.EListClient[0].P_DESPRODUCTO
                                        .Replace("COLABORADORES | ", "")
                                        .Replace(" | COLABORADORES | ", "")
                                        .Replace(" | COLABORADORES", "");

                                    var sepSegmentos = responseMovigoo.EListClient_P_DESPRODUCTO.Split('|');
                                    List<string> resAgrupado = new List<string>();
                                    foreach (var segmento in sepSegmentos)
                                    {
                                        if (segmento.Trim().Contains("RENTA"))
                                        {
                                            resAgrupado.Add("RENTAS");
                                        }
                                        else if (segmento.Trim().Contains("SOAT"))
                                        {
                                            resAgrupado.Add("SOAT");
                                        }
                                        else
                                        {
                                            resAgrupado.Add(segmento.Trim());
                                        }
                                    }
                                    resAgrupado.Sort();
                                    responseMovigoo.EListClient_P_DESPRODUCTO = string.Join(" | ", resAgrupado.ToArray());
                                }
                            }
                        }
                        return Ok(responseMovigoo);
                    }
                    else
                    {
                        responseMovigoo.P_NCODE = response.P_NCODE;
                        responseMovigoo.P_SMESSAGE = response.P_SMESSAGE;
                        responseMovigoo.EListClient_P_DESDOC_TYPE = response.EListClient[0].P_NIDDOC_TYPE;
                        responseMovigoo.EListClient_P_SIDDOC = response.EListClient[0].P_SIDDOC;
                        responseMovigoo.EListClient_P_SFIRSTNAME = response.EListClient[0].P_SFIRSTNAME;
                        responseMovigoo.EListClient_P_SLASTNAME = response.EListClient[0].P_SLASTNAME;
                        responseMovigoo.EListClient_P_SLASTNAME2 = response.EListClient[0].P_SLASTNAME2;
                        responseMovigoo.EListClient_P_DESSEXCLIEN = response.EListClient[0].P_SSEXCLIEN;
                        responseMovigoo.EListClient_P_DESCIVILSTA = response.EListClient[0].P_NCIVILSTA;
                        responseMovigoo.EListClient_P_DESPRODUCTO = "";
                        return Ok(responseMovigoo);
                    }

                }
                else
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El request que se ha enviado no tiene el formato correcto.";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.P_NCODE = "-1";
                response.P_SMESSAGE = ex.Message;
                return Ok(response);
            }
        }

        [Route("GestionarClienteVigencia")]
        [HttpPost]

        public IHttpActionResult GestionarClienteVigencia(ClientBindingModel request)
        {
            ConsultaCore ConsultaCore = new ConsultaCore();
            InsertaCore InsertaCore = new InsertaCore();
            ServiceCore ServiceCore = new ServiceCore();

            ResponseViewModel response = new ResponseViewModel();
            ResponsePViewModel responseP = new ResponsePViewModel();
            ResponseReniecViewModel responseReniec = new ResponseReniecViewModel();
            ResponseViewModelMovigoo responseMovigoo = new ResponseViewModelMovigoo();
            string TipoDoc;
            try
            {
                //Se homologa el tipo de documento de acuerdo al codigo de aplicacion
                TipoDoc = HomologarCampo(request.P_NIDDOC_TYPE, request.P_CodAplicacion, "DOCIDE");

                if (request != null)
                {
                    switch (request.P_CodAplicacion.ToUpper())
                    {
                        case "EXTERNO1GCI":
                            request.P_TipOper = "VIGENCIA";
                            response = ConsultaCore.Consultar(request);
                            if (response.P_NCODE == "0")
                            {
                                responseP = ConsultaCore.ConsultarProveedor(request);
                                if (responseP.P_NCODE == "0")
                                {
                                    responseP.EListClient = ConsultaCore.ConsultarClienteProveedor(request);
                                    if (responseP.EListClient.Count == 0)
                                    {
                                        responseP.P_SMESSAGE = "No existen registros con los criterios ingresados.";
                                    }
                                }
                            }
                            else
                            {
                                responseP.P_NCODE = response.P_NCODE;
                                responseP.P_SMESSAGE = response.P_SMESSAGE;
                            }

                            break;
                        default:

                            if (ArrayAplicacion.Contains(request.P_CodAplicacion.ToUpper()))
                            {
                                if (request.P_CodAplicacion.ToUpper() == "REGISTRORM" || request.P_CodAplicacion.ToUpper() == "REGISTROR")
                                {
                                }
                                else
                                {
                                    if (request.EListAddresClient != null)
                                    {
                                        request.EListAddresClient = SetRecowner(request.EListAddresClient);
                                    }
                                }
                                //response = ValidarCamposCliente(request);

                                switch (request.P_TipOper.ToUpper())
                                {
                                    case "CON":
                                        response = ConsultaCore.Consultar(request);
                                        if (response.P_NCODE == "0")
                                        {
                                            response.EListClient = new List<ClientViewModel>();
                                            response.EListClient = ConsultaCore.ConsultarCliente(request);

                                            if (response.EListClient.Count == 0)
                                            {
                                                if (request.P_NROL != "104")
                                                {
                                                    if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "2")
                                                    {
                                                        try
                                                        {
                                                            try
                                                            {
                                                                string responseR = "";
                                                                responseReniec = ObtenerClientReniecLocal(request, out bool ExisteLocal);
                                                                if (ExisteLocal != true)
                                                                {
                                                                    responseR = ServiceCore.ConsultarCliente(request.P_SIDDOC, "UrlService");
                                                                    responseReniec = JsonConvert.DeserializeObject<ResponseReniecViewModel>(responseR, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                                                    AddClientReniec(responseReniec);
                                                                }
                                                                if (responseReniec.CODIGOERROR != "0000")
                                                                {
                                                                    responseReniec.CODIGOERROR = "0004";
                                                                    response.P_NCODE = "3";
                                                                    return Ok(response);
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                responseReniec.CODIGOERROR = "0004";
                                                                response.P_NCODE = "3";
                                                                response.P_SMESSAGE = "No se encontro informacion .";
                                                                return Ok(response);

                                                            }

                                                            if (responseReniec.CODIGOERROR == "0000")
                                                            {
                                                                var itemCliente = new ClientViewModel
                                                                {
                                                                    P_NIDDOC_TYPE = "2",
                                                                    P_SIDDOC = responseReniec.NUMERODNI,
                                                                    P_DIG_VERIFICACION = responseReniec.DIGITOVERIFICACION,
                                                                    P_SLASTNAME = responseReniec.APELLIDOPATERNO.Trim(),
                                                                    P_SLASTNAME2 = responseReniec.APELLIDOMATERNO.Trim(),
                                                                    P_APELLIDO_CASADA = responseReniec.APELLIDOCASADA.Trim(),
                                                                    P_SFIRSTNAME = responseReniec.NOMBRES.Trim()
                                                                };

                                                                if (responseReniec.ESTADOCIVILCIUDADANO == "1")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "2";
                                                                }
                                                                else if (responseReniec.ESTADOCIVILCIUDADANO == "2")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_NCIVILSTA = responseReniec.ESTADOCIVILCIUDADANO;
                                                                }

                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_NCIVILSTA = HomologarCampo(responseReniec.ESTADOCIVILCIUDADANO, request.P_CodAplicacion, "RESTADOCIVIL");
                                                                }

                                                                itemCliente.P_SGRADO_INSTRUCCION = responseReniec.CODIGOGRADOINSTRUCCION;
                                                                itemCliente.P_NHEIGHT = responseReniec.ESTATURA;

                                                                if (responseReniec.SEXO == "1")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "2";
                                                                }
                                                                else if (responseReniec.SEXO == "2")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = "1";
                                                                }
                                                                else
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = responseReniec.SEXO;
                                                                }
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.P_SSEXCLIEN = HomologarCampo(responseReniec.SEXO, request.P_CodAplicacion, "RSEXO");
                                                                }
                                                                itemCliente.P_ORIGEN_DATA = "RENIEC";
                                                                itemCliente.P_NNATIONALITY = "1";
                                                                itemCliente.P_SSISTEMA = request.P_SSISTEMA;
                                                                itemCliente.P_TI_DOC_SUSTENT = responseReniec.TIPODOCUMENTOIDENTIDAD;
                                                                itemCliente.P_NU_DOC_SUSTENT = responseReniec.NUMERODOCUMENTOIDENTIDAD.Trim();
                                                                itemCliente.P_COD_UBIG_DEP_NAC = responseReniec.CODIGOUBIGEODEPARTAMENTONACIMIENTO;
                                                                itemCliente.P_COD_UBIG_PROV_NAC = responseReniec.CODIGOUBIGEOPROVINCIANACIMIENTO;
                                                                itemCliente.P_COD_UBIG_DIST_NAC = responseReniec.CODIGOUBIGEODISTRITONACIMIENTO;
                                                                itemCliente.P_DEPARTAMENTO_NACIMIENTO = responseReniec.DEPARTAMENTONACIMIENTO.Trim();
                                                                itemCliente.P_PROVINCIA_NACIMIENTO = responseReniec.PROVINCIANACIMIENTO.Trim();
                                                                itemCliente.P_DISTRITO_NACIMIENTO = responseReniec.DISTRITONACIMIENTO.Trim();
                                                                var fechaNacimiento = DateTime.ParseExact(responseReniec.FECHANACIMIENTO, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_DBIRTHDAT = fechaNacimiento.ToString("dd/MM/yyyy");
                                                                itemCliente.P_NOMBRE_PADRE = responseReniec.NOMBRESPADRE.Trim();
                                                                itemCliente.P_NOMBRE_MADRE = responseReniec.NOMBRESMADRE.Trim();
                                                                var fechaInscripcion = DateTime.ParseExact(responseReniec.FECHAINSCRIPCION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_INSC = fechaInscripcion.ToString("dd/MM/yyyy");
                                                                var fechaEmision = DateTime.ParseExact(responseReniec.FECHAEMISION, "yyyyMMdd", CultureInfo.InvariantCulture);
                                                                itemCliente.P_FECHA_EXPEDICION = fechaEmision.ToString("dd/MM/yyyy");
                                                                itemCliente.P_CONSTANCIA_VOTACION = responseReniec.CONSTANCIAVOTACION;
                                                                itemCliente.P_RESTRICCION = responseReniec.RESTRICCIONES.Trim();
                                                                itemCliente.P_NTITLE = "99";
                                                                itemCliente.P_NSPECIALITY = "99";
                                                                itemCliente.P_SISRENIEC_IND = "1";

                                                                itemCliente.EListAddresClient = new List<AddressViewModel>();
                                                                var itemDireccion = new AddressViewModel
                                                                {
                                                                    P_NCOUNTRY = "1",
                                                                    P_DESTIDIRE = "Particular",
                                                                    P_SRECTYPE = "2",
                                                                    P_SCOD_DEP_UBI_DOM = responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO,
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_SCOD_PRO_UBI_DOM = responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_SCOD_DIS_UBI_DOM = responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO,
                                                                    P_SDES_DEP_DOM = responseReniec.DEPARTAMENTODOMICILIO.Trim(),
                                                                    P_SDES_PRO_DOM = responseReniec.PROVINCIADOMICILIO.Trim(),
                                                                    P_SDES_DIS_DOM = responseReniec.DISTRITODOMICILIO.Trim(),
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.BLOCKCHALET.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim()
                                                                };
                                                                //Direccion completa
                                                                var param = new DireccionCompletaBindingModel
                                                                {
                                                                    P_STI_DIRE = responseReniec.PREFIJODIRECCION.Trim(),
                                                                    P_SNOM_DIRECCION = responseReniec.DIRECCION.Trim(),
                                                                    P_SNUM_DIRECCION = responseReniec.NUMERODIRECCION.Trim(),
                                                                    P_STI_BLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_SBLOCKCHALET = responseReniec.PREFIJOBLOCKCHALET.Trim(),
                                                                    P_STI_INTERIOR = responseReniec.PREFIJODPTOPISOINTERIOR.Trim(),
                                                                    P_SNUM_INTERIOR = responseReniec.INTERIOR.Trim(),
                                                                    P_STI_CJHT = responseReniec.PREFIJOURBCONDRESID.Trim(),
                                                                    P_SNOM_CJHT = responseReniec.URBANIZACION.Trim(),
                                                                    P_SETAPA = responseReniec.ETAPA.Trim(),
                                                                    P_SMANZANA = responseReniec.MANZANA.Trim(),
                                                                    P_SLOTE = responseReniec.LOTE.Trim(),
                                                                    P_SREFERENCIA = "",
                                                                    P_NPROVINCE = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString(),
                                                                    P_NLOCAL = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO,
                                                                    P_NMUNICIPALITY = Convert.ToInt32(responseReniec.CODIGOUBIGEODEPARTAMENTODOMICILIO).ToString() + responseReniec.CODIGOUBIGEOPROVINCIADOMICILIO + responseReniec.CODIGOUBIGEODISTRITODOMICILIO
                                                                };
                                                                var result = InsertaCore.DireccionCompleta(param);
                                                                itemDireccion.P_SDESDIREBUSQ = result.P_SDESDIREBUSQ;
                                                                itemCliente.EListAddresClient.Add(itemDireccion);
                                                                if (request.P_CodAplicacion.ToString() == "FIDELIZACION")
                                                                {
                                                                    itemCliente.EListAddresClient = HomologarUbigeo(itemCliente.EListAddresClient, request.P_CodAplicacion.ToString());
                                                                }
                                                                response.EListClient.Add(itemCliente);

                                                                itemCliente.EListPhoneClient = new List<PhoneViewModel>();
                                                                itemCliente.EListEmailClient = new List<EmailViewModel>();
                                                                itemCliente.EListContactClient = new List<ContactViewModel>();
                                                                itemCliente.EListCIIUClient = new List<CiiuViewModel>();
                                                            }
                                                            else
                                                            {
                                                                // response.P_NCODE = "1";
                                                                if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                                {
                                                                    response.P_NCODE = "1";
                                                                }
                                                                else
                                                                {
                                                                    response.P_NCODE = "3";
                                                                }
                                                                response.P_SMESSAGE = "no se encontro informacion!";
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            if (request.P_CodAplicacion.ToUpper() == "SEACSA")
                                                            {
                                                                response.P_NCODE = "1";
                                                            }
                                                            else
                                                            {
                                                                response.P_NCODE = "3";
                                                            }
                                                            response.P_SMESSAGE = "no se encontro informacion!";
                                                            //response.P_NCODE = "1";
                                                            //response.P_SMESSAGE = "El dni ingresado NO es válido.";
                                                        }


                                                    }
                                                    else if (request.P_SIDDOC != "" && request.P_NIDDOC_TYPE == "1")
                                                    {
                                                        return Ok(ConsultarSUNAT(request.P_SIDDOC));
                                                    }

                                                }

                                            }
                                            else
                                            {
                                                foreach (var item in response.EListClient)
                                                {
                                                    item.P_SSISTEMA = request.P_SSISTEMA;
                                                    item.P_ORIGEN_DATA = "VTIME";
                                                    if (request.P_CodAplicacion == "FIDELIZACION")
                                                    {
                                                        item.P_SSEXCLIEN = HomologarCampo(item.P_SSEXCLIEN, request.P_CodAplicacion, "RSEXO");
                                                        item.P_NCIVILSTA = HomologarCampo(item.P_NCIVILSTA, request.P_CodAplicacion, "RESTADOCIVIL");
                                                        List<AddressViewModel> ListAddress = ConsultaCore.ConsultarClienteDireccion(item.P_SCLIENT);
                                                        item.EListAddresClient = HomologarUbigeo(ListAddress, request.P_CodAplicacion);
                                                    }
                                                    else
                                                    {
                                                        item.EListAddresClient = ConsultaCore.ConsultarClienteDireccion(item.P_SCLIENT);
                                                    }
                                                    item.EListPhoneClient = ConsultaCore.ConsultarClienteTelefono(item.P_SCLIENT);
                                                    item.EListEmailClient = ConsultaCore.ConsultarClienteEmail(item.P_SCLIENT);
                                                    item.EListContactClient = ConsultaCore.ConsultarClienteContacto(item.P_SCLIENT, item.P_NPERSON_TYP);
                                                    item.EListCIIUClient = ConsultaCore.ConsultarClienteCiiu(item.P_SCLIENT);
                                                    item.EListHistoryClient = ConsultaCore.ConsultarClienteHistory(item.P_SCLIENT);
                                                }
                                            }

                                        }
                                        break;
                                    case "INS":
                                        response = EjecutarStore(request);
                                        break;
                                }
                            }
                            else
                            {
                                response.P_NCODE = "1";
                                response.P_SMESSAGE = "El tipo de operación enviado no es el correcto.";
                            }
                            break;
                    }



                    if (request.P_CodAplicacion == "EXTERNO1GCI")
                    {
                        responseMovigoo.P_NCODE = responseP.P_NCODE;
                        responseMovigoo.P_SMESSAGE = responseP.P_SMESSAGE;

                        if (responseP.EListClient != null)
                        {

                            if (responseP.EListClient.Count == 1)
                            {
                                responseMovigoo.P_SMESSAGE = "";
                                responseMovigoo.EListClient_P_DESDOC_TYPE = responseP.EListClient[0].P_DESDOC_TYPE;
                                responseMovigoo.EListClient_P_SIDDOC = responseP.EListClient[0].P_SIDDOC;
                                responseMovigoo.EListClient_P_SFIRSTNAME = responseP.EListClient[0].P_SFIRSTNAME;
                                responseMovigoo.EListClient_P_SLASTNAME = responseP.EListClient[0].P_SLASTNAME;
                                responseMovigoo.EListClient_P_SLASTNAME2 = responseP.EListClient[0].P_SLASTNAME2;
                                responseMovigoo.EListClient_P_DESSEXCLIEN = responseP.EListClient[0].P_DESSEXCLIEN;
                                responseMovigoo.EListClient_P_DESCIVILSTA = responseP.EListClient[0].P_DESCIVILSTA;


                                var arraMov = responseP.EListClient[0].P_DESPRODUCTO.Split('|');
                                if (arraMov.Length > 1)
                                {
                                    responseMovigoo.EListClient_P_DESPRODUCTO = arraMov[0];
                                    responseMovigoo.EListClient_P_DESPRODUCTO_Finicio = arraMov[1];
                                    responseMovigoo.EListClient_P_DESPRODUCTO_Ffin = arraMov[2];
                                    responseMovigoo.EListClient_P_NPOLICY = arraMov[3];
                                    responseMovigoo.EListClient_P_NCERTIF = arraMov[4];
                                }
                                else
                                {
                                    responseMovigoo.EListClient_P_DESPRODUCTO = arraMov[0];
                                    responseMovigoo.EListClient_P_NPOLICY = arraMov[1];
                                }

                            }
                        }
                        return Ok(responseMovigoo);
                    }
                    else
                    {
                        responseMovigoo.P_NCODE = response.P_NCODE;
                        responseMovigoo.P_SMESSAGE = response.P_SMESSAGE;
                        responseMovigoo.EListClient_P_DESDOC_TYPE = response.EListClient[0].P_NIDDOC_TYPE;
                        responseMovigoo.EListClient_P_SIDDOC = response.EListClient[0].P_SIDDOC;
                        responseMovigoo.EListClient_P_SFIRSTNAME = response.EListClient[0].P_SFIRSTNAME;
                        responseMovigoo.EListClient_P_SLASTNAME = response.EListClient[0].P_SLASTNAME;
                        responseMovigoo.EListClient_P_SLASTNAME2 = response.EListClient[0].P_SLASTNAME2;
                        responseMovigoo.EListClient_P_DESSEXCLIEN = response.EListClient[0].P_SSEXCLIEN;
                        responseMovigoo.EListClient_P_DESCIVILSTA = response.EListClient[0].P_NCIVILSTA;
                        responseMovigoo.EListClient_P_DESPRODUCTO = "";
                        return Ok(responseMovigoo);
                    }

                }
                else
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El request que se ha enviado no tiene el formato correcto.";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.P_NCODE = "-1";
                response.P_SMESSAGE = ex.Message;
                return Ok(response);
            }
        }

        public ResponseViewModel ConsultarCE(ClientBindingModel client)
        {
            ResponseViewModel response = new ResponseViewModel();
            ResponseCEModel _responseCEModel;
            ServiceCore ServiceCore = new ServiceCore();

            var itemCliente = new ClientViewModel
            {
                EListPhoneClient = new List<PhoneViewModel>(),
                EListEmailClient = new List<EmailViewModel>(),
                EListContactClient = new List<ContactViewModel>(),
                EListCIIUClient = new List<CiiuViewModel>(),
                EListAddresClient = new List<AddressViewModel>(),
                P_DBIRTHDAT = client.P_DBIRTHDAT,
                P_NIDDOC_TYPE = client.P_NIDDOC_TYPE,
                P_SIDDOC = client.P_SIDDOC,
                P_SREGIST = "1",
                P_ORIGEN_DATA = "CE"
            };
            response.P_NCODE = "3";
            try
            {
                var FechaNac = string.IsNullOrEmpty(client.P_DBIRTHDAT) ? DateTime.Now : Convert.ToDateTime(client.P_DBIRTHDAT);
                RequestCEModel _objCE = new RequestCEModel()
                {
                    CE = client.P_SIDDOC,
                    ANIO = FechaNac.Year.ToString(),
                    MES = FechaNac.Month.ToString(),
                    DIA = FechaNac.Day.ToString()
                };
                var request = JsonConvert.SerializeObject(_objCE);

                string _responseJSON = ServiceCore.ConsumeServiceComum(Service.UrlCE, Method.POST, new System.Net.WebHeaderCollection(), request, null, false);
                if (!string.IsNullOrEmpty(_responseJSON))
                {
                    _responseCEModel = JsonConvert.DeserializeObject<ResponseCEModel>(_responseJSON, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (_responseCEModel.estado == 0)
                    {
                        response.P_NCODE = "0";
                        itemCliente.P_DBIRTHDAT = Convert.ToDateTime(_responseCEModel.nacimiento).ToString("dd/MM/yyyy");
                        string[] nombres = _responseCEModel.nombre.Replace(",", "").Split(' ').ToArray();
                        itemCliente.P_SLASTNAME = (nombres.ElementAtOrDefault(0) != null) ? nombres[0] : string.Empty;
                        itemCliente.P_SLASTNAME2 = (nombres.ElementAtOrDefault(1) != null) ? nombres[1] : string.Empty;
                        itemCliente.P_SFIRSTNAME = (nombres.ElementAtOrDefault(2) != null) ? nombres[2] : string.Empty;
                        itemCliente.P_SFIRSTNAME += (nombres.ElementAtOrDefault(3) != null) ? " " + nombres[3] : string.Empty;
                        itemCliente.P_NNATIONALITY = new ConsultaCore().ObtenerCodigoPais(_responseCEModel.nacionalidad).ToString();

                    }
                }

                response.EListClient = new List<ClientViewModel> { itemCliente };
            }
            catch (Exception ex)
            {
                response.P_NCODE = "3";
                response.P_SMESSAGE = ex.Message;
                response.EListClient = new List<ClientViewModel> { itemCliente };
            }

            return response;
        }

        public ResponseViewModel ConsultarSUNAT(string str_Documento)
        {

            ResponseSunatModel _ResponseSUNAT;
            ServiceCore ServiceCore = new ServiceCore();
            ResponseViewModel response = new ResponseViewModel();
            var itemCliente = new ClientViewModel
            {
                EListPhoneClient = new List<PhoneViewModel>(),
                EListEmailClient = new List<EmailViewModel>(),
                EListContactClient = new List<ContactViewModel>(),
                EListCIIUClient = new List<CiiuViewModel>(),
                EListAddresClient = new List<AddressViewModel>(),
                P_NIDDOC_TYPE = "1",
                P_SIDDOC = str_Documento
            };
            try
            {
                string _responseJSON = ServiceCore.ConsultarCliente(str_Documento, "UrlServiceSUNAT");
                _ResponseSUNAT = JsonConvert.DeserializeObject<ResponseSunatModel>(_responseJSON, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                if (_ResponseSUNAT.Exito == true && _ResponseSUNAT.Data != null)
                {

                    var _ObjClient = _ResponseSUNAT.Data;


                    string[] array = new string[] { "10", "15", "17" };
                    if (array.Contains(str_Documento.Substring(0, 2)))
                    {

                        string[] Nombre = _ObjClient.RazonSocial.Split(' ');
                        string Firstname = "";
                        if (Nombre.Length > 2)
                        {
                            Firstname = Nombre[2];
                        }
                        if (Nombre.Length > 3)
                        {
                            Firstname = Firstname + " " + Nombre[3];
                        }
                        itemCliente.P_SFIRSTNAME = Firstname;
                        if (Nombre[0] != null)
                        {
                            itemCliente.P_SLASTNAME = Nombre[0];
                        }
                        if (Nombre[1] != null)
                        {
                            itemCliente.P_SLASTNAME2 = Nombre[1];
                        }


                    }

                    itemCliente.P_SLEGALNAME = _ObjClient.RazonSocial;

                    // itemCliente.P_SFIRSTNAME = (_ObjClient.Direcion == "-" || _ObjClient.Direcion == null  ? _ObjClient.RazonSocial : null);
                    //Address

                    var _ObjDirecClient = new AddressViewModel();
                    if (_ObjClient.Direcion != "-" && _ObjClient.Direcion != null)
                    {

                        _ObjDirecClient.P_SNOM_DIRECCION = _ObjClient.Direcion;
                        _ObjDirecClient.P_SDESDIREBUSQ = _ObjClient.Direcion;
                        _ObjDirecClient.P_DESDISTRITO = RemoveAccents(_ObjClient?.Distrito);//
                        _ObjDirecClient.P_DESDEPARTAMENTO = RemoveAccents(_ObjClient?.Departamento);//
                        _ObjDirecClient.P_DESPROVINCIA = RemoveAccents(_ObjClient?.Provincia);//
                        itemCliente.EListAddresClient.Add(_ObjDirecClient);
                    }

                    itemCliente.P_ORIGEN_DATA = "RENIEC";
                    itemCliente.P_SISRENIEC_IND = "1";
                    response.EListClient = new List<ClientViewModel> { itemCliente };
                    response.P_NCODE = "0";
                }
                else
                {
                    response.EListClient = new List<ClientViewModel> { itemCliente };
                    response.P_SMESSAGE = _ResponseSUNAT.Mensaje;
                    response.P_NCODE = "3";
                }
            }
            catch (Exception ex)
            {
                response.EListClient = new List<ClientViewModel> { itemCliente };
                response.P_NCODE = "3";
                response.P_SMESSAGE = ex.Message;
            }
            return response;
        }



        public ResponseReniecViewModel ObtenerClientReniecLocal(ClientBindingModel client, out bool ExistLocal)
        {
            try
            {
                ExistLocal = false;
                ResponseReniecViewModel ResponseClient = new ResponseReniecViewModel();
                InsertaCore InsertaCore = new InsertaCore();
                ResponseViewModel response = InsertaCore.ObtenerClientReniecLocal(client);
                List<ResponseReniecViewModel> ListReniec = (List<ResponseReniecViewModel>)response.Data;
                if (ListReniec != null)
                {
                    ExistLocal = (ListReniec.Count > 0) ? true : false;

                    ResponseClient = ListReniec[0];

                    //Campos muy grandes para el store 
                    ResponseClient.CODIGOUBIGEODEPARTAMENTODOMICILIO = ResponseClient.CODUBIGEODEPARTAMENTODOMICILIO;
                    ResponseClient.CODIGOUBIGEODEPARTAMENTONACIMIENTO = ResponseClient.CODUBIGEODEPARTAMENTONACI;
                    ResponseClient.CODIGOUBIGEOPROVINCIANACIMIENTO = ResponseClient.CODUBIGEOPROVINCIANACI;
                }
                return ResponseClient;
            }
            catch (Exception ex)
            {
                ExistLocal = false;
                return null;
            }
        }
        public void AddClientReniec(ResponseReniecViewModel responseR)
        {
            if (responseR != null)
            {
                InsertaCore Core = new InsertaCore();
                ResponseViewModel Response = Core.InsertarClienteReniec(responseR);
            }
        }
        public string HomologarCampo(string valor, string CodAplication, string Tipo)
        {
            if (valor != "")
            {
                InsertaCore Core = new InsertaCore();
                return Core.HomologarCampos(CodAplication, Tipo, valor);
            }
            else { return ""; }
        }
        public List<AddressViewModel> HomologarUbigeo(List<AddressViewModel> addressBindingModels, string CodAplication)
        {
            InsertaCore Core = new InsertaCore();
            foreach (AddressViewModel element in addressBindingModels)
            {
                string Municipality = Core.HomologarCampos(CodAplication, "RUBIGEO", element.P_NMUNICIPALITY.ToString());
                element.P_NMUNICIPALITY = Municipality;
                element.P_NLOCAL = Municipality.Substring(0, 2);
                element.P_NPROVINCE = Municipality.Substring(0, 4);
            }
            return addressBindingModels;
        }
        public List<AddressBindingModel> SetRecowner(List<AddressBindingModel> addressBindingModels)
        {
            foreach (AddressBindingModel element in addressBindingModels)
            {
                if (element.P_NRECOWNER != "3" && element.P_NRECOWNER != "4")
                {
                    element.P_NRECOWNER = "2";
                }
            }
            return addressBindingModels;
        }
        private ResponseViewModel ValidarCamposCliente(ClientBindingModel request)
        {
            InsertaCore InsertaCore = new InsertaCore();
            ResponseViewModel response = new ResponseViewModel();
            if (request.P_CodAplicacion.ToUpper() == "GESTORCLIENTE")
            {
                if (request.P_NIDDOC_TYPE == "1")
                {
                    if (request.EListCIIUClient == null || request.EListCIIUClient.Count() == 0)
                    {
                        return new ResponseViewModel
                        {
                            P_NCODE = "1",
                            P_SMESSAGE = "Para continuar el proceso debe de registrar un CIIU."
                        };
                    }
                    else
                    {
                        var countDEL = request.EListCIIUClient.Where(x => x.P_TipOper == "DEL").Count();
                        if (request.EListCIIUClient.Count() == countDEL)
                        {
                            return new ResponseViewModel
                            {
                                P_NCODE = "1",
                                P_SMESSAGE = "Para continuar el proceso debe de tener un CIIU activo."
                            };
                        }
                    }
                }
            }
            response = (request.P_TIPO_VAL != "MASIVO") ? InsertaCore.ValidarCliente(request) : InsertaCore.ValidarClienteMasivo(request);
            return response;
        }
        private ResponseViewModel EjecutarStore(ClientBindingModel request)
        {
            InsertaCore InsertaCore = new InsertaCore();
            ResponseViewModel response = new ResponseViewModel();
            string TipoDoc = InsertaCore.HomologarCampos(request.P_CodAplicacion, "DOCIDE", request.P_NIDDOC_TYPE.ToString());
            if (TipoDoc == "1" && request.P_CodAplicacion == "GESTORCLIENTE" && request.P_SREGISTCIIU != "1")
            {
                if (request.EListCIIUClient == null || request.EListCIIUClient.Count() == 0)
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "Para continuar el proceso debe de registrar un CIIU.";
                }
                else
                {
                    var countDEL = request.EListCIIUClient.Where(x => x.P_TipOper == "DEL").Count();
                    if (request.EListCIIUClient.Count() == countDEL)
                    {
                        response.P_NCODE = "1";
                        response.P_SMESSAGE = "Para continuar el proceso debe de tener un CIIU activo.";
                    }
                    else
                    {
                        try
                        {
                            response = InsertaCore.InsertarCliente(request);
                            if (request.P_SSISTEMA != null)
                            {
                                if (request.P_SSISTEMA != "")
                                {
                                    ResponseSistemaViewModel responseSistema = new ResponseSistemaViewModel();
                                    responseSistema = InsertaCore.InsertarSistema(request.P_SSISTEMA, request.P_NIDDOC_TYPE, request.P_SIDDOC);
                                    response.P_SCOD_CLIENT = responseSistema.P_SCOD_CLIENT;
                                    response.P_SURL_SISTEMA = responseSistema.P_SURL_SISTEMA;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            response.P_NCODE = "1";
                            response.P_SMESSAGE = ex.Message;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    response = InsertaCore.InsertarCliente(request);

                    if (request.P_SSISTEMA != "" && request.P_SSISTEMA != null)
                    {
                        request.P_NIDDOC_TYPE = TipoDoc;
                        ResponseSistemaViewModel responseSistema = new ResponseSistemaViewModel();
                        responseSistema = InsertaCore.InsertarSistema(request.P_SSISTEMA, request.P_NIDDOC_TYPE, request.P_SIDDOC);

                        response.P_SCOD_CLIENT = responseSistema.P_SCOD_CLIENT;
                        response.P_SURL_SISTEMA = responseSistema.P_SURL_SISTEMA;
                    }
                }
                catch (Exception ex)
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = ex.Message;
                }
            }
            return response;
        }

        [Route("ConsultarSegmentoporDocumento")]
        [HttpPost]
        public IHttpActionResult ConsultarSegementoporDocumento(SegmentoBindingModel request)
        {
            ConsultaCore ConsultaCore = new ConsultaCore();

            ResponseSegmentoViewModel response = new ResponseSegmentoViewModel();
            try
            {

                if (request != null)
                {
                    response = ConsultaCore.ConsultarSegementoporDocumento(request);
                    return Ok(response);
                }
                else
                {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = "El request que se ha enviado no tiene el formato correcto.";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.P_NCODE = "-1";
                response.P_SMESSAGE = ex.Message;
                return Ok(response);
            }
        }
        private string RemoveAccents(string text)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                {
                    sbReturn.Append(letter);
                }
            }
            return sbReturn.ToString();
        }

    }

}