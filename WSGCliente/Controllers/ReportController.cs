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