using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSGCliente.Entities.BindingModel;
using WSGCliente.Entities.BindingModel.Intermediarios;
using WSGCliente.Entities.ViewModel;
using WSGCliente.Util;

namespace WSGCliente.DataAccess
{
    public class ConsultaDataAccess : ConnectionBase
    {
        public ResponseViewModel Consultar(ClientBindingModel request)
        {
           // var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_CLIENT";
            var sPackageName = "PKG_BDU_CLIENTE1.SP_LIST_CLIENT";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();
            result.EListClient = new List<ClientViewModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_CODAPLICACION", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPOPER", OracleDbType.Varchar2, request.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, request.P_SFIRSTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, request.P_SLASTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, request.P_SLASTNAME2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, request.P_SLEGALNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_CUSPP", OracleDbType.Varchar2, request.P_COD_CUSPP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREGIST", OracleDbType.Varchar2, request.P_SREGIST, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLIENT_SEG", OracleDbType.Varchar2, request.P_NCLIENT_SEG, ParameterDirection.Input));
                //hcama@mg 03.06.21 ini
                parameter.Add(new OracleParameter("P_TIPO_BUSQUEDA", OracleDbType.Varchar2, request.P_TIPO_BUSQUEDA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_VALOR_BUSQUEDA", OracleDbType.Varchar2, request.P_VALOR_BUSQUEDA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, request.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Int64, request.P_NCERTIF, ParameterDirection.Input));
                //hcama@mg 03.06.21 fin

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, result.EListClient, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);


                this.ExecuteByStoredProcedureEX(sPackageName, parameter);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return result;
        }

        public ResponsePViewModel ConsultarProveedor(ClientBindingModel request)
        {
            //  var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_CLIENT";
              var sPackageName = "PKG_BDU_CLIENTE1.SP_LIST_CLIENT";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponsePViewModel result = new ResponsePViewModel();
            result.EListClient = new List<ClientPViewModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_CODAPLICACION", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPOPER", OracleDbType.Varchar2, request.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, request.P_SFIRSTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, request.P_SLASTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, request.P_SLASTNAME2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, request.P_SLEGALNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_CUSPP", OracleDbType.Varchar2, request.P_COD_CUSPP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREGIST", OracleDbType.Varchar2, request.P_SREGIST, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLIENT_SEG", OracleDbType.Varchar2, request.P_NCLIENT_SEG, ParameterDirection.Input));
                //hcama@mg 03.06.21 ini
                parameter.Add(new OracleParameter("P_TIPO_BUSQUEDA", OracleDbType.Varchar2, request.P_TIPO_BUSQUEDA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_VALOR_BUSQUEDA", OracleDbType.Varchar2, request.P_VALOR_BUSQUEDA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, request.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Int64, request.P_NCERTIF, ParameterDirection.Input));
                //hcama@mg 03.06.21 fin
                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, result.EListClient, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);


                this.ExecuteByStoredProcedureEX(sPackageName, parameter);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return result;
        }
        public List<ClientViewModel> ConsultarCliente(ClientBindingModel request)
        {
            //var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_CLIENT";
            var sPackageName = "PKG_BDU_CLIENTE1.SP_LIST_CLIENT";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ClientViewModel> EListClient = new List<ClientViewModel>();
            string V_NCODE = "";
            string V_SMESSAGE = "";


            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_CODAPLICACION", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPOPER", OracleDbType.Varchar2, request.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, request.P_SFIRSTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, request.P_SLASTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, request.P_SLASTNAME2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, request.P_SLEGALNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_CUSPP", OracleDbType.Varchar2, request.P_COD_CUSPP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREGIST", OracleDbType.Varchar2, request.P_SREGIST, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLIENT_SEG", OracleDbType.Varchar2, request.P_NCLIENT_SEG, ParameterDirection.Input));
                //hcama@mg 03.06.21 ini
                parameter.Add(new OracleParameter("P_TIPO_BUSQUEDA", OracleDbType.Varchar2, request.P_TIPO_BUSQUEDA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_VALOR_BUSQUEDA", OracleDbType.Varchar2, request.P_VALOR_BUSQUEDA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, request.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Int64, request.P_NCERTIF, ParameterDirection.Input));
                //hcama@mg 03.06.21 fin
                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, V_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, V_SMESSAGE, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, EListClient, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);

                
                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    EListClient = dr.ReadRowsList<ClientViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EListClient;
        }
        public List<ClientPViewModel> ConsultarClienteProveedor(ClientBindingModel request)
        {
            // var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_CLIENT";
            var sPackageName = "PKG_BDU_CLIENTE1.SP_LIST_CLIENT";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ClientPViewModel> EListClient = new List<ClientPViewModel>();
            string V_NCODE = "";
            string V_SMESSAGE = "";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_CODAPLICACION", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPOPER", OracleDbType.Varchar2, request.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, request.P_SFIRSTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, request.P_SLASTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, request.P_SLASTNAME2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, request.P_SLEGALNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_CUSPP", OracleDbType.Varchar2, request.P_COD_CUSPP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREGIST", OracleDbType.Varchar2, request.P_SREGIST, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLIENT_SEG", OracleDbType.Varchar2, request.P_NCLIENT_SEG, ParameterDirection.Input));
                //hcama@mg 03.06.21 ini
                parameter.Add(new OracleParameter("P_TIPO_BUSQUEDA", OracleDbType.Varchar2, request.P_TIPO_BUSQUEDA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_VALOR_BUSQUEDA", OracleDbType.Varchar2, request.P_VALOR_BUSQUEDA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, request.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Int64, request.P_NCERTIF, ParameterDirection.Input));
                //hcama@mg 03.06.21 fin
                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, V_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, V_SMESSAGE, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, EListClient, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    EListClient = dr.ReadRowsList<ClientPViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EListClient;
        }

        public List<AddressViewModel> ConsultarClienteDireccion(string P_SCLIENT)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_ADDRESS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<AddressViewModel> EListAddresClient = new List<AddressViewModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    EListAddresClient = dr.ReadRowsList<AddressViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EListAddresClient;
        }

        public List<PhoneViewModel> ConsultarClienteTelefono(string P_SCLIENT)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_PHONES";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PhoneViewModel> EListPhoneClient = new List<PhoneViewModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    EListPhoneClient = dr.ReadRowsList<PhoneViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EListPhoneClient;
        }

        public List<EmailViewModel> ConsultarClienteEmail(string P_SCLIENT)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_EMAILS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<EmailViewModel> EListEmailClient = new List<EmailViewModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    EListEmailClient = dr.ReadRowsList<EmailViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EListEmailClient;
        }

        public List<ContactViewModel> ConsultarClienteContacto(string P_SCLIENT, string P_NPERSON_TYP)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_CONTACTS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ContactViewModel> EListContactClient = new List<ContactViewModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPERSON_TYP", OracleDbType.Varchar2, P_NPERSON_TYP, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    EListContactClient = dr.ReadRowsList<ContactViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EListContactClient;
        }

        public List<CiiuViewModel> ConsultarClienteCiiu(string P_SCLIENT)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_CIIU";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CiiuViewModel> EListCIIUClient = new List<CiiuViewModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    EListCIIUClient = dr.ReadRowsList<CiiuViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EListCIIUClient;
        }

        public List<HistoryViewModel> ConsultarClienteHistory(string P_SCLIENT)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_HIST_CLIENT";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<HistoryViewModel> EListHistoryClient = new List<HistoryViewModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    EListHistoryClient = dr.ReadRowsList<HistoryViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return EListHistoryClient;
        }

        public List<ApplicationsBindingModel> ConsultarAplicacionesGC()
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_APLLICATIONS_GS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ApplicationsBindingModel> ListApplicationsModels = new List<ApplicationsBindingModel>();

            try
            {
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListApplicationsModels = dr.ReadRowsList<ApplicationsBindingModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListApplicationsModels;
        }




        public int ObtenerCodigoPais(string Nacionalidad)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_OBT_NACIONALIDAD";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();
            result.EListClient = new List<ClientViewModel>();
            int IdNacionalidad = 0;
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SDESNACIONALIDAD_IN", OracleDbType.Varchar2, Nacionalidad, ParameterDirection.Input));
                //OUTPUT
                OracleParameter DES_NACI = new OracleParameter("P_SDESNACIONALIDAD_OUT", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter ID_NACI = new OracleParameter("P_NNATIONALITY", OracleDbType.Int32, ParameterDirection.Output);

                DES_NACI.Size = 1000;
                ID_NACI.Size = 100;

                parameter.Add(DES_NACI);
                parameter.Add(ID_NACI);



                this.ExecuteByStoredProcedureEX(sPackageName, parameter);

                IdNacionalidad = (ID_NACI.Value == null) ? 0 : Convert.ToInt32(ID_NACI.Value.ToString());


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return IdNacionalidad;
        }
        //Implementacion Intermediario 2020
        public List<InfoBancariaBindingModel> ConsultarInfoBancaria(string P_SCLIENT)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_CLIENT_INFO_BANCARIA";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<InfoBancariaBindingModel> ElistInfoBancaria = new List<InfoBancariaBindingModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ElistInfoBancaria = dr.ReadRowsList<InfoBancariaBindingModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ElistInfoBancaria;
        }
        public List<DocumentosBindingModel> ConsultarDocumentosAdjunto(string P_SCLIENT)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_LIST_CLIENT_DOCUMENT_ADJ";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<DocumentosBindingModel> ElistDocumentosAdjuntos = new List<DocumentosBindingModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ElistDocumentosAdjuntos = dr.ReadRowsList<DocumentosBindingModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ElistDocumentosAdjuntos;
        }      


        public ResponseSegmentoViewModel ConsultarSegementoporDocumento(SegmentoBindingModel request)
        {
          
            var sPackageName = "PKG_BDU_INFO_CLIENTE.SPS_LIST_OBTENER_SEGMENTO_CLIENTE";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<SegmentoViewModel> EListSegmento = new List<SegmentoViewModel>();
            ResponseSegmentoViewModel result = new ResponseSegmentoViewModel();
            result.EListSegmento = new List<SegmentoViewModel>();

            try
            {
                //INPUT

                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));   

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, result.EListSegmento, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);


                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    EListSegmento = dr.ReadRowsList<SegmentoViewModel>();
                }
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                if (EListSegmento.Count > 0)
                {
                    result.EListSegmento = EListSegmento;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return result;
        }
    }
}
