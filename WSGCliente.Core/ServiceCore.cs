using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSGCliente.DataAccess;
using WSGCliente.Entities.BindingModel;
using WSGCliente.Entities.ViewModel;

namespace WSGCliente.Core
{
    public class ServiceCore
    {
        ServiceDataAccess ServiceDataAccess = new ServiceDataAccess();

        public string ConsultarCliente(string dni,string service)
        {
            try
            {
                //string json = JsonConvert.SerializeObject(dni);
                return ServiceDataAccess.ServiceReniec(dni, service);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public ResponseViewModelEmBlue SendSMS(EmBlueBindingModel request, string rutaEmblue)
        {
            return ServiceDataAccess.ConsultarEmBlue(request, rutaEmblue);
        }









        public string ConsumeServiceComum(Util.GlobalEnum.Service service,Util.GlobalEnum.Method method,System.Net.WebHeaderCollection webHeaderCollection,string Json, String[] Params,Boolean CertificateSSL = false)
        {
            try
            {
                return ServiceDataAccess.ConsumeServiceComum(service,method, webHeaderCollection,Json, Params,CertificateSSL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string ConsumeServiceComum(Util.GlobalEnum.Service service, Util.GlobalEnum.Method method, String[] Params, Boolean CertificateSSL = false)
        {
            try
            {
                return ServiceDataAccess.ConsumeServiceComum(service, method, new System.Net.WebHeaderCollection(), string.Empty, Params, CertificateSSL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }







    }
}
