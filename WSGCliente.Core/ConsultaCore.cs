using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSGCliente.DataAccess;
using WSGCliente.Entities.ViewModel;
using WSGCliente.Entities.BindingModel;
using WSGCliente.Entities.BindingModel.Intermediarios;

namespace WSGCliente.Core
{
    public class ConsultaCore
    {
        ConsultaDataAccess ConsultaDataAccess = new ConsultaDataAccess();
        public ResponseViewModel Consultar(ClientBindingModel request)
        {
            return ConsultaDataAccess.Consultar(request);
        }

        public ResponsePViewModel ConsultarProveedor(ClientBindingModel request)
        {
            return ConsultaDataAccess.ConsultarProveedor(request);
        }
        public List<ClientViewModel> ConsultarCliente(ClientBindingModel request)
        {
            return ConsultaDataAccess.ConsultarCliente(request);
        }
        public List<ClientPViewModel> ConsultarClienteProveedor(ClientBindingModel request)
        {
            return ConsultaDataAccess.ConsultarClienteProveedor(request);
        }
        public List<AddressViewModel> ConsultarClienteDireccion(string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteDireccion(P_SCLIENT);
        }

        public List<PhoneViewModel> ConsultarClienteTelefono(string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteTelefono(P_SCLIENT);
        }

        public List<EmailViewModel> ConsultarClienteEmail(string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteEmail(P_SCLIENT);
        }
        public List<ContactViewModel> ConsultarClienteContacto(string P_SCLIENT, string P_NPERSON_TYP)
        {
            return ConsultaDataAccess.ConsultarClienteContacto(P_SCLIENT, P_NPERSON_TYP);
        }
        public List<CiiuViewModel> ConsultarClienteCiiu(string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteCiiu(P_SCLIENT);
        }
        public List<HistoryViewModel> ConsultarClienteHistory(string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteHistory(P_SCLIENT);
        }
				//add 20220121
        public List<HistoryInformationViewModel> ConsultarClienteHistoryInformation(string P_NID, string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteHistoryInformation(P_NID,P_SCLIENT);
        }
        public List<HistoryPhoneViewModel> ConsultarClienteHistoryPhoneBefore(string P_NID, string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteHistoryPhoneBefore(P_NID,P_SCLIENT);
        }
        public List<HistoryPhoneViewModel> ConsultarClienteHistoryPhoneNow(string P_NID, string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteHistoryPhoneNow(P_NID,P_SCLIENT);
        }
        public List<HistoryEmailViewModel> ConsultarClienteHistoryEmailBefore(string P_NID, string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteHistoryEmailBefore(P_NID,P_SCLIENT);
        }
        public List<HistoryEmailViewModel> ConsultarClienteHistoryEmailNow(string P_NID, string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteHistoryEmailNow(P_NID,P_SCLIENT);
        }
        public List<HistoryAddressViewModel> ConsultarClienteHistoryAddressBefore(string P_NID, string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteHistoryAddressBefore(P_NID,P_SCLIENT);
        }
        public List<HistoryAddressViewModel> ConsultarClienteHistoryAddressNow(string P_NID, string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteHistoryAddressNow(P_NID,P_SCLIENT);
        }
        public List<HistoryContactViewModel> ConsultarClienteHistoryContactBefore(string P_NID, string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteHistoryContactBefore(P_NID,P_SCLIENT);
        }
        public List<HistoryContactViewModel> ConsultarClienteHistoryContactNow(string P_NID, string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarClienteHistoryContactNow(P_NID,P_SCLIENT);
        }
		//add 20220121
        //Implementacion Intermediario 2020
        public List<InfoBancariaBindingModel> ConsultarInfoBancaria(string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarInfoBancaria(P_SCLIENT);
        }
        public List<DocumentosBindingModel> ConsultarDocumentosAdjunto(string P_SCLIENT)
        {
            return ConsultaDataAccess.ConsultarDocumentosAdjunto(P_SCLIENT);
        }


        public List<ApplicationsBindingModel> ConsultarAplicacionesGC()
        {
            return ConsultaDataAccess.ConsultarAplicacionesGC();
        }

        public int ObtenerCodigoPais(string Nacionalidad)
        {
            return ConsultaDataAccess.ObtenerCodigoPais(Nacionalidad);
        }
        public ResponseSegmentoViewModel ConsultarSegementoporDocumento(SegmentoBindingModel request)
        {
            return ConsultaDataAccess.ConsultarSegementoporDocumento(request);
        }
    }
}
