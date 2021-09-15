using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSGCliente.DataAccess;
using WSGCliente.Entities.ViewModel;
using WSGCliente.Entities.BindingModel;

namespace WSGCliente.Core
{
    public class InsertaCore
    {
        InsertaDataAccess InsertaDataAccess = new InsertaDataAccess();
        public ResponseViewModel InsertarCliente(ClientBindingModel request)
        {
            return InsertaDataAccess.InsertarClienteGeneral(request);
        }
        public ResponseViewModel ValidarCliente(ClientBindingModel request)
        {
            return InsertaDataAccess.ValidarClienteGeneral(request);
        }
        public ResponseViewModel ValidarClienteMasivo(ClientBindingModel request)
        {
            return InsertaDataAccess.ValidarClienteGeneralMasivo(request);
        }

        public DireccionCompletaViewModel DireccionCompleta(DireccionCompletaBindingModel request)
        {
            return InsertaDataAccess.DireccionCompleta(request);
        }

        public ResponseSistemaViewModel InsertarSistema(string P_SSISTEMA, string P_NIDDOC_TYPE, string P_SIDDOC)
        {
            return InsertaDataAccess.InsertarSistema(P_SSISTEMA, P_NIDDOC_TYPE, P_SIDDOC);
        }
        public string HomologarCampos(string P_Origen, string Campo, string expresion)
        {
            return InsertaDataAccess.HomologarCampos(P_Origen, Campo, expresion);
        }
        public ResponseViewModel InsertarErrores(ErrorValBindingModel request)
        {
            return InsertaDataAccess.InsertarErrores(request);
        }
        public ResponseViewModel InsertarReniec(ReniecBindingModel request)
        {
            return InsertaDataAccess.InsertarReniec(request);
        }
        public ResponseViewModel InsertarExitosos(ExitoValBindingModel request)
        {
            return InsertaDataAccess.InsertarExitosos(request);
        }
        public List<ExitoViewModel> ListarExitoso(ReportBindingModel request)
        {
            return InsertaDataAccess.ListarExitoso(request);
        }
        public List<ErrorViewModel> ListarError(ReportBindingModel request)
        {
            return InsertaDataAccess.ListarError(request);
        }
        public List<ReniecViewModel> ListarReniec(ReportBindingModel request)
        {
            return InsertaDataAccess.ListarReniec(request);
        }
 public ResponseViewModel ObtenerClientReniecLocal(ClientBindingModel Client) {
            return InsertaDataAccess.ObtenerClientReniecLocal(Client);
        }
        public ResponseViewModel InsertarClienteReniec(ResponseReniecViewModel Reniec)
        {
            return InsertaDataAccess.InsertarClienteReniec(Reniec);
        }
 public ResponseViewModel DestinatarioEmail(string nUSERCODE)
        {
            return InsertaDataAccess.DestinatarioEmail(nUSERCODE);
        }    
    }
}
