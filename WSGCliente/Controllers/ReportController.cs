using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Reporting.WinForms;
using WSGCliente.Entities.BindingModel.Report;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace WSGCliente.Controllers
{
    [RoutePrefix("Api/Report")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReportController : ApiController
    {
        [Route("CreateSolicitud")]
        [HttpPost]
        public IHttpActionResult CreateSolicitud(Ticket request)
        {
       
            GenerateResponse reponse = new GenerateResponse();
            try
            { 
                string base64String = GenerarSolicitud(request);
                reponse.data = base64String;
                reponse.P_NCODE = 0;
            }
            catch (Exception ex)
            {
                reponse.P_NCODE = 1;
                reponse.P_SMESSAGE = ex.Message;
            }
            return Ok(reponse);
        }
        [Route("CreateReclamo")]
        [HttpPost]
        public IHttpActionResult CreateReclamo(Ticket request)
        {

            GenerateResponse reponse = new GenerateResponse();
            try
            {
                string base64String = GenerarSolicitud(request);
                reponse.data = base64String;
                reponse.P_NCODE = 0;
            }
            catch (Exception ex)
            {
                reponse.P_NCODE = 1;
                reponse.P_SMESSAGE = ex.Message;
            }
            return Ok(reponse);
        }

        [Route("GenerateReport")]
        [HttpPost]
        public IHttpActionResult GenerateReportCupon(List<TemplateCupon1> request)
        {
            List<byte[]> listByte = new List<byte[]>();
            GenerateResponse reponse = new GenerateResponse();
            try { 
            foreach(TemplateCupon1 item in request)
            {
                byte[] PdfBype = null;
                PdfBype = GeneratePDFTemplateCupon1(item);
                if(PdfBype != null)
                {
                    listByte.Add(PdfBype);
                }
            }
            byte[] PdfFinale = MergeFiles(listByte);
            string base64String = Convert.ToBase64String(PdfFinale, 0, PdfFinale.Length);
            reponse.data = base64String;
            reponse.P_NCODE = 0;
            }catch(Exception ex)
            {
                reponse.P_NCODE = 1;
                reponse.P_SMESSAGE = ex.Message;
            }
            return Ok(reponse);
        }

        [Route("GenerateReportCrono")]
        [HttpPost]
        public IHttpActionResult GenerateReportCrono(List<TemplateCupon2> request)
        {
            GenerateResponse reponse = new GenerateResponse();
            try
            {
                byte[] PdfBype = null;
                PdfBype = GeneratePDFTemplateCuponCrono(request);        
                string base64String = Convert.ToBase64String(PdfBype, 0, PdfBype.Length);
                reponse.data = base64String;
                reponse.P_NCODE = 0;
            }
            catch (Exception ex)
            {
                reponse.P_NCODE = 1;
                reponse.P_SMESSAGE = ex.Message;
            }
            return Ok(reponse);
        }


        private DataTable CuponPagoDatatable(TemplateCupon1 Cupon)
        {
            DataTable dt = new DataTable();
            dt.TableName = "CuponPago";
            dt.Columns.Add("CONVENIO");
            dt.Columns.Add("PAGO_NUMERO");
            dt.Columns.Add("VENCIMIENTO_PAGO");
            dt.Columns.Add("IMPORTE");
            DataRow row = dt.NewRow();
            row["CONVENIO"] = Cupon.Cuponera;
            row["PAGO_NUMERO"] = Cupon.Cupon;
            row["VENCIMIENTO_PAGO"] = Cupon.FechaVencimiento;
            row["IMPORTE"] = Cupon.Importe.ToString();
            dt.Rows.Add(row);
            return dt;
        }
        private DataTable CuponPagoDatatableCrono(List<TemplateCupon2> ListCronograma)
        {
            DataTable dt = new DataTable();
            dt.TableName = "CuponPago";
            dt.Columns.Add("NROCUPON");
            dt.Columns.Add("NRORECIBO");
            dt.Columns.Add("VENCIMIENTO");
            dt.Columns.Add("INTERES");
            dt.Columns.Add("IMPORTE");
            foreach (TemplateCupon2 item in ListCronograma)
            {
                DataRow row = dt.NewRow();
                row["NROCUPON"] = item.NroCupon;
                row["NRORECIBO"] = item.NroRecibo;
                row["VENCIMIENTO"] = item.Vencimiento;
                row["INTERES"] = item.Interes;
                row["IMPORTE"] = item.Importe;
                dt.Rows.Add(row);
            }

            return dt;
        }

        public byte[] GeneratePDFTemplateCuponCrono(List<TemplateCupon2> cronograma)
        {
            string base64String = string.Empty;
            byte[] renderedBytes;
            try
            {
                ReportDataSource dsOBJ = new ReportDataSource();
                dsOBJ.Name = "TblCronograma";
                dsOBJ.Value = CuponPagoDatatableCrono(cronograma);

                IEnumerable<ReportDataSource> datasets = new List<ReportDataSource> { dsOBJ };
                LocalReport localReport = new LocalReport();
                string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
                //localReport.ReportPath = startupPath + @"Reports\\ReportError.rdlc";
                localReport.ReportPath = HttpContext.Current.Server.MapPath("~/Reports/cuponera/CronogramaPago.rdlc");
                var itemCrono = cronograma[0];
                ReportParameter[] reportParameter = new ReportParameter[11];
                reportParameter[0] = new ReportParameter("NROPAGO", itemCrono.NroPago);
                reportParameter[1] = new ReportParameter("RAMO", itemCrono.DesRamo);
                reportParameter[2] = new ReportParameter("POLIZA", itemCrono.Poliza);
                reportParameter[3] = new ReportParameter("VIGENCIADESDE", itemCrono.VigenciaDesde);
                reportParameter[4] = new ReportParameter("VIGENCIAHASTA", itemCrono.VigenciaHasta);
                reportParameter[5] = new ReportParameter("MONEDA", itemCrono.Moneda);
                reportParameter[6] = new ReportParameter("MODALIDADPAGO", itemCrono.ModalidadPago);
                reportParameter[7] = new ReportParameter("FECHA", itemCrono.Fecha);
                reportParameter[8] = new ReportParameter("NOMBRES", itemCrono.Nombres);
                reportParameter[9] = new ReportParameter("NRODOCUMENTO", itemCrono.NroDocumento);
                reportParameter[10] = new ReportParameter("DIRECION", itemCrono.Direccion);

                localReport.SetParameters(reportParameter);
                localReport.Refresh();
                foreach (ReportDataSource datasource in datasets)
                {
                    localReport.DataSources.Add(datasource);
                }

                //Renderizado
                string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat></DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                string mimeType;
                string encoding;
                string fileNameExtension;
                localReport.Refresh();
                renderedBytes = localReport.Render("PDF", deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            }
            catch (Exception ex)
            {
                renderedBytes = null;
            }
            return renderedBytes;
        }

        public string GenerarSolicitud(Ticket ticket)
        {

            string filename = "Solicitud" + "-" + ticket.Codigo + ".pdf";
            string pat = "C:/PDFs/" + filename;
            string respuesta = pat; 
            string direccion = ticket.Direccion;
            string telefono = ".";
            string Cdoc = "-";
            LocalReport Reporte = new LocalReport(); 
            Reporte.ReportPath = HttpContext.Current.Server.MapPath("~/Reports/SOL.rdlc");
            ReportParameterCollection parametros = new ReportParameterCollection();
            parametros.Add(new ReportParameter("FechaRecepcion", ticket.FecRecepcion.ToString().Substring(0, 10)));
            parametros.Add(new ReportParameter("ViaRecepcion", ticket.ViaRecepcion.ToString()));
            parametros.Add(new ReportParameter("Numero", ticket.Codigo.ToString()));
            if (ticket.TipoDocumento == "RUC")
            {
                if (ticket.Documento.Substring(0, 2) == "10" || ticket.Documento.Substring(0, 2) == "15" || ticket.Documento.Substring(0, 2) == "17")
                {
                    parametros.Add(new ReportParameter("NombreContacto", ticket.Contacto.ToString()));
                    parametros.Add(new ReportParameter("RazonSocialContacto", Cdoc));
                }
                else
                {
                    parametros.Add(new ReportParameter("NombreContacto", Cdoc));
                    parametros.Add(new ReportParameter("RazonSocialContacto", ticket.Contacto.ToString()));
                }
            }
            else
            {
                parametros.Add(new ReportParameter("NombreContacto", ticket.Contacto.ToString()));
                parametros.Add(new ReportParameter("RazonSocialContacto", Cdoc));
            }
            Cdoc = ticket.TipoDocumento + " " + ticket.Documento.ToString();
            parametros.Add(new ReportParameter("DocumentoContacto", Cdoc));
            Cdoc = "-";
            parametros.Add(new ReportParameter("VinculoContacto", ticket.Vinculo));
            parametros.Add(new ReportParameter("DireccionContacto", ticket.Direccion));
            parametros.Add(new ReportParameter("ReferenciaContacto", ticket.referencia));
            parametros.Add(new ReportParameter("CorreoContacto", ticket.Email));
            parametros.Add(new ReportParameter("TelefonoContacto", ticket.Telefono));
            parametros.Add(new ReportParameter("CelularContacto", "-"));
            if (ticket.TipoDocumento == "RUC")
            {
                if (ticket.Documento.Substring(0, 2) == "10" || ticket.Documento.Substring(0, 2) == "15" || ticket.Documento.Substring(0, 2) == "17")
                {
                    parametros.Add(new ReportParameter("NombreCliente", ticket.Nombre.ToString()));
                    parametros.Add(new ReportParameter("RazonSocialCliente", Cdoc));
                }
                else
                {
                    parametros.Add(new ReportParameter("NombreCliente", Cdoc));
                    parametros.Add(new ReportParameter("RazonSocialCliente", ticket.Nombre.ToString()));
                }
            }
            else
            {
                parametros.Add(new ReportParameter("NombreCliente", ticket.Nombre.ToString()));
                parametros.Add(new ReportParameter("RazonSocialCliente", Cdoc));
            }
            Cdoc = ticket.TipoDocumento + " " + ticket.Documento.ToString();
            parametros.Add(new ReportParameter("DocumentoCliente", Cdoc));
            parametros.Add(new ReportParameter("DireccionCliente", ticket.direccioncli));
            parametros.Add(new ReportParameter("CorreoCliente", ticket.EmailCli));
            parametros.Add(new ReportParameter("TelefonoCliente", telefono));
            parametros.Add(new ReportParameter("CelularCliente", "-"));
            parametros.Add(new ReportParameter("Producto", ticket.Producto));
            parametros.Add(new ReportParameter("Poliza", ticket.Poliza));
            parametros.Add(new ReportParameter("Motivo", ticket.SubMotivo));
            parametros.Add(new ReportParameter("Descripcion", ticket.Descripcion));
            parametros.Add(new ReportParameter("Ejecutivo", ticket.Ejecutivo));
            string adjunto1 = " ";
            string adjunto2 = " ";
            string adjunto3 = " ";
            string adjunto4 = " ";
            string adjunto5 = " ";
            int control = 0;
            if (ticket.Adjuntos != null)
            {
                foreach (Archivo archivo in ticket.Adjuntos)
                {
                    control++;
                    if (control == 1) { adjunto1 = archivo.name; }
                    if (control == 2) { adjunto2 = archivo.name; }
                    if (control == 3) { adjunto3 = archivo.name; }
                    if (control == 4) { adjunto4 = archivo.name; }
                    if (control == 5) { adjunto5 = archivo.name; }
                }
            }

            parametros.Add(new ReportParameter("Adjunto1", adjunto1));
            parametros.Add(new ReportParameter("Adjunto2", adjunto2));
            parametros.Add(new ReportParameter("Adjunto3", adjunto3));
            parametros.Add(new ReportParameter("Adjunto4", adjunto4));
            parametros.Add(new ReportParameter("Adjunto5", adjunto5));

            try
            {
                Reporte.SetParameters(parametros);
                Reporte.Refresh();
                var pgs = Reporte.GetDefaultPageSettings();

                string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat><MarginRight>0in</MarginRight><MarginTop>0in</MarginTop><MarginBottom>0in</MarginBottom>><MarginLeft>0in</MarginLeft></DeviceInfo>";
                Warning[] warnings;
                byte[] renderedBytes;
                string[] streams;
                string mimeType;
                string encoding;
                string fileNameExtension;
                renderedBytes = Reporte.Render("PDF", deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                respuesta = Convert.ToBase64String(renderedBytes, 0, renderedBytes.Length); 

            }
            catch (Exception ex)
            {
                var inn = ex.InnerException;
                string mess = "";
                if (inn != null)
                {
                    mess = ex.InnerException.ToString();
                } 
                respuesta = "0";
            }
            return respuesta;
        }

        public string GeneraReclamacion(Ticket ticket)
        {
            string respuesta = "";
            try
            { 
                string filename = "Reclamo" + "-" + ticket.Codigo + ".pdf"; 
                string pat = "C:/PDFs/" + filename;
                respuesta = pat;  
                string direccion = ticket.Direccion;
                string Cdoc = "-"; 
                LocalReport Reporte = new LocalReport();
                Reporte.ReportPath = HttpContext.Current.Server.MapPath("~/Reports/REC2.rdlc"); 
                ReportParameterCollection parametros = new ReportParameterCollection();
                parametros.Add(new ReportParameter("FechaRecepcion", ticket.FecRecepcion.ToString().Substring(0, 10)));
                parametros.Add(new ReportParameter("ViaRecepcion", ticket.ViaRecepcion.ToString()));
                parametros.Add(new ReportParameter("Numero", ticket.Codigo.ToString()));
                 
                if (ticket.TipoDocumento == "RUC")
                {
                    if (ticket.Documento.Substring(0, 2) == "10" || ticket.Documento.Substring(0, 2) == "15" || ticket.Documento.Substring(0, 2) == "17")
                    {
                        parametros.Add(new ReportParameter("NombreContacto", ticket.Contacto.ToString()));
                        parametros.Add(new ReportParameter("RazonSocialContacto", Cdoc));
                    }
                    else
                    {
                        parametros.Add(new ReportParameter("NombreContacto", Cdoc));
                        parametros.Add(new ReportParameter("RazonSocialContacto", ticket.Contacto.ToString()));
                    }
                }
                else
                {
                    parametros.Add(new ReportParameter("NombreContacto", ticket.Contacto.ToString()));
                    parametros.Add(new ReportParameter("RazonSocialContacto", Cdoc));
                }
                Cdoc = ticket.TipoDocumento + " " + ticket.Documento.ToString();
                parametros.Add(new ReportParameter("DocumentoContacto", Cdoc));
                Cdoc = "-";
                parametros.Add(new ReportParameter("VinculoContacto", ticket.Vinculo));
                parametros.Add(new ReportParameter("DireccionContacto", ticket.Direccion));
                parametros.Add(new ReportParameter("ReferenciaContacto", ticket.referencia));
                parametros.Add(new ReportParameter("CorreoContacto", ticket.Email));
                parametros.Add(new ReportParameter("TelefonoContacto", ticket.Telefono));
                parametros.Add(new ReportParameter("CelularContacto", "-"));
                parametros.Add(new ReportParameter("ViaRespuestaContacto", ticket.ViaRespuesta.ToString()));
                 
                if (ticket.TipoDocumentoCli == "RUC")
                {
                    if (ticket.DocumentoCli.Substring(0, 2) == "10" || ticket.DocumentoCli.Substring(0, 2) == "15" || ticket.DocumentoCli.Substring(0, 2) == "17")
                    {
                        parametros.Add(new ReportParameter("NombreAsegurado", ticket.Nombre));
                        parametros.Add(new ReportParameter("RazonSocialAsegurado", Cdoc));
                    }
                    else
                    {
                        parametros.Add(new ReportParameter("NombreAsegurado", Cdoc));
                        parametros.Add(new ReportParameter("RazonSocialAsegurado", ticket.Nombre));
                    }
                }
                else
                {
                    parametros.Add(new ReportParameter("NombreAsegurado", ticket.Nombre));
                    parametros.Add(new ReportParameter("RazonSocialAsegurado", Cdoc));
                }
                Cdoc = ticket.TipoDocumentoCli + " " + ticket.DocumentoCli.ToString();
                parametros.Add(new ReportParameter("DocumentoAsegurado", Cdoc));
                parametros.Add(new ReportParameter("DireccionAsegurado", ticket.direccioncli));
                parametros.Add(new ReportParameter("CorreoAsegurado", ticket.EmailCli));
                parametros.Add(new ReportParameter("TelefonoAsegurado", ""));
                parametros.Add(new ReportParameter("Servicio", ticket.Producto));
                parametros.Add(new ReportParameter("Monto", ticket.Monto));
                parametros.Add(new ReportParameter("Motivo", ticket.SubMotivo));
                parametros.Add(new ReportParameter("Descripcion", ticket.Descripcion));
                parametros.Add(new ReportParameter("Ejecutivo", ticket.Ejecutivo));
                string adjunto1 = " ";
                string adjunto2 = " ";
                string adjunto3 = " ";
                string adjunto4 = " ";
                string adjunto5 = " ";
                int control = 0;
                if (ticket.Adjuntos != null)
                {
                    foreach (Archivo archivo in ticket.Adjuntos)
                    {
                        control++;
                        if (control == 1) { adjunto1 = archivo.name; }
                        if (control == 2) { adjunto2 = archivo.name; }
                        if (control == 3) { adjunto3 = archivo.name; }
                        if (control == 4) { adjunto4 = archivo.name; }
                        if (control == 5) { adjunto5 = archivo.name; }
                    }
                } 
                parametros.Add(new ReportParameter("Adjunto1", adjunto1));
                parametros.Add(new ReportParameter("Adjunto2", adjunto2));
                parametros.Add(new ReportParameter("Adjunto3", adjunto3));
                parametros.Add(new ReportParameter("Adjunto4", adjunto4));
                parametros.Add(new ReportParameter("Adjunto5", adjunto5));

                try
                {
                    Reporte.SetParameters(parametros);
                    Reporte.Refresh(); 
                    var pgs = Reporte.GetDefaultPageSettings();

                    string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat><MarginRight>0in</MarginRight><MarginTop>0in</MarginTop><MarginBottom>0in</MarginBottom>><MarginLeft>0in</MarginLeft></DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    string mimeType;
                    byte[] renderedBytes;
                    string encoding;
                    string fileNameExtension; 
                    renderedBytes = Reporte.Render("PDF", deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                    respuesta = Convert.ToBase64String(renderedBytes, 0, renderedBytes.Length); 

                }
                catch (Exception ex)
                { 
                    var inn = ex.InnerException;
                    string mess = "";
                    if (inn != null)
                    {
                        mess = ex.InnerException.ToString();
                    } 
                    respuesta = "0";
                }
            }
            catch (Exception ex)
            { 
                var inn = ex.InnerException;
                string mess = "";
                if (inn != null)
                {
                    mess = ex.InnerException.ToString();
                } 
                respuesta = "0";
            }
            return respuesta;
        }

        public byte[] GeneratePDFTemplateCupon1(TemplateCupon1 cupon1)
        {
            string base64String = string.Empty;
            byte[] renderedBytes;
            try
            {
                ReportDataSource dsOBJ = new ReportDataSource();
                dsOBJ.Name = "TblCuponPago";
                dsOBJ.Value = CuponPagoDatatable(cupon1);

                IEnumerable<ReportDataSource> datasets = new List<ReportDataSource> { dsOBJ };
                LocalReport localReport = new LocalReport();
                string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
                //localReport.ReportPath = startupPath + @"Reports\\ReportError.rdlc";
                localReport.ReportPath = HttpContext.Current.Server.MapPath("~/Reports/cuponera/CuponPago.rdlc");

                ReportParameter[] reportParameter = new ReportParameter[10];
                reportParameter[0] = new ReportParameter("RAMO", cupon1.DesRamo);
                reportParameter[1] = new ReportParameter("POLIZA", cupon1.Policy);
                reportParameter[2] = new ReportParameter("MONEDA", cupon1.DesMoneda);
                reportParameter[3] = new ReportParameter("VIGENCIA_DESDE", cupon1.VigenciaDesde);
                reportParameter[4] = new ReportParameter("VIGENCIA_HASTA", cupon1.VigenciaHasta);
                reportParameter[5] = new ReportParameter("ASEGURADO", cupon1.Asegurado);
                reportParameter[6] = new ReportParameter("DOCUMENTO", cupon1.Documento);
                reportParameter[7] = new ReportParameter("DIRECCION", cupon1.Direccion);
                reportParameter[8] = new ReportParameter("INTERMEDIARIO", cupon1.Intermediario);
                reportParameter[9] = new ReportParameter("CUPON_CABECERA",cupon1.Cupon + "/" + cupon1.Cupones);

                localReport.SetParameters(reportParameter);
                localReport.Refresh();
                foreach (ReportDataSource datasource in datasets)
                {
                    localReport.DataSources.Add(datasource);
                }

                //Renderizado
                string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat></DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                string mimeType;
                string encoding;
                string fileNameExtension;
                localReport.Refresh();
                renderedBytes = localReport.Render("PDF", deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            }
            catch (Exception ex)
            {
                renderedBytes = null;
            }
            return renderedBytes;
        }

        public static byte[] MergeFiles(List<byte[]> sourceFiles)
        {
            Document document = new Document();
            using (MemoryStream ms = new MemoryStream())
            {
                PdfCopy copy = new PdfCopy(document, ms);
                document.Open();
                int documentPageCounter = 0;

                // Iterate through all pdf documents
                for (int fileCounter = 0; fileCounter < sourceFiles.Count; fileCounter++)
                {
                    // Create pdf reader
                    PdfReader reader = new PdfReader(sourceFiles[fileCounter]);
                    int numberOfPages = reader.NumberOfPages;

                    // Iterate through all pages
                    for (int currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
                    {
                        documentPageCounter++;
                        PdfImportedPage importedPage = copy.GetImportedPage(reader, currentPageIndex);
                        PdfCopy.PageStamp pageStamp = copy.CreatePageStamp(importedPage);

                        // Write header
                        ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                            new Phrase(""), importedPage.Width / 2, importedPage.Height - 30,
                            importedPage.Width < importedPage.Height ? 0 : 1);

                        // Write footer
                        ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                            new Phrase(String.Format("Pagina {0}", documentPageCounter)), importedPage.Width / 2, 30,
                            importedPage.Width < importedPage.Height ? 0 : 1);

                        pageStamp.AlterContents();

                        copy.AddPage(importedPage);
                    }

                    copy.FreeReader(reader);
                    reader.Close();
                }

                document.Close();
                return ms.GetBuffer();
            }
        }



    }
}