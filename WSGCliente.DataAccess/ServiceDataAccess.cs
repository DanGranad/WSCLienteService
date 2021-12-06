using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WSGCliente.Entities.ViewModel;
using System.Net.Http;
using System.Net.Http.Headers;
using WSGCliente.Entities.BindingModel;
using Newtonsoft.Json;
using static WSGCliente.Util.GlobalEnum;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace WSGCliente.DataAccess
{
    public class ServiceDataAccess
    {
        public String ServiceReniec(string dni,string Service)
        {
            string response = string.Empty;
            string url = String.Format(ConfigurationManager.AppSettings[Service], dni);
            //byte[] data = UTF8Encoding.UTF8.GetBytes("");

            try
            {
                HttpWebRequest request;
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.Timeout = Convert.ToInt32(time) * 1000;
                request.Method = (Service == "UrlServiceSUNAT" ? "POST" : "GET") ;
                request.ContentLength = 0;
                request.ContentType = "application/json; charset=utf-8";
                request.Proxy = new WebProxy() { UseDefaultCredentials = false };

                HttpWebResponse respuesta = request.GetResponse() as HttpWebResponse;
                StreamReader read = new StreamReader(respuesta.GetResponseStream(), Encoding.UTF8);
                response = read.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }
        public ResponseViewModelEmBlue ConsultarEmBlue(EmBlueBindingModel request, string rutaEmblue)
        {
            ResponseViewModelEmBlue response = new ResponseViewModelEmBlue();
            var client = new HttpClient();
            client.BaseAddress = new Uri(rutaEmblue);
            client.DefaultRequestHeaders
                 .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Emails = request.Emails.Split(',');


            var result = client.PostAsync("json/SendSMS", new StringContent(
             JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")).Result;

            if (result.IsSuccessStatusCode)
            {
                var jsonString = result.Content.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(jsonString);

                response.AggregatesSendSMS = data.AggregatesSendSMS;
                response.GroupId = data.GroupId;
                response.Timestamp = data.Timestamp;
                response.TotalSendSMS = data.TotalSendSMS;

            }
            return response;
        }
    

        public String ConsumeServiceComum(Service service, Method method, WebHeaderCollection collection, string Json, String[] Params,Boolean CertificateSSL = false)
        {
            string response = string.Empty;
            string url = String.Format(ConfigurationManager.AppSettings[service.ToString()],(Params == null ? new String[] { } : Params));
            byte[] data = UTF8Encoding.UTF8.GetBytes(Json);
            try
            {
                HttpWebRequest request;
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = method.ToString();
                request.Headers = collection;
                request.ContentLength = data.Length;
                request.ContentType = "application/json; charset=utf-8";
                request.Proxy = new WebProxy() { UseDefaultCredentials = false };



                if (method.ToString() == "POST")
                {
                    Stream postTorrente = request.GetRequestStream();
                    postTorrente.Write(data, 0, data.Length);
                    postTorrente.Close();
                }

                //Certificate SSL
                if (CertificateSSL) { 
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                HttpWebResponse respuesta = request.GetResponse() as HttpWebResponse;
                StreamReader read = new StreamReader(respuesta.GetResponseStream(), Encoding.UTF8);
                response = read.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
           
        }

        public static bool ValidateServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
