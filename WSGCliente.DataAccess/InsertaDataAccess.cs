using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSGCliente.Entities.BindingModel;
using WSGCliente.Entities.BindingModel.Intermediarios;
using WSGCliente.Entities.ViewModel;
using WSGCliente.Util;

namespace WSGCliente.DataAccess
{
    public class InsertaDataAccess : ConnectionBase
    {
        public ResponseViewModel ValidarClienteGeneral(ClientBindingModel request)
        {
            ResponseViewModel response = null;
            try
            {
                response = ValidarCliente(request);
                if (response.P_NCODE != "1")
                {
                    if (request.EListCIIUClient != null)
                    {
                        foreach (var item in request.EListCIIUClient)
                        {
                            if (response.P_NCODE != "1")
                            {
                                response = ValidarCiiu(item, request);
                            }
                        }
                    }
                    if (request.EListAddresClient != null)
                    {
                        foreach (var item in request.EListAddresClient)
                        {
                            if (response.P_NCODE != "1")
                            {
                                response = ValidarDireccion(item, request);
                            }

                        }
                    }
                    if (request.EListContactClient != null)
                    {
                        foreach (var item in request.EListContactClient)
                        {
                            if (response.P_NCODE != "1")
                            {
                                response = ValidarContacto(item, request);
                            }

                        }
                    }
                    if (request.EListEmailClient != null)
                    {
                        foreach (var item in request.EListEmailClient)
                        {
                            if (response.P_NCODE != "1")
                            {
                                response = ValidarCorreo(item, request);
                            }

                        }
                    }
                    if (request.EListPhoneClient != null)
                    {
                        foreach (var item in request.EListPhoneClient)
                        {
                            if (response.P_NCODE != "1")
                            {
                                response = ValidarTelefono(item, request);
                            }

                        }
                    }
                }
                if (response.P_NCODE == "0")
                {
                    response.P_SMESSAGE = "No se encontraron errores al validar";
                }
            }
            catch (Exception ex)
            {
                response.P_SMESSAGE = ex.Message;
                response.P_NCODE = "1";
            }
            return response;
        }

        public ResponseViewModel DestinatarioEmail(string nUSERCODE)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_OBT_CORREO_ENVIO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                parameter.Add(new OracleParameter("P_USUARIO", OracleDbType.Varchar2, nUSERCODE, ParameterDirection.Input));


                OracleParameter P_SEMAIL = new OracleParameter("P_SEMAIL", OracleDbType.Varchar2, result.P_SEMAIL, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_SEMAIL.Size = 4000;
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_SEMAIL);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_SEMAIL = P_SEMAIL.Value.ToString();
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.P_NCODE = "1";
                result.P_SMESSAGE = ex.Message;
                result.P_SEMAIL = ConfigurationManager.AppSettings["wstr_EmailAdmin"];
            }
            return result;
        }

        public ResponseViewModel ValidarClienteGeneralMasivo(ClientBindingModel request)
        {
            List<ListViewErrores> listViewErrores = new List<ListViewErrores>();
            ResponseViewModel response = null;
            
            try
            {
                response = ValidarCliente(request);

                listViewErrores = listViewErrores.Union(response.EListErrores).ToList();


                if (response.P_NCODE != "3"){
                    if (request.EListCIIUClient != null)
                    {
                        foreach (var item in request.EListCIIUClient)
                         {
                                response = ValidarCiiu(item, request);
                         }
                    }
                    if(request.EListAddresClient != null)
                    {
                      foreach(var item in request.EListAddresClient)
                        {
                                response = ValidarDireccion(item, request);
                                listViewErrores = listViewErrores.Union(response.EListErrores).ToList();
                        }
                    }
                    if (request.EListContactClient != null)
                    {
                        foreach (var item in request.EListContactClient)
                        {
                                response = ValidarContacto(item, request);
                        }
                    }
                    if (request.EListEmailClient != null)
                    {
                        foreach (var item in request.EListEmailClient)
                        {                      
                                response = ValidarCorreo(item, request);
                                listViewErrores = listViewErrores.Union(response.EListErrores).ToList();     
                        }
                    }
                    if (request.EListPhoneClient != null)
                    {
                        foreach (var item in request.EListPhoneClient)
                        {
                                response = ValidarTelefono(item, request);
                                listViewErrores = listViewErrores.Union(response.EListErrores).ToList();
                        }
                    }
                }
                //if (response.P_NCODE == "0"){
                //    response.P_SMESSAGE = "No se encontraron errores al validar";
                // }   
                if (listViewErrores.Count > 0) {
                    response.P_NCODE = "1";
                    response.P_SMESSAGE = listViewErrores[0].SMENSAJE;
                    response.EListErrores = listViewErrores;
                }

            }catch(Exception ex)
            {
                response.P_SMESSAGE = ex.Message;
                response.P_NCODE = "1";
                
            }
            return response;
        }

        public ResponseViewModel InsertarClienteGeneral(ClientBindingModel request)
        {
            ResponseViewModel response = null;
            var listResponse = new List<ResponseViewModel>();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            string CodClient = "";
            var EnvioSEACSA = 0;
            try
            {
                
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();
                response = InsertarCliente(request, DataConnection, trx);
                CodClient = response.P_SCOD_CLIENT;
                if(request.P_SCREDIT_CARD == "RET" && request.P_CodAplicacion == "SICCRM" && response.P_NCODE == "0")
                {
                    trx.Commit();
                    return response;
                }
                listResponse.Add(response);
                //Address
                if (response.P_NCODE == "0" || response.P_NCODE == "2")
                {
                    if (request.EListAddresClient != null)
                    {
                        foreach (var item in request.EListAddresClient)
                        {
                            if (response.P_NCODE == "0" || response.P_NCODE == "2")
                            {
                                response = InsertarDireccion(request, item, DataConnection, trx);
                                listResponse.Add(response);
                                if (response.P_NCODE == "0")
                                {
                                    EnvioSEACSA = 1;
                                }
                            }
                        }
                    }
                }
                //Phone
                if (response.P_NCODE == "0" || response.P_NCODE == "2")
                {
                    if (request.EListPhoneClient != null)
                    {
                        foreach (var item in request.EListPhoneClient)
                        {
                            if (response.P_NCODE == "0" || response.P_NCODE == "2")
                            {
                                response = InsertarTelefono(request, item, DataConnection, trx);
                                listResponse.Add(response);
                                if (response.P_NCODE == "0")
                                {
                                    EnvioSEACSA = 1;
                                }
                            }
                        }
                    }

                }
                //Email
                if (response.P_NCODE == "0" || response.P_NCODE == "2")
                {
                    if (request.EListEmailClient != null)
                    {
                        foreach (var item in request.EListEmailClient)
                        {
                            if (response.P_NCODE == "0" || response.P_NCODE == "2")
                            {
                                response = InsertarEmail(request, item, DataConnection, trx);
                                listResponse.Add(response);
                                if(response.P_NCODE == "0")
                                {
                                    EnvioSEACSA = 1;
                                }
                            }
                        }
                    }
                }
                //Contact
                if (response.P_NCODE == "0" || response.P_NCODE == "2")
                {
                    if (request.EListContactClient != null)
                    {
                        foreach (var item in request.EListContactClient)
                        {
                            if (response.P_NCODE == "0" || response.P_NCODE == "2")
                            {
                                response = InsertarContacto(request, item, DataConnection, trx);
                                listResponse.Add(response);
                            }
                        }
                    }
                }
                //CIIU
                if (response.P_NCODE == "0" || response.P_NCODE == "2")
                {
                    if (request.EListCIIUClient != null)
                    {
                        foreach (var item in request.EListCIIUClient)
                        {
                            if (response.P_NCODE == "0" || response.P_NCODE == "2")
                            {
                                response = InsertarCiiu(request, item, DataConnection, trx);
                                listResponse.Add(response);
                            }
                        }
                    }
                }
                //Info Bancaria
                if (response.P_NCODE == "0" || response.P_NCODE == "2")
                {
                    if (request.ElistInfoBancariaClient != null)
                    {
                        foreach (var item in request.ElistInfoBancariaClient)
                        {
                            if (response.P_NCODE == "0" || response.P_NCODE == "2")
                            {
                                response = InsertarInfoBancaria(request, item, DataConnection, trx);
                                listResponse.Add(response);
                            }
                        }
                    }
                }

                if (response.P_NCODE == "0" || response.P_NCODE == "2")
                {
                    if (request.ElistDocumentClient != null)
                    {
                        foreach (var item in request.ElistDocumentClient)
                        {
                            if (response.P_NCODE == "0" || response.P_NCODE == "2")
                            {
                                response = InsertarArchivoAdjuntos(request, item, DataConnection, trx);
                                listResponse.Add(response);
                            }
                        }
                    }
                }
                //FOTO_CLIENTE
                if (response.P_NCODE == "0" || response.P_NCODE == "2") {
                    if (request.P_NIDDOC_TYPE == "2") {
  
                                if (!string.IsNullOrWhiteSpace(CodClient)) {
                                    response = InsertarClienteFoto(CodClient, request.P_SIDDOC, DataConnection, trx);
                                    listResponse.Add(response);
                                }
   
                    }
                }

                

                var countLista = listResponse.Count();
                var countSCam = listResponse.Where(x => x.P_NCODE == "2").Count();
                var countError = listResponse.Where(x => x.P_NCODE == "1").Count();

                if (countError == 0)
                {
                    if (countLista == countSCam)
                    {
                        response.P_NCODE = "2";
                        response.P_SMESSAGE = "No se ha realizado ninguna modificaci�n en el cliente";
                    }
                    else
                    {
                        //  AgregarSIACSA(request);
                        response.P_NCODE = "0";
                        response.P_SMESSAGE = "Se ha realizado la actualizaci�n correctamente";
                        if (request.P_SISSEACSA_IND == "1" && EnvioSEACSA == 1)
                        {
                            try
                            {
                                request.P_NIDDOC_TYPE = HomologarCamposTrx("SEACSA", "RDOCIDE", request.P_NIDDOC_TYPE, DataConnection,trx);
                                
                                ResponseViewModel ResponseSEACSA = AgregarSEACSA(request);
                                
                                // ResponseViewModel ResponseSEACSA = new ResponseViewModel
                                // {
                                //    P_NCODE = "0",
                                //};
                                if (ResponseSEACSA.P_NCODE == "0" || ResponseSEACSA.P_NCODE == "2" )
                                {
                                    //INI MARC
                                    ResponseViewModel ResponseRentas = ActualizarDatosRentasV(request);//MARC
                                    if (ResponseRentas.P_NCODE == "0" || ResponseRentas.P_NCODE == "2")
                                    {
                                        trx.Commit();
                                    }
                                    //FIN MARC
                                }
                                else
                                {
                                    if(ResponseSEACSA.P_NCODE == "4")
                                    {
                                        response.P_NCODE = "0";
                                        response.P_SMESSAGE = "El cliente cuenta con un endoso pendiente de aprobar.Solo se ha realizado la actualizaci�n en la base unica de clientes";
                                        trx.Commit();
                                    }
                                    else
                                    {
                                        response.P_NCODE = "1";
                                        response.P_SMESSAGE = ResponseSEACSA.P_SMESSAGE;
                                        trx.Rollback();
                                    }
                                    
                                }

                            }
                            catch (Exception ex)
                            {
                                response.P_NCODE = "1";
                                response.P_SMESSAGE = "Ocurrio un problema al registrar en SEACSA";
                                trx.Rollback();
                            }

                        }
                        else
                        {
                            trx.Commit();
                        }
                    }
                }
                else
                {
                    trx.Rollback();
                }

                response.P_SCOD_CLIENT = CodClient;
            }
            catch (Exception ex)
            {
                if (trx != null) trx.Rollback();
                throw ex;

            }
            finally
            {
                DataConnection.Close();
                trx.Dispose();
            }

            return response;
        }

        public AddressBindingModel GetUltimeAddress(List<AddressBindingModel> addressBindingModels)
        {
            AddressBindingModel SetAddress = new AddressBindingModel();
          if(addressBindingModels != null) {
                if (addressBindingModels.Count > 0)
                {
                    List<AddressBindingModel> Listaddress = addressBindingModels;
                    var MaxIns = Listaddress.Where(x => x.P_TipOper != "DEL").Max(y => y.p_NUM_INSERT);
                    if (MaxIns != null)
                    {
                        SetAddress = Listaddress.Where(x => x.p_NUM_INSERT == MaxIns).FirstOrDefault();
                    }
                    else
                    {
                        var MaxUpd = Listaddress.Where(x => x.P_TipOper != "DEL").Max(y => y.P_NUM_UPDATE);
                        if (MaxUpd != null)
                        {
                            SetAddress = Listaddress.Where(x => x.P_NUM_UPDATE == MaxUpd).FirstOrDefault();
                        }
                    }
                }
            }

            return SetAddress;
        }
        public PhoneBindingModel GetUltimePhone(List<PhoneBindingModel> phoneBindingModels, string typePhone)
        {
         
            PhoneBindingModel SetPhone = new PhoneBindingModel();
            if (phoneBindingModels != null) {
                if (phoneBindingModels.Count > 0)
                {

                    List<PhoneBindingModel> ListPhone = phoneBindingModels;
                    var MaxIns = ListPhone.Where(x => x.P_TipOper != "DEL" && x.P_NPHONE_TYPE == typePhone).Max(y => y.p_NUM_INSERT);
                    if (MaxIns != null)
                    {
                        SetPhone = ListPhone.Where(x => x.p_NUM_INSERT == MaxIns).FirstOrDefault();
                    }
                    else
                    {
                        var MaxUpd = ListPhone.Where(x => x.P_TipOper != "DEL" && x.P_NPHONE_TYPE == typePhone).Max(y => y.P_NUM_UPDATE);
                        if (MaxUpd != null)
                        {
                            SetPhone = ListPhone.Where(x => x.P_NUM_UPDATE == MaxUpd).FirstOrDefault();
                        }
                    }
                }
            }
            return SetPhone;
        }
        public EmailBindingModel GetUltimeEmail(List<EmailBindingModel> emailBindingModels)
        {
            EmailBindingModel SetEmail = new EmailBindingModel();
            if (emailBindingModels != null)
            {
                if (emailBindingModels.Count > 0)
                {
                    List<EmailBindingModel> ListEmail = emailBindingModels;
                    var MaxIns = ListEmail.Where(x => x.P_TipOper != "DEL").Max(y => y.p_NUM_INSERT);
                    if (MaxIns != null)
                    {
                        SetEmail = ListEmail.Where(x => x.p_NUM_INSERT == MaxIns).FirstOrDefault();
                    }
                    else
                    {
                        var MaxUpd = ListEmail.Where(x => x.P_TipOper != "DEL").Max(y => y.P_NUM_UPDATE);
                        if (MaxUpd != null)
                        {
                            SetEmail = ListEmail.Where(x => x.P_NUM_UPDATE == MaxUpd).FirstOrDefault();
                        }
                    }
                }
            }
            return SetEmail;
        }

        public ResponseViewModel AgregarSEACSA(ClientBindingModel Client)
        {
            ResponseViewModel response = null;
            //Obteniendo el ultimo registro
            AddressBindingModel address = GetUltimeAddress(Client.EListAddresClient);
            PhoneBindingModel phoneCelular = GetUltimePhone(Client.EListPhoneClient,"2");
            PhoneBindingModel phoneParticular = GetUltimePhone(Client.EListPhoneClient, "4");
            EmailBindingModel email = GetUltimeEmail(Client.EListEmailClient);
            if (address.P_SRECTYPE == null && phoneCelular.P_NPHONE_TYPE == null && email.P_SRECTYPE == null && phoneParticular == null)
            {
                return new ResponseViewModel
                {
                    P_NCODE = "0",
                    P_SMESSAGE = "No se encontro actualizaciones para SEACSA"
                };
            }


            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVentasV);
            DbTransaction trx = null;
            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();
                
                response = SaveSIACSA(Client, phoneCelular, phoneParticular, email, address, DataConnection, trx);

                if (response.P_NCODE == "0" || response.P_NCODE == "2")
                {
                    trx.Commit();
                }
                else
                {
                    trx.Rollback();
                }
                

            }
            catch(Exception ex)
            {
                if (trx != null) trx.Rollback();
                throw ex;
            }
            finally
            {
                DataConnection.Close();
                trx.Dispose();
            }
            return response;

        }

        //INI MARC
        public ResponseViewModel ActualizarDatosRentasV(ClientBindingModel Client)
        {
            ResponseViewModel response = null;
            //Obteniendo el ultimo registro
            AddressBindingModel address = GetUltimeAddress(Client.EListAddresClient);
            PhoneBindingModel phoneCelular = GetUltimePhone(Client.EListPhoneClient, "2");
            PhoneBindingModel phoneParticular = GetUltimePhone(Client.EListPhoneClient, "4");
            EmailBindingModel email = GetUltimeEmail(Client.EListEmailClient);

            if (address.P_SRECTYPE == null && phoneCelular.P_NPHONE_TYPE == null && email.P_SRECTYPE == null && phoneParticular == null)
            {
                return new ResponseViewModel
                {
                    P_NCODE = "0",
                    P_SMESSAGE = "No se encontro actualizaciones para RENTAS"
                };
            }

            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVentasV);
            DbTransaction trx = null;
            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                response = UpdateRentasV(Client, phoneCelular, phoneParticular, email, address, DataConnection, trx);

                if (response.P_NCODE == "0" || response.P_NCODE == "2")
                {
                    trx.Commit();
                }
                else
                {
                    trx.Rollback();
                }


            }
            catch (Exception ex)
            {
                if (trx != null) trx.Rollback();
                throw ex;
            }
            finally
            {
                DataConnection.Close();
                trx.Dispose();
            }
            return response;

        }
        //FIN MARC

        public DireccionCompletaViewModel DireccionCompleta(DireccionCompletaBindingModel request)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_FORMA_DESDIREBUSQ";
            List<OracleParameter> parameter = new List<OracleParameter>();
            DireccionCompletaViewModel result = new DireccionCompletaViewModel();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_DIRE", OracleDbType.Varchar2, request.P_STI_DIRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_DIRECCION", OracleDbType.Varchar2, request.P_SNOM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_DIRECCION", OracleDbType.Varchar2, request.P_SNUM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_BLOCKCHALET", OracleDbType.Varchar2, request.P_STI_BLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKCHALET", OracleDbType.Varchar2, request.P_SBLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_INTERIOR", OracleDbType.Varchar2, request.P_STI_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_INTERIOR", OracleDbType.Varchar2, request.P_SNUM_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_CJHT", OracleDbType.Varchar2, request.P_STI_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_CJHT", OracleDbType.Varchar2, request.P_SNOM_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SETAPA", OracleDbType.Varchar2, request.P_SETAPA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMANZANA", OracleDbType.Varchar2, request.P_SMANZANA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLOTE", OracleDbType.Varchar2, request.P_SLOTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREFERENCIA", OracleDbType.Varchar2, request.P_SREFERENCIA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPROVINCE", OracleDbType.Varchar2, request.P_NPROVINCE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLOCAL", OracleDbType.Varchar2, request.P_NLOCAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Varchar2, request.P_NMUNICIPALITY, ParameterDirection.Input));
                
                //OUTPUT
                //OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SDESDIREBUSQ = new OracleParameter("P_SDESDIREBUSQ", OracleDbType.Varchar2, result.P_SDESDIREBUSQ, ParameterDirection.Output);

                //P_NCODE.Size = 4000;
                P_SDESDIREBUSQ.Size = 4000;

                //parameter.Add(P_NCODE);
                parameter.Add(P_SDESDIREBUSQ);


                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_SDESDIREBUSQ = P_SDESDIREBUSQ.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public ResponseViewModel SaveSIACSA(ClientBindingModel Client, PhoneBindingModel phone, PhoneBindingModel phone2,EmailBindingModel email, AddressBindingModel address, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = "SP_ACTUALIZA_DIR_CLI_VT";
            List<OracleParameter> parameter = new List<OracleParameter>();

            var CodUsuarioSEACSA = System.Configuration.ConfigurationManager.AppSettings["CodUsuarioSEACSA"];

            ResponseViewModel result = new ResponseViewModel();
            try
            {
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, Client.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SID_DOC", OracleDbType.Varchar2, Client.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_DIRE_VIA", OracleDbType.Varchar2, address.P_STI_DIRE , ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_DIRECCION", OracleDbType.Varchar2, address.P_SNOM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DIRECCION", OracleDbType.Varchar2, address.P_SNUM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_BLOCKCHALET", OracleDbType.Varchar2, address.P_STI_BLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_BLOCKCHALET", OracleDbType.Varchar2, address.P_SBLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_INTERIOR", OracleDbType.Varchar2, address.P_STI_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_INTERIOR", OracleDbType.Varchar2, address.P_SNUM_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_CJHT", OracleDbType.Varchar2, address.P_STI_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_NOM_CJHT", OracleDbType.Varchar2, address.P_SNOM_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_ETAPA", OracleDbType.Varchar2, address.P_SETAPA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_MANZANA", OracleDbType.Varchar2, address.P_SMANZANA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_LOTE", OracleDbType.Varchar2, address.P_SLOTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_REFERENCIA", OracleDbType.Varchar2, address.P_SREFERENCIA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_PAIS", OracleDbType.Varchar2, address.P_NCOUNTRY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_DEPARTAMENTO", OracleDbType.Varchar2, address.P_NPROVINCE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_PROVINCIA", OracleDbType.Varchar2, address.P_NLOCAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_DISTRITO", OracleDbType.Varchar2, address.P_NMUNICIPALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_TIPO_TELEFONO", OracleDbType.Varchar2, phone.P_NPHONE_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_AREA", OracleDbType.Varchar2, phone.P_NAREA_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_TELEFONO", OracleDbType.Varchar2, phone.P_SPHONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_TIPO_TELEFONO_2", OracleDbType.Varchar2, phone2.P_NPHONE_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_AREA_2", OracleDbType.Varchar2, phone2.P_NAREA_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_TELEFONO_2", OracleDbType.Varchar2, phone2.P_SPHONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_CORREO", OracleDbType.Varchar2, email.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_USUARIOCREA", OracleDbType.Varchar2, CodUsuarioSEACSA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_USUARIOCREA_VT", OracleDbType.Varchar2, Client.P_NUSERCODE, ParameterDirection.Input));


                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);


                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch(Exception ex)
            {
                result.P_NCODE = "1";
                result.P_SMESSAGE = ex.Message;
            }
            return result;
        }

        public ResponseViewModel UpdateRentasV(ClientBindingModel Client, PhoneBindingModel phone, PhoneBindingModel phone2, EmailBindingModel email, AddressBindingModel address, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = "SP_ACTUALIZA_DATOS_CONTACTO";
            List<OracleParameter> parameter = new List<OracleParameter>();

            var CodUsuarioRENTASV = ConfigurationManager.AppSettings["CodUsuarioRENTAS"];
            DbConnection cn = ConnectionGet(enuTypeDataBase.OracleVTime);
            string tipoFijo = "", nroFijo = "", tipoCelular = "", nroCelular = "";

            if (!string.IsNullOrEmpty(phone.P_SPHONE) || !string.IsNullOrEmpty(phone.P_SKEYADDRESS))
            {
                tipoCelular = phone.P_NPHONE_TYPE;
                nroCelular = phone.P_SPHONE;
            }

            if (!string.IsNullOrEmpty(phone2.P_SPHONE) || !string.IsNullOrEmpty(phone2.P_SKEYADDRESS))
            {
                tipoFijo += HomologarCamposTrx("RENTASTOTAL", "TIPTEL", phone2.P_NPHONE_TYPE, cn, trx);
                nroFijo += phone2.P_SPHONE;
            }

            ResponseViewModel result = new ResponseViewModel();
            try
            {
                parameter.Add(new OracleParameter("P_NRODOCU", OracleDbType.Varchar2, Client.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_EMAIL", OracleDbType.Varchar2, string.IsNullOrEmpty(email.P_SE_MAIL) ? "" : email.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_TELEFONO", OracleDbType.Varchar2, string.IsNullOrEmpty(tipoCelular) ? "" : tipoCelular, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_FIJO", OracleDbType.Varchar2, string.IsNullOrEmpty(tipoFijo) ? "" : tipoFijo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_TELEFONO", OracleDbType.Varchar2, string.IsNullOrEmpty(nroCelular) ? "" : nroCelular, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_FIJO", OracleDbType.Varchar2, string.IsNullOrEmpty(nroFijo) ? "" : nroFijo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DIRECCION", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_SNOM_DIRECCION) ? "" : address.P_SNOM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_DEPARTAMENTO", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_NPROVINCE) ? "" : address.P_NPROVINCE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_PROVINCIA", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_NLOCAL) ? "" : address.P_NLOCAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_DISTRITO", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_NMUNICIPALITY) ? "" : address.P_NMUNICIPALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_AREA", OracleDbType.Decimal, string.IsNullOrEmpty(phone2.P_NAREA_CODE) ? 0 : double.Parse(phone2.P_NAREA_CODE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DIRECCION", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_SNUM_DIRECCION) ? "" : address.P_SNUM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_BLOCKCHALET", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_STI_BLOCKCHALET) ? "" : address.P_STI_BLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_BLOCKCHALET", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_SBLOCKCHALET) ? "" : address.P_SBLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_INTERIOR", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_STI_INTERIOR) ? "" : address.P_STI_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_INTERIOR", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_SNUM_INTERIOR) ? "" : address.P_SNUM_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_CJHT", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_STI_CJHT) ? "" : address.P_STI_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_NOM_CJHT", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_SNOM_CJHT) ? "" : address.P_SNOM_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_ETAPA", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_SETAPA) ? "" : address.P_SETAPA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_MANZANA", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_SMANZANA) ? "" : address.P_SMANZANA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_LOTE", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_SLOTE) ? "" : address.P_SLOTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_GLS_REFERENCIA", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_SREFERENCE) ? "" : address.P_SREFERENCE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_DIRE_VIA", OracleDbType.Varchar2, string.IsNullOrEmpty(address.P_STI_DIRE) ? "" : address.P_STI_DIRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_USUARIO", OracleDbType.Varchar2, string.IsNullOrEmpty(CodUsuarioRENTASV) ? "" : CodUsuarioRENTASV, ParameterDirection.Input));  

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.P_NCODE = "1";
                result.P_SMESSAGE = ex.Message;
            }
            return result;
        }
        public ResponseViewModel InsertarCliente(ClientBindingModel request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_INS_CLIENTE";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();
            //result.EListClient = new List<ClientViewModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, request.P_SFIRSTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, request.P_SLASTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, request.P_SLASTNAME2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, request.P_SLEGALNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Varchar2, request.P_SSEXCLIEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINCAPACITY", OracleDbType.Varchar2, request.P_NINCAPACITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINCAP_COD", OracleDbType.Varchar2, request.P_NINCAP_COD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, request.P_DBIRTHDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DINCAPACITY", OracleDbType.Varchar2, request.P_DINCAPACITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NHEALTH_ORG", OracleDbType.Varchar2, request.P_NHEALTH_ORG, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDEATHDAT", OracleDbType.Varchar2, request.P_DDEATHDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DWEDD", OracleDbType.Varchar2, request.P_DWEDD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SACCOUNT_IN", OracleDbType.Varchar2, request.P_SACCOUNT_IN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINVOICING", OracleDbType.Varchar2, request.P_NINVOICING, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSPECIALITY", OracleDbType.Varchar2, request.P_NSPECIALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIVILSTA", OracleDbType.Varchar2, request.P_NCIVILSTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DAPROBDATE", OracleDbType.Varchar2, request.P_DAPROBDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKADE", OracleDbType.Varchar2, request.P_SBLOCKADE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLASS", OracleDbType.Varchar2, request.P_NCLASS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDRIVERDAT", OracleDbType.Varchar2, request.P_DDRIVERDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NHEIGHT", OracleDbType.Varchar2, request.P_NHEIGHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NHOUSE_TYPE", OracleDbType.Varchar2, request.P_NHOUSE_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLICENSE", OracleDbType.Varchar2, request.P_SLICENSE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNOTENUM", OracleDbType.Varchar2, request.P_NNOTENUM, ParameterDirection.Input));
                parameter.Add(new OracleParameter(" P_NOFFICE ", OracleDbType.Varchar2, request.P_NOFFICE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NQ_CARS", OracleDbType.Varchar2, request.P_NQ_CARS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NQ_CHILD", OracleDbType.Varchar2, request.P_NQ_CHILD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRATE", OracleDbType.Varchar2, request.P_NRATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STAX_CODE", OracleDbType.Varchar2, request.P_STAX_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSMOKING", OracleDbType.Varchar2, request.P_SSMOKING, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCUIT", OracleDbType.Varchar2, request.P_SCUIT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTITLE", OracleDbType.Varchar2, request.P_NTITLE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NWEIGHT", OracleDbType.Varchar2, request.P_NWEIGHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAUTO_CHAR", OracleDbType.Varchar2, request.P_SAUTO_CHAR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCREDIT_CARD", OracleDbType.Varchar2, request.P_SCREDIT_CARD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NECONOMIC_L", OracleDbType.Varchar2, request.P_NECONOMIC_L, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NEMPL_QUA", OracleDbType.Varchar2, request.P_NEMPL_QUA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIMAGENUM", OracleDbType.Varchar2, request.P_NIMAGENUM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAREA", OracleDbType.Varchar2, request.P_NAREA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDRIVEXPDAT", OracleDbType.Varchar2, request.P_DDRIVEXPDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPDRIVER", OracleDbType.Varchar2, request.P_NTYPDRIVER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDISABILITY", OracleDbType.Varchar2, request.P_NDISABILITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITDRIV", OracleDbType.Varchar2, request.P_NLIMITDRIV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAFP", OracleDbType.Varchar2, request.P_NAFP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBILL_IND", OracleDbType.Varchar2, request.P_SBILL_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNATIONALITY", OracleDbType.Varchar2, request.P_NNATIONALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDIGIT", OracleDbType.Varchar2, request.P_SDIGIT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DRETIREMENT", OracleDbType.Varchar2, request.P_DRETIREMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DINDEPENDANT", OracleDbType.Varchar2, request.P_DINDEPENDANT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDEPENDANT", OracleDbType.Varchar2, request.P_DDEPENDANT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMAILINGPREF", OracleDbType.Varchar2, request.P_NMAILINGPREF, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLANGUAGE", OracleDbType.Varchar2, request.P_NLANGUAGE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEFTHANDED", OracleDbType.Varchar2, request.P_SLEFTHANDED, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKLAFT", OracleDbType.Varchar2, request.P_SBLOCKLAFT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCLIENT_IND", OracleDbType.Varchar2, request.P_SISCLIENT_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCLIENT_GBD", OracleDbType.Varchar2, request.P_SISCLIENT_GBD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISRENIEC_IND", OracleDbType.Varchar2, request.P_SISRENIEC_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLIENT_SEG", OracleDbType.Varchar2, request.P_NCLIENT_SEG, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOLIZA_ELECT_IND", OracleDbType.Varchar2, request.P_SPOLIZA_ELECT_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TI_DOC_SUSTENT", OracleDbType.Varchar2, request.P_TI_DOC_SUSTENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NU_DOC_SUSTENT", OracleDbType.Varchar2, request.P_NU_DOC_SUSTENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBIG_DEP_NAC", OracleDbType.Varchar2, request.P_COD_UBIG_DEP_NAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBIG_PROV_NAC", OracleDbType.Varchar2, request.P_COD_UBIG_PROV_NAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBIG_DIST_NAC", OracleDbType.Varchar2, request.P_COD_UBIG_DIST_NAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEPARTAMENTO_NACIMIENTO", OracleDbType.Varchar2, request.P_DEPARTAMENTO_NACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PROVINCIA_NACIMIENTO", OracleDbType.Varchar2, request.P_PROVINCIA_NACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DISTRITO_NACIMIENTO", OracleDbType.Varchar2, request.P_DISTRITO_NACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRE_PADRE", OracleDbType.Varchar2, request.P_NOMBRE_PADRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRE_MADRE", OracleDbType.Varchar2, request.P_NOMBRE_MADRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_INSC", OracleDbType.Varchar2, request.P_FECHA_INSC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_EXPEDICION", OracleDbType.Varchar2, request.P_FECHA_EXPEDICION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_CONSTANCIA_VOTACION", OracleDbType.Varchar2, request.P_CONSTANCIA_VOTACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APELLIDO_CASADA", OracleDbType.Varchar2, request.P_APELLIDO_CASADA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDIG_VERIFICACION", OracleDbType.Varchar2, request.P_SDIG_VERIFICACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPROTEG_DATOS_IND", OracleDbType.Varchar2, request.P_SPROTEG_DATOS_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_CUSPP", OracleDbType.Varchar2, request.P_COD_CUSPP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SGRADO_INSTRUCCION", OracleDbType.Varchar2, request.P_SGRADO_INSTRUCCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FOTO_RENIEC", OracleDbType.Varchar2, request.P_FOTO_RENIEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FIRMA_RENIEC", OracleDbType.Varchar2, request.P_FIRMA_RENIEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCLIENT_CRITICO", OracleDbType.Varchar2, request.P_SISCLIENT_CRITICO, ParameterDirection.Input));
                
                parameter.Add(new OracleParameter("P_SISCLIENT_INTERME", OracleDbType.Varchar2, request.P_SISCLIENT_CORREDOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCLIENT_CONTRA", OracleDbType.Varchar2, request.P_SISCLIENT_CONTRATANTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCORREDOR_IND", OracleDbType.Varchar2, request.P_NIND_CORREDOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNAME_COMERCIAL", OracleDbType.Varchar2, request.P_SNAME_COMERCIAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCANT_TRABAJADORES", OracleDbType.Varchar2, request.P_NCANT_TRABAJADORES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCANT_USUARIOS", OracleDbType.Varchar2, request.P_NNUMERO_USUARIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_SBS", OracleDbType.Varchar2, request.P_SCOD_SBS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCONSE_CLAUS_IND", OracleDbType.Varchar2, request.P_SIND_CONSENTI_CLAUS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_SECTOR_EMPRESA", OracleDbType.Varchar2, request.P_NID_SECTOR_EMPRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_AREA_RESPO", OracleDbType.Varchar2, request.P_NID_AREA_RESPONSABLE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFECHA_RECEP_FACTU", OracleDbType.Varchar2, request.P_DFECHA_RECEP_FACTU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SVENTACONSORCIO_IND", OracleDbType.Varchar2, request.P_SIND_VENTA_COMERCIAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPARTIDA_ELECTRONICA", OracleDbType.Varchar2, request.P_SPARTIDA_ELECTRONICA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBAJAMAIL_IND", OracleDbType.Varchar2, request.P_SBAJAMAIL_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFE_ANIVERSARIO", OracleDbType.Varchar2, request.P_DFE_ANIVERSARIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLIDER_CONSORCIO", OracleDbType.Varchar2, request.P_SLIDER_CONSORCIO, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);
                OracleParameter P_SCOD_CLIENT = new OracleParameter("P_SCOD_CLIENT", OracleDbType.Varchar2, result.P_SCOD_CLIENT, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                P_SCOD_CLIENT.Size = 4000;
                parameter.Add(P_SCOD_CLIENT);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                result.P_SCOD_CLIENT = P_SCOD_CLIENT.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public ResponseViewModel InsertarDireccion(ClientBindingModel request, AddressBindingModel request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_INS_DIRECCION";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {

                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, request2.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRECOWNER", OracleDbType.Varchar2, request2.P_NRECOWNER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRECTYPE", OracleDbType.Varchar2, request2.P_SRECTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, request2.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTREET", OracleDbType.Varchar2, request2.P_SSTREET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SZONE", OracleDbType.Varchar2, request2.P_SZONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, request2.P_SCERTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, request2.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_SECOND", OracleDbType.Varchar2, request2.P_NLAT_SECOND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_SECOND", OracleDbType.Varchar2, request2.P_NLON_SECOND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_COORD", OracleDbType.Varchar2, request2.P_NLAT_COORD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_COORD", OracleDbType.Varchar2, request2.P_NLON_COORD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCONTRAT", OracleDbType.Varchar2, request2.P_NCONTRAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_CARDIN", OracleDbType.Varchar2, request2.P_NLAT_CARDIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_MINUTE", OracleDbType.Varchar2, request2.P_NLAT_MINUTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_CARDIN", OracleDbType.Varchar2, request2.P_NLON_CARDIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_MINUTE", OracleDbType.Varchar2, request2.P_NLON_MINUTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Varchar2, request2.P_NCERTIF, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLAIM", OracleDbType.Varchar2, request2.P_NCLAIM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, request2.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NZIP_CODE", OracleDbType.Varchar2, request2.P_NZIP_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_GRADE", OracleDbType.Varchar2, request2.P_NLAT_GRADE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOUNTRY", OracleDbType.Varchar2, request2.P_NCOUNTRY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_GRADE", OracleDbType.Varchar2, request2.P_NLON_GRADE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBK_AGENCY", OracleDbType.Varchar2, request2.P_NBK_AGENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBANK_CODE", OracleDbType.Varchar2, request2.P_NBANK_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, request2.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOFFICE", OracleDbType.Varchar2, request2.P_NOFFICE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPROVINCE", OracleDbType.Varchar2, request2.P_NPROVINCE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, request2.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLOCAL", OracleDbType.Varchar2, request2.P_NLOCAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBUILD", OracleDbType.Varchar2, request2.P_SBUILD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Varchar2, request2.P_NMUNICIPALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLOOR", OracleDbType.Varchar2, request2.P_NFLOOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDEPARTMENT", OracleDbType.Varchar2, request2.P_SDEPARTMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOPULATION", OracleDbType.Varchar2, request2.P_SPOPULATION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SINFOR", OracleDbType.Varchar2, request2.P_SINFOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOBOX", OracleDbType.Varchar2, request2.P_SPOBOX, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESCADD", OracleDbType.Varchar2, request2.P_SDESCADD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOD_AGREE", OracleDbType.Varchar2, request2.P_NCOD_AGREE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREFERENCE", OracleDbType.Varchar2, request2.P_SREFERENCE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_DIRE", OracleDbType.Varchar2, request2.P_STI_DIRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_DIRECCION", OracleDbType.Varchar2, request2.P_SNOM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_DIRECCION", OracleDbType.Varchar2, request2.P_SNUM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_BLOCKCHALET", OracleDbType.Varchar2, request2.P_STI_BLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKCHALET", OracleDbType.Varchar2, request2.P_SBLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_INTERIOR", OracleDbType.Varchar2, request2.P_STI_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_INTERIOR", OracleDbType.Varchar2, request2.P_SNUM_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_CJHT", OracleDbType.Varchar2, request2.P_STI_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_CJHT", OracleDbType.Varchar2, request2.P_SNOM_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SETAPA", OracleDbType.Varchar2, request2.P_SETAPA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMANZANA", OracleDbType.Varchar2, request2.P_SMANZANA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLOTE", OracleDbType.Varchar2, request2.P_SLOTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREFERENCIA", OracleDbType.Varchar2, request2.P_SREFERENCE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_DEP_UBI_DOM", OracleDbType.Varchar2, request2.P_SCOD_DEP_UBI_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_PRO_UBI_DOM", OracleDbType.Varchar2, request2.P_SCOD_PRO_UBI_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_DIS_UBI_DOM", OracleDbType.Varchar2, request2.P_SCOD_DIS_UBI_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDES_DEP_DOM", OracleDbType.Varchar2, request2.P_SDES_DEP_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDES_PRO_DOM", OracleDbType.Varchar2, request2.P_SDES_PRO_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDES_DIS_DOM", OracleDbType.Varchar2, request2.P_SDES_DIS_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISRENIEC_IND", OracleDbType.Varchar2, request.P_SISRENIEC_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SKEYADDRESS", OracleDbType.Varchar2, request2.P_SKEYADDRESS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NROW", OracleDbType.Varchar2, request2.P_NROW, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public ResponseViewModel InsertarTelefono(ClientBindingModel request, PhoneBindingModel request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_INS_TELEFONO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, request2.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRECOWNER", OracleDbType.Varchar2, request2.P_NRECOWNER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NKEYPHONES", OracleDbType.Varchar2, request2.P_NKEYPHONES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, request2.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBESTTIMETOCALL", OracleDbType.Varchar2, request2.P_NBESTTIMETOCALL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAREA_CODE", OracleDbType.Varchar2, request2.P_NAREA_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, request2.P_SPHONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NORDER", OracleDbType.Varchar2, request2.P_NORDER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NEXTENS1", OracleDbType.Varchar2, request2.P_NEXTENS1, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NEXTENS2", OracleDbType.Varchar2, request2.P_NEXTENS2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPHONE_TYPE", OracleDbType.Varchar2, request2.P_NPHONE_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOUNTRY_CODE", OracleDbType.Varchar2, request2.P_NCOUNTRY_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SASSOCADDR", OracleDbType.Varchar2, request2.P_SASSOCADDR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SKEYADDRESS", OracleDbType.Varchar2, request2.P_SKEYADDRESS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NROW", OracleDbType.Varchar2, request2.P_NROW, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public ResponseViewModel InsertarEmail(ClientBindingModel request, EmailBindingModel request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_INS_CORREO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, request2.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRECOWNER", OracleDbType.Varchar2, request2.P_NRECOWNER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRECTYPE", OracleDbType.Varchar2, request2.P_SRECTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, request2.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, request2.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SKEYADDRESS", OracleDbType.Varchar2, request2.P_SKEYADDRESS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NROW", OracleDbType.Varchar2, request2.P_NROW, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SINFOR", OracleDbType.Varchar2, request2.P_SINFOR, ParameterDirection.Input));
                
                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ResponseViewModel InsertarContacto(ClientBindingModel request, ContactBindingModel request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_INS_CONTACTO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, request2.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRECOWNER", OracleDbType.Varchar2, request2.P_NRECOWNER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE_CL", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC_CL", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCONT", OracleDbType.Varchar2, request2.P_NIDCONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIPCONT", OracleDbType.Varchar2, request2.P_NTIPCONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, request2.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request2.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request2.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOMBRES", OracleDbType.Varchar2, request2.P_SNOMBRES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPEPAT", OracleDbType.Varchar2, request2.P_SAPEPAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPEMAT", OracleDbType.Varchar2, request2.P_SAPEMAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, request2.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, request2.P_SPHONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNAMEAREA", OracleDbType.Varchar2, request2.P_SNAMEAREA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNAMEPOSITION", OracleDbType.Varchar2, request2.P_SNAMEPOSITION, ParameterDirection.Input)); 
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NROW", OracleDbType.Varchar2, request2.P_NROW, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE1", OracleDbType.Varchar2, request2.P_SPHONE1, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ADDRESS", OracleDbType.Varchar2, request2.P_ADDRESS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_UBIGEO", OracleDbType.Varchar2, request2.P_UBIGEO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NEXTENS", OracleDbType.Varchar2, request2.P_NEXTENS, ParameterDirection.Input));
                

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ResponseViewModel InsertarCiiu(ClientBindingModel request, CiiuBindingModel request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_INS_CIIU";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, request2.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCIIU", OracleDbType.Varchar2, request2.P_SCIIU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, request2.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NROW", OracleDbType.Varchar2, request2.P_NROW, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public string HomologarCamposTrx(string P_Origen, string Campo, string expresion,DbConnection dbConnection , DbTransaction trx)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_HOMOLDATOSOTROS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();
            string Return = "";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, P_Origen, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STIPO_DATO", OracleDbType.Varchar2, Campo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCO_DATO_OTRO", OracleDbType.Varchar2, expresion, ParameterDirection.Input));
                OracleParameter Valor = new OracleParameter("P_SCO_DATO_VTIME", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                Valor.Size = 4000;
                parameter.Add(Valor);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter,dbConnection,trx);
                Return = Valor.Value.ToString();
            }
            catch (Exception ex)
            {
                Return = ex.Message;
            }

            return Return;
        }
        public string HomologarCampos(string P_Origen, string Campo , string expresion)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_HOMOLDATOSOTROS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();
            string Return = "";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, P_Origen, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STIPO_DATO", OracleDbType.Varchar2, Campo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCO_DATO_OTRO", OracleDbType.Varchar2, expresion, ParameterDirection.Input));
                OracleParameter Valor = new OracleParameter("P_SCO_DATO_VTIME", OracleDbType.Varchar2,4000, ParameterDirection.Output);
                Valor.Size = 4000;
                parameter.Add(Valor);
               
                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                Return = Valor.Value.ToString();
            }
            catch (Exception ex)
            {
                Return = ex.Message;
            }

            return Return;
        }

   public ResponseViewModel InsertarClienteReniec(ResponseReniecViewModel request2)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_INS_CLIENT_RENIEC";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_CODIGO", OracleDbType.Varchar2, request2.CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, request2.MESSAGE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_CODIGOERROR", OracleDbType.Varchar2, request2.CODIGOERROR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUMERO_DNI", OracleDbType.Varchar2, request2.NUMERODNI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DIGITO_VERIFICA", OracleDbType.Varchar2, request2.DIGITOVERIFICACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_PATERNO", OracleDbType.Varchar2, request2.APELLIDOPATERNO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_MATERNO", OracleDbType.Varchar2, request2.APELLIDOMATERNO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_CASADA", OracleDbType.Varchar2, request2.APELLIDOCASADA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRES", OracleDbType.Varchar2, request2.NOMBRES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBI_DEPA_DOMI", OracleDbType.Varchar2, request2.CODIGOUBIGEODEPARTAMENTODOMICILIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBI_PROV_DOMI", OracleDbType.Varchar2, request2.CODIGOUBIGEOPROVINCIADOMICILIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBI_DIST_DOMI", OracleDbType.Varchar2, request2.CODIGOUBIGEODISTRITODOMICILIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEPA_DOMICILIO", OracleDbType.Varchar2, request2.DEPARTAMENTODOMICILIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PROV_DOMICILIO", OracleDbType.Varchar2, request2.PROVINCIADOMICILIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DIST_DOMICILIO", OracleDbType.Varchar2, request2.DISTRITODOMICILIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ESTADO_CIVIL", OracleDbType.Varchar2, request2.ESTADOCIVILCIUDADANO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_GRADO_INSTRUC", OracleDbType.Varchar2, request2.CODIGOGRADOINSTRUCCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ESTATURA", OracleDbType.Varchar2, request2.ESTATURA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SEXO", OracleDbType.Varchar2, request2.SEXO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_DOC_IDENTIDAD", OracleDbType.Varchar2, request2.TIPODOCUMENTOIDENTIDAD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DOC_IDENTIDAD", OracleDbType.Varchar2, request2.NUMERODOCUMENTOIDENTIDAD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBI_DEPA_NACI", OracleDbType.Varchar2, request2.CODIGOUBIGEODEPARTAMENTONACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBI_PROV_NACI", OracleDbType.Varchar2, request2.CODIGOUBIGEOPROVINCIANACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBI_DIST_NACI", OracleDbType.Varchar2, request2.CODIGOUBIGEODISTRITONACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEPA_NACIMIENTO", OracleDbType.Varchar2, request2.DEPARTAMENTONACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PROV_NACIMIENTO", OracleDbType.Varchar2, request2.PROVINCIANACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DIST_NACIMIENTO", OracleDbType.Varchar2, request2.DISTRITONACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_NACIMIENTO", OracleDbType.Varchar2, request2.FECHANACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOM_PADRE", OracleDbType.Varchar2, request2.NOMBRESPADRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOM_MADRE", OracleDbType.Varchar2, request2.NOMBRESMADRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FEC_INSCRIPCION", OracleDbType.Varchar2, request2.FECHAINSCRIPCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FEC_EMISION", OracleDbType.Varchar2, request2.FECHAEMISION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_CONSTANCIA_VOTACI", OracleDbType.Varchar2, request2.CONSTANCIAVOTACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_RESTRICCIONES", OracleDbType.Varchar2, request2.RESTRICCIONES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PREFIJ_DIRECCION", OracleDbType.Varchar2, request2.PREFIJODIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_DIRECCION", OracleDbType.Varchar2, request2.DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUMERO_DIRECCION", OracleDbType.Varchar2, request2.NUMERODIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_BLOCKCHALET", OracleDbType.Varchar2, request2.BLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_INTERIOR", OracleDbType.Varchar2, request2.INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_URBANIZACION", OracleDbType.Varchar2, request2.URBANIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ETAPA", OracleDbType.Varchar2, request2.ETAPA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MANZANA", OracleDbType.Varchar2, request2.MANZANA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_LOTE", OracleDbType.Varchar2, request2.LOTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PREFIJ_BLOCK", OracleDbType.Varchar2, request2.PREFIJOBLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PREFIJ_INTERIOR", OracleDbType.Varchar2, request2.PREFIJODPTOPISOINTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PREFIJ_CONJ", OracleDbType.Varchar2, request2.PREFIJOURBCONDRESID, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_RESERVADOR", OracleDbType.Varchar2, request2.RESERVADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHOTO", OracleDbType.Clob, request2.FOTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSIGNATURE", OracleDbType.Clob, request2.FIRMA, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ResponseViewModel ObtenerClientReniecLocal(ClientBindingModel request) {
            var sPackageName = "PKG_BDU_CLIENTE.SP_SEL_CLIENT_RENIEC";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();
            List<ResponseReniecViewModel> ElistClientReniec = new List<ResponseReniecViewModel>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_USERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_TABLA = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ElistClientReniec, ParameterDirection.Output);

                parameter.Add(P_TABLA);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                   // ElistClientReniec = dr.ReadRowsList<ResponseReniecViewModel>();
                    ResponseReniecViewModel reniecModel = new ResponseReniecViewModel();

                    while (dr.Read())
                    {
                        reniecModel.CODE = dr["CODE"] == null || dr["CODE"].ToString() == "" ? "" : dr["CODE"].ToString();
                        reniecModel.MESSAGE = dr["MESSAGE"] == null || dr["MESSAGE"].ToString() == "" ? "" : dr["MESSAGE"].ToString();
                        reniecModel.CODIGOERROR = dr["CODIGOERROR"] == null || dr["CODIGOERROR"].ToString() == "" ? "" : dr["CODIGOERROR"].ToString();
                        reniecModel.NUMERODNI = dr["NUMERODNI"] == null || dr["NUMERODNI"].ToString() == "" ? "" : dr["NUMERODNI"].ToString();
                        reniecModel.DIGITOVERIFICACION = dr["DIGITOVERIFICACION"] == null || dr["DIGITOVERIFICACION"].ToString() == "" ? "" : dr["DIGITOVERIFICACION"].ToString();
                        reniecModel.APELLIDOPATERNO = dr["APELLIDOPATERNO"] == null || dr["APELLIDOPATERNO"].ToString() == "" ? "" : dr["APELLIDOPATERNO"].ToString();
                        reniecModel.APELLIDOMATERNO = dr["APELLIDOMATERNO"] == null || dr["APELLIDOMATERNO"].ToString() == "" ? "" : dr["APELLIDOMATERNO"].ToString();
                        reniecModel.APELLIDOCASADA = dr["APELLIDOCASADA"] == null || dr["APELLIDOCASADA"].ToString() == "" ? "" : dr["APELLIDOCASADA"].ToString();
                        reniecModel.NOMBRES = dr["NOMBRES"] == null || dr["NOMBRES"].ToString() == "" ? "" : dr["NOMBRES"].ToString();
                        //reniecModel.CODIGOUBIGEODEPARTAMENTODOMICILIO = dr["CODIGOUBIGEODEPARTAMENTODOMICILIO"] == null || dr["CODIGOUBIGEODEPARTAMENTODOMICILIO"].ToString() == "" ? "" : dr["CODIGOUBIGEODEPARTAMENTODOMICILIO"].ToString();
                        //reniecModel.CODIGOUBIGEOPROVINCIADOMICILIO = dr["CODIGOUBIGEOPROVINCIADOMICILIO"] == null || dr["CODIGOUBIGEOPROVINCIADOMICILIO"].ToString() == "" ? "" : dr["CODIGOUBIGEOPROVINCIADOMICILIO"].ToString();
                        //reniecModel.CODIGOUBIGEODISTRITODOMICILIO = dr["CODIGOUBIGEODISTRITODOMICILIO"] == null || dr["MESCODIGOUBIGEODISTRITODOMICILIOSAGE"].ToString() == "" ? "" : dr["CODIGOUBIGEODISTRITODOMICILIO"].ToString();
                        reniecModel.DEPARTAMENTODOMICILIO = dr["DEPARTAMENTODOMICILIO"] == null || dr["DEPARTAMENTODOMICILIO"].ToString() == "" ? "" : dr["DEPARTAMENTODOMICILIO"].ToString();
                        reniecModel.PROVINCIADOMICILIO = dr["PROVINCIADOMICILIO"] == null || dr["PROVINCIADOMICILIO"].ToString() == "" ? "" : dr["PROVINCIADOMICILIO"].ToString();
                        reniecModel.DISTRITODOMICILIO = dr["DISTRITODOMICILIO"] == null || dr["DISTRITODOMICILIO"].ToString() == "" ? "" : dr["DISTRITODOMICILIO"].ToString();
                        reniecModel.ESTADOCIVILCIUDADANO = dr["ESTADOCIVILCIUDADANO"] == null || dr["ESTADOCIVILCIUDADANO"].ToString() == "" ? "" : dr["ESTADOCIVILCIUDADANO"].ToString();
                        reniecModel.CODIGOGRADOINSTRUCCION = dr["CODIGOGRADOINSTRUCCION"] == null || dr["CODIGOGRADOINSTRUCCION"].ToString() == "" ? "" : dr["CODIGOGRADOINSTRUCCION"].ToString();
                        reniecModel.ESTATURA = dr["ESTATURA"] == null || dr["ESTATURA"].ToString() == "" ? "" : dr["ESTATURA"].ToString();
                        reniecModel.SEXO = dr["SEXO"] == null || dr["SEXO"].ToString() == "" ? "" : dr["SEXO"].ToString();
                        reniecModel.TIPODOCUMENTOIDENTIDAD = dr["TIPODOCUMENTOIDENTIDAD"] == null || dr["TIPODOCUMENTOIDENTIDAD"].ToString() == "" ? "" : dr["TIPODOCUMENTOIDENTIDAD"].ToString();
                        reniecModel.NUMERODOCUMENTOIDENTIDAD = dr["NUMERODOCUMENTOIDENTIDAD"] == null || dr["NUMERODOCUMENTOIDENTIDAD"].ToString() == "" ? "" : dr["NUMERODOCUMENTOIDENTIDAD"].ToString();
                        //reniecModel.CODIGOUBIGEODEPARTAMENTONACIMIENTO = dr["CODIGOUBIGEODEPARTAMENTONACIMIENTO"] == null || dr["CODIGOUBIGEODEPARTAMENTONACIMIENTO"].ToString() == "" ? "" : dr["CODIGOUBIGEODEPARTAMENTONACIMIENTO"].ToString();
                        //reniecModel.CODIGOUBIGEOPROVINCIANACIMIENTO = dr["CODIGOUBIGEOPROVINCIANACIMIENTO"] == null || dr["CODIGOUBIGEOPROVINCIANACIMIENTO"].ToString() == "" ? "" : dr["CODIGOUBIGEOPROVINCIANACIMIENTO"].ToString();
                        //reniecModel.CODIGOUBIGEODISTRITONACIMIENTO = dr["CODIGOUBIGEODISTRITONACIMIENTO"] == null || dr["CODIGOUBIGEODISTRITONACIMIENTO"].ToString() == "" ? "" : dr["CODIGOUBIGEODISTRITONACIMIENTO"].ToString();
                        reniecModel.DEPARTAMENTONACIMIENTO = dr["DEPARTAMENTONACIMIENTO"] == null || dr["DEPARTAMENTONACIMIENTO"].ToString() == "" ? "" : dr["DEPARTAMENTONACIMIENTO"].ToString();
                        reniecModel.PROVINCIANACIMIENTO = dr["PROVINCIANACIMIENTO"] == null || dr["PROVINCIANACIMIENTO"].ToString() == "" ? "" : dr["PROVINCIANACIMIENTO"].ToString();
                        reniecModel.DISTRITONACIMIENTO = dr["DISTRITONACIMIENTO"] == null || dr["DISTRITONACIMIENTO"].ToString() == "" ? "" : dr["DISTRITONACIMIENTO"].ToString();
                        reniecModel.FECHANACIMIENTO = dr["FECHANACIMIENTO"] == null || dr["FECHANACIMIENTO"].ToString() == "" ? "" : dr["FECHANACIMIENTO"].ToString();
                        reniecModel.NOMBRESPADRE = dr["NOMBRESPADRE"] == null || dr["NOMBRESPADRE"].ToString() == "" ? "" : dr["NOMBRESPADRE"].ToString();
                        reniecModel.NOMBRESMADRE = dr["NOMBRESMADRE"] == null || dr["NOMBRESMADRE"].ToString() == "" ? "" : dr["NOMBRESMADRE"].ToString();
                        reniecModel.FECHAINSCRIPCION = dr["FECHAINSCRIPCION"] == null || dr["FECHAINSCRIPCION"].ToString() == "" ? "" : dr["FECHAINSCRIPCION"].ToString();
                        reniecModel.FECHAEMISION = dr["FECHAEMISION"] == null || dr["FECHAEMISION"].ToString() == "" ? "" : dr["FECHAEMISION"].ToString();
                        reniecModel.CONSTANCIAVOTACION = dr["CONSTANCIAVOTACION"] == null || dr["CONSTANCIAVOTACION"].ToString() == "" ? "" : dr["CONSTANCIAVOTACION"].ToString();
                        reniecModel.RESTRICCIONES = dr["RESTRICCIONES"] == null || dr["RESTRICCIONES"].ToString() == "" ? "" : dr["RESTRICCIONES"].ToString();
                        reniecModel.PREFIJODIRECCION = dr["PREFIJODIRECCION"] == null || dr["PREFIJODIRECCION"].ToString() == "" ? "" : dr["PREFIJODIRECCION"].ToString();
                        reniecModel.DIRECCION = dr["DIRECCION"] == null || dr["DIRECCION"].ToString() == "" ? "" : dr["DIRECCION"].ToString();
                        reniecModel.NUMERODIRECCION = dr["NUMERODIRECCION"] == null || dr["NUMERODIRECCION"].ToString() == "" ? "" : dr["NUMERODIRECCION"].ToString();
                        reniecModel.BLOCKCHALET = dr["BLOCKCHALET"] == null || dr["BLOCKCHALET"].ToString() == "" ? "" : dr["BLOCKCHALET"].ToString();
                        reniecModel.INTERIOR = dr["INTERIOR"] == null || dr["INTERIOR"].ToString() == "" ? "" : dr["INTERIOR"].ToString();
                        reniecModel.URBANIZACION = dr["URBANIZACION"] == null || dr["URBANIZACION"].ToString() == "" ? "" : dr["URBANIZACION"].ToString();
                        reniecModel.ETAPA = dr["ETAPA"] == null || dr["ETAPA"].ToString() == "" ? "" : dr["ETAPA"].ToString();
                        reniecModel.MANZANA = dr["MANZANA"] == null || dr["MANZANA"].ToString() == "" ? "" : dr["MANZANA"].ToString();
                        reniecModel.LOTE = dr["LOTE"] == null || dr["LOTE"].ToString() == "" ? "" : dr["LOTE"].ToString();
                        reniecModel.PREFIJOBLOCKCHALET = dr["PREFIJOBLOCKCHALET"] == null || dr["PREFIJOBLOCKCHALET"].ToString() == "" ? "" : dr["PREFIJOBLOCKCHALET"].ToString();
                        reniecModel.PREFIJODPTOPISOINTERIOR = dr["PREFIJODPTOPISOINTERIOR"] == null || dr["PREFIJODPTOPISOINTERIOR"].ToString() == "" ? "" : dr["PREFIJODPTOPISOINTERIOR"].ToString();
                        reniecModel.PREFIJOURBCONDRESID = dr["PREFIJOURBCONDRESID"] == null || dr["PREFIJOURBCONDRESID"].ToString() == "" ? "" : dr["PREFIJOURBCONDRESID"].ToString();
                        reniecModel.RESERVADO = dr["RESERVADO"] == null || dr["RESERVADO"].ToString() == "" ? "" : dr["RESERVADO"].ToString();
                        reniecModel.CODUBIGEODEPARTAMENTODOMICILIO = dr["CODUBIGEODEPARTAMENTODOMICILIO"] == null || dr["CODUBIGEODEPARTAMENTODOMICILIO"].ToString() == "" ? "" : dr["CODUBIGEODEPARTAMENTODOMICILIO"].ToString();
                        reniecModel.CODUBIGEODEPARTAMENTONACI = dr["CODUBIGEODEPARTAMENTONACI"] == null || dr["CODUBIGEODEPARTAMENTONACI"].ToString() == "" ? "" : dr["CODUBIGEODEPARTAMENTONACI"].ToString();
                        reniecModel.CODUBIGEOPROVINCIANACI = dr["CODUBIGEOPROVINCIANACI"] == null || dr["CODUBIGEOPROVINCIANACI"].ToString() == "" ? "" : dr["CODUBIGEOPROVINCIANACI"].ToString();
                        reniecModel.FOTO = dr["FOTO"].ToString() == null || dr["FOTO"].ToString() == "" ? "" : dr["FOTO"].ToString(); 
                        reniecModel.FIRMA = dr["FIRMA"].ToString() == null || dr["FIRMA"].ToString() == "" ? "" : dr["FIRMA"].ToString();
                        ElistClientReniec.Add(reniecModel);
                        //reniecModel.FOTO
                        // var length = dr["FOTO"]; //== null || dr["FOTO"].ToString() == "" ? "" : dr["FOTO"].ToString();
                        //var length2 = length.Length;
                        //reniecModel.FIRMA = (string) dr.GetOracleClob(48).Value; //dr["FIRMA"] == null || dr["FIRMA"].ToString() == "" ? "" : dr["FIRMA"].ToString();
                    }
                   

                }
                result.P_NCODE = "0";
                result.Data = ElistClientReniec;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ResponseSistemaViewModel InsertarSistema(string P_SSISTEMA, string P_NIDDOC_TYPE, string P_SIDDOC)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_ACT_CLIENT_OTRO_SISTEMA";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseSistemaViewModel result = new ResponseSistemaViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, P_SSISTEMA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, P_SIDDOC, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SCOD_CLIENT = new OracleParameter("P_SCOD_CLIENT", OracleDbType.Varchar2, result.P_SCOD_CLIENT, ParameterDirection.Output);
                OracleParameter P_SURL_SISTEMA = new OracleParameter("P_SURL_SISTEMA", OracleDbType.Varchar2, result.P_SURL_SISTEMA, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_SCOD_CLIENT.Size = 4000;
                P_SURL_SISTEMA.Size = 4000;
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_SCOD_CLIENT);
                parameter.Add(P_SURL_SISTEMA);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_SCOD_CLIENT = P_SCOD_CLIENT.Value.ToString();
                result.P_SURL_SISTEMA = P_SURL_SISTEMA.Value.ToString();
                result.P_NCODE = Convert.ToInt64(P_NCODE.Value.ToString());
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public ResponseViewModel ValidarCliente(ClientBindingModel request)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_VAL_CLIENTE";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ListViewErrores> ElistErrores = new List<ListViewErrores>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, request.P_SFIRSTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, request.P_SLASTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, request.P_SLASTNAME2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, request.P_SLEGALNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Varchar2, request.P_SSEXCLIEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINCAPACITY", OracleDbType.Varchar2, request.P_NINCAPACITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINCAP_COD", OracleDbType.Varchar2, request.P_NINCAP_COD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, request.P_DBIRTHDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DINCAPACITY", OracleDbType.Varchar2, request.P_DINCAPACITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NHEALTH_ORG", OracleDbType.Varchar2, request.P_NHEALTH_ORG, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDEATHDAT", OracleDbType.Varchar2, request.P_DDEATHDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DWEDD", OracleDbType.Varchar2, request.P_DWEDD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SACCOUNT_IN", OracleDbType.Varchar2, request.P_SACCOUNT_IN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINVOICING", OracleDbType.Varchar2, request.P_NINVOICING, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSPECIALITY", OracleDbType.Varchar2, request.P_NSPECIALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIVILSTA", OracleDbType.Varchar2, request.P_NCIVILSTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DAPROBDATE", OracleDbType.Varchar2, request.P_DAPROBDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKADE", OracleDbType.Varchar2, request.P_SBLOCKADE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLASS", OracleDbType.Varchar2, request.P_NCLASS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDRIVERDAT", OracleDbType.Varchar2, request.P_DDRIVERDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NHEIGHT", OracleDbType.Varchar2, request.P_NHEIGHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NHOUSE_TYPE", OracleDbType.Varchar2, request.P_NHOUSE_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLICENSE", OracleDbType.Varchar2, request.P_SLICENSE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNOTENUM", OracleDbType.Varchar2, request.P_NNOTENUM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NQ_CARS", OracleDbType.Varchar2, request.P_NQ_CARS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NQ_CHILD", OracleDbType.Varchar2, request.P_NQ_CHILD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRATE", OracleDbType.Varchar2, request.P_NRATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STAX_CODE", OracleDbType.Varchar2, request.P_STAX_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSMOKING", OracleDbType.Varchar2, request.P_SSMOKING, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCUIT", OracleDbType.Varchar2, request.P_SCUIT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTITLE", OracleDbType.Varchar2, request.P_NTITLE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NWEIGHT", OracleDbType.Varchar2, request.P_NWEIGHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAUTO_CHAR", OracleDbType.Varchar2, request.P_SAUTO_CHAR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCREDIT_CARD", OracleDbType.Varchar2, request.P_SCREDIT_CARD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NECONOMIC_L", OracleDbType.Varchar2, request.P_NECONOMIC_L, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NEMPL_QUA", OracleDbType.Varchar2, request.P_NEMPL_QUA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIMAGENUM", OracleDbType.Varchar2, request.P_NIMAGENUM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAREA", OracleDbType.Varchar2, request.P_NAREA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDRIVEXPDAT", OracleDbType.Varchar2, request.P_DDRIVEXPDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPDRIVER", OracleDbType.Varchar2, request.P_NTYPDRIVER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDISABILITY", OracleDbType.Varchar2, request.P_NDISABILITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITDRIV", OracleDbType.Varchar2, request.P_NLIMITDRIV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAFP", OracleDbType.Varchar2, request.P_NAFP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBILL_IND", OracleDbType.Varchar2, request.P_SBILL_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNATIONALITY", OracleDbType.Varchar2, request.P_NNATIONALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDIGIT", OracleDbType.Varchar2, request.P_SDIGIT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DRETIREMENT", OracleDbType.Varchar2, request.P_DRETIREMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DINDEPENDANT", OracleDbType.Varchar2, request.P_DINDEPENDANT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDEPENDANT", OracleDbType.Varchar2, request.P_DDEPENDANT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMAILINGPREF", OracleDbType.Varchar2, request.P_NMAILINGPREF, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLANGUAGE", OracleDbType.Varchar2, request.P_NLANGUAGE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEFTHANDED", OracleDbType.Varchar2, request.P_SLEFTHANDED, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKLAFT", OracleDbType.Varchar2, request.P_SBLOCKLAFT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCLIENT_IND", OracleDbType.Varchar2, request.P_SISCLIENT_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISRENIEC_IND", OracleDbType.Varchar2, request.P_SISRENIEC_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLIENT_SEG", OracleDbType.Varchar2, request.P_NCLIENT_SEG, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOLIZA_ELECT_IND", OracleDbType.Varchar2, request.P_SPOLIZA_ELECT_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TI_DOC_SUSTENT", OracleDbType.Varchar2, request.P_TI_DOC_SUSTENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NU_DOC_SUSTENT", OracleDbType.Varchar2, request.P_NU_DOC_SUSTENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBIG_DEP_NAC", OracleDbType.Varchar2, request.P_COD_UBIG_DEP_NAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBIG_PROV_NAC", OracleDbType.Varchar2, request.P_COD_UBIG_PROV_NAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_UBIG_DIST_NAC", OracleDbType.Varchar2, request.P_COD_UBIG_DIST_NAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEPARTAMENTO_NACIMIENTO", OracleDbType.Varchar2, request.P_DEPARTAMENTO_NACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PROVINCIA_NACIMIENTO", OracleDbType.Varchar2, request.P_PROVINCIA_NACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DISTRITO_NACIMIENTO", OracleDbType.Varchar2, request.P_DISTRITO_NACIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRE_PADRE", OracleDbType.Varchar2, request.P_NOMBRE_PADRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRE_MADRE", OracleDbType.Varchar2, request.P_NOMBRE_MADRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_INSC", OracleDbType.Varchar2, request.P_FECHA_INSC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_EXPEDICION", OracleDbType.Varchar2, request.P_FECHA_EXPEDICION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_CONSTANCIA_VOTACION", OracleDbType.Varchar2, request.P_CONSTANCIA_VOTACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APELLIDO_CASADA", OracleDbType.Varchar2, request.P_APELLIDO_CASADA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDIG_VERIFICACION", OracleDbType.Varchar2, request.P_SDIG_VERIFICACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPROTEG_DATOS_IND", OracleDbType.Varchar2, request.P_SPROTEG_DATOS_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_CUSPP", OracleDbType.Varchar2, request.P_COD_CUSPP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SGRADO_INSTRUCCION", OracleDbType.Varchar2, request.P_SGRADO_INSTRUCCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FOTO_RENIEC", OracleDbType.Varchar2, request.P_FOTO_RENIEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FIRMA_RENIEC", OracleDbType.Varchar2, request.P_FIRMA_RENIEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCLIENT_CRITICO", OracleDbType.Varchar2, request.P_SISCLIENT_CRITICO, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_SISCLIENT_INTERME", OracleDbType.Varchar2, request.P_SISCLIENT_CORREDOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCLIENT_CONTRA", OracleDbType.Varchar2, request.P_SISCLIENT_CONTRATANTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCORREDOR_IND", OracleDbType.Varchar2, request.P_NIND_CORREDOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNAME_COMERCIAL", OracleDbType.Varchar2, request.P_SNAME_COMERCIAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCANT_TRABAJADORES", OracleDbType.Varchar2, request.P_NCANT_TRABAJADORES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCANT_USUARIOS", OracleDbType.Varchar2, request.P_NNUMERO_USUARIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_SBS", OracleDbType.Varchar2, request.P_SCOD_SBS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCONSE_CLAUS_IND", OracleDbType.Varchar2, request.P_SIND_CONSENTI_CLAUS, ParameterDirection.Input));
                parameter.Add(new OracleParameter(" P_NID_SECTOR_EMPRESA", OracleDbType.Varchar2, request.P_NID_SECTOR_EMPRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_AREA_RESPO", OracleDbType.Varchar2, request.P_NID_AREA_RESPONSABLE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFECHA_RECEP_FACTU", OracleDbType.Varchar2, request.P_DFECHA_RECEP_FACTU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SVENTACONSORCIO_IND", OracleDbType.Varchar2, request.P_SIND_VENTA_COMERCIAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPARTIDA_ELECTRONICA", OracleDbType.Varchar2, request.P_SPARTIDA_ELECTRONICA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBAJAMAIL_IND", OracleDbType.Varchar2, request.P_SBAJAMAIL_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFE_ANIVERSARIO", OracleDbType.Varchar2, request.P_DFE_ANIVERSARIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLIDER_CONSORCIO", OracleDbType.Varchar2, request.P_SLIDER_CONSORCIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCLIENT_GBD", OracleDbType.Varchar2, request.P_SISCLIENT_GBD, ParameterDirection.Input));

                //parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, client.P_CodAplicacion, ParameterDirection.Input));

                //OUTPUT

                OracleParameter P_TABLA = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ElistErrores, ParameterDirection.Output);
                OracleParameter P_CAMPO = new OracleParameter("P_CAMPO", OracleDbType.Varchar2, result.P_SMESSAGE,ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                P_CAMPO.Size = 4000;
                parameter.Add(P_TABLA);
                parameter.Add(P_CAMPO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ElistErrores = dr.ReadRowsList<ListViewErrores>();
                    
                }
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                result.EListErrores = ElistErrores;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public ResponseViewModel ValidarTelefono(PhoneBindingModel data, ClientBindingModel client)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_VAL_TELEFONO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();
            List<ListViewErrores> ElistErrores = new List<ListViewErrores>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, data.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, client.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, client.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NKEYPHONES", OracleDbType.Varchar2, data.P_NKEYPHONES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBESTTIMETOCALL", OracleDbType.Varchar2, data.P_NBESTTIMETOCALL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAREA_CODE", OracleDbType.Varchar2, data.P_NAREA_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, data.P_SPHONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NORDER", OracleDbType.Varchar2, data.P_NORDER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NEXTENS1", OracleDbType.Varchar2, data.P_NEXTENS1, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NEXTENS2", OracleDbType.Varchar2, data.P_NEXTENS2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPHONE_TYPE", OracleDbType.Varchar2, data.P_NPHONE_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, client.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOUNTRY_CODE", OracleDbType.Varchar2, data.P_NCOUNTRY_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SASSOCADDR", OracleDbType.Varchar2, data.P_SASSOCADDR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SKEYADDRESS", OracleDbType.Varchar2, data.P_SKEYADDRESS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, client.P_CodAplicacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_TABLA = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ElistErrores, ParameterDirection.Output);
                OracleParameter P_CAMPO = new OracleParameter("P_CAMPO", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);
                P_CAMPO.Size = 4000;
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_TABLA);
                parameter.Add(P_CAMPO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ElistErrores = dr.ReadRowsList<ListViewErrores>();

                }
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                result.EListErrores = ElistErrores;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public ResponseViewModel ValidarCiiu(CiiuBindingModel data, ClientBindingModel client)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_VAL_CIIU";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, data.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, client.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, client.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCIIU", OracleDbType.Varchar2, data.P_SCIIU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, client.P_NUSERCODE, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, client.P_CodAplicacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public ResponseViewModel ValidarCorreo(EmailBindingModel data, ClientBindingModel client)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_VAL_CORREO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();
            List<ListViewErrores> ElistErrores = new List<ListViewErrores>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, data.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRECTYPE", OracleDbType.Varchar2, data.P_SRECTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, client.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, client.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, data.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, client.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SKEYADDRESS", OracleDbType.Varchar2, data.P_SKEYADDRESS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, client.P_CodAplicacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_TABLA = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ElistErrores, ParameterDirection.Output);
                OracleParameter P_CAMPO = new OracleParameter("P_CAMPO", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_CAMPO.Size = 4000;
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_TABLA);
                parameter.Add(P_CAMPO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ElistErrores = dr.ReadRowsList<ListViewErrores>();

                }
                
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                result.EListErrores = ElistErrores;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public ResponseViewModel ValidarDireccion(AddressBindingModel data, ClientBindingModel client)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_VAL_DIRECCION";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();
            List<ListViewErrores> ElistErrores = new List<ListViewErrores>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, data.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRECOWNER", OracleDbType.Varchar2, data.P_NRECOWNER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRECTYPE", OracleDbType.Varchar2, data.P_SRECTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTREET", OracleDbType.Varchar2, data.P_SSTREET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SZONE", OracleDbType.Varchar2, data.P_SZONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, client.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, client.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, data.P_SCERTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, data.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_SECOND", OracleDbType.Varchar2, data.P_NLAT_SECOND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_SECOND", OracleDbType.Varchar2, data.P_NLON_SECOND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_COORD", OracleDbType.Varchar2, data.P_NLAT_COORD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_COORD", OracleDbType.Varchar2, data.P_NLON_COORD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCONTRAT", OracleDbType.Varchar2, data.P_NCONTRAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_CARDIN", OracleDbType.Varchar2, data.P_NLAT_CARDIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_MINUTE", OracleDbType.Varchar2, data.P_NLAT_MINUTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_CARDIN", OracleDbType.Varchar2, data.P_NLON_CARDIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_MINUTE", OracleDbType.Varchar2, data.P_NLON_MINUTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Varchar2, data.P_NCERTIF, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLAIM", OracleDbType.Varchar2, data.P_NCLAIM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, data.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NZIP_CODE", OracleDbType.Varchar2, data.P_NZIP_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_GRADE", OracleDbType.Varchar2, data.P_NLAT_GRADE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOUNTRY", OracleDbType.Varchar2, data.P_NCOUNTRY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_GRADE", OracleDbType.Varchar2, data.P_NLON_GRADE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, client.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBK_AGENCY", OracleDbType.Varchar2, data.P_NBK_AGENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBANK_CODE", OracleDbType.Varchar2, data.P_NBANK_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, data.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOFFICE", OracleDbType.Varchar2, data.P_NOFFICE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPROVINCE", OracleDbType.Varchar2, data.P_NPROVINCE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, data.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLOCAL", OracleDbType.Varchar2, data.P_NLOCAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBUILD", OracleDbType.Varchar2, data.P_SBUILD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Varchar2, data.P_NMUNICIPALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLOOR", OracleDbType.Varchar2, data.P_NFLOOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDEPARTMENT", OracleDbType.Varchar2, data.P_SDEPARTMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOPULATION", OracleDbType.Varchar2, data.P_SPOPULATION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SINFOR", OracleDbType.Varchar2, data.P_SINFOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOBOX", OracleDbType.Varchar2, data.P_SPOBOX, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESCADD", OracleDbType.Varchar2, data.P_SDESCADD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOD_AGREE", OracleDbType.Varchar2, data.P_NCOD_AGREE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREFERENCE", OracleDbType.Varchar2, data.P_SREFERENCE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_DIRE", OracleDbType.Varchar2, data.P_STI_DIRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_DIRECCION", OracleDbType.Varchar2, data.P_SNOM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_DIRECCION", OracleDbType.Varchar2, data.P_SNUM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_BLOCKCHALET", OracleDbType.Varchar2, data.P_STI_BLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKCHALET", OracleDbType.Varchar2, data.P_SBLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_INTERIOR", OracleDbType.Varchar2, data.P_STI_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_INTERIOR", OracleDbType.Varchar2, data.P_SNUM_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_CJHT", OracleDbType.Varchar2, data.P_STI_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_CJHT", OracleDbType.Varchar2, data.P_SNOM_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SETAPA", OracleDbType.Varchar2, data.P_SETAPA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMANZANA", OracleDbType.Varchar2, data.P_SMANZANA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLOTE", OracleDbType.Varchar2, data.P_SLOTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREFERENCIA", OracleDbType.Varchar2, data.P_SREFERENCIA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_DEP_UBI_DOM", OracleDbType.Varchar2, data.P_SCOD_DEP_UBI_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_PRO_UBI_DOM", OracleDbType.Varchar2, data.P_SCOD_PRO_UBI_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_DIS_UBI_DOM", OracleDbType.Varchar2, data.P_SCOD_DIS_UBI_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDES_DEP_DOM", OracleDbType.Varchar2, data.P_SDES_DEP_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDES_PRO_DOM", OracleDbType.Varchar2, data.P_SDES_PRO_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDES_DIS_DOM", OracleDbType.Varchar2, data.P_SDES_DIS_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISRENIEC_IND", OracleDbType.Varchar2, client.P_SISRENIEC_IND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SKEYADDRESS", OracleDbType.Varchar2, data.P_SKEYADDRESS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, client.P_CodAplicacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_TABLA = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ElistErrores, ParameterDirection.Output);
                OracleParameter P_CAMPO = new OracleParameter("P_CAMPO", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                P_CAMPO.Size = 4000;

                parameter.Add(P_TABLA);
                parameter.Add(P_CAMPO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ElistErrores = dr.ReadRowsList<ListViewErrores>();

                }
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                result.EListErrores = ElistErrores;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public ResponseViewModel ValidarContacto(ContactBindingModel data, ClientBindingModel client)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_VAL_CONTACTO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, data.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRECOWNER", OracleDbType.Varchar2, data.P_NRECOWNER, ParameterDirection.Input));
                
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE_CL", OracleDbType.Varchar2, client.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC_CL", OracleDbType.Varchar2, client.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCONT", OracleDbType.Varchar2, data.P_NIDCONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIPCONT", OracleDbType.Varchar2, data.P_NTIPCONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, data.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, data.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOMBRES", OracleDbType.Varchar2, data.P_SNOMBRES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPEPAT", OracleDbType.Varchar2, data.P_SAPEPAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPEMAT", OracleDbType.Varchar2, data.P_SAPEMAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, data.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, data.P_SPHONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNAMEAREA", OracleDbType.Varchar2, data.P_SNAMEAREA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNAMEPOSITION", OracleDbType.Varchar2, data.P_SNAMEPOSITION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, client.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, client.P_CodAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE1", OracleDbType.Varchar2, data.P_SPHONE1, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NEXTENS", OracleDbType.Varchar2, data.P_NEXTENS, ParameterDirection.Input));
                
                

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        public ResponseViewModel InsertarErrores(ErrorValBindingModel data)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_ERROR_CARGA_MASIVA";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SNOPROCESO", OracleDbType.Varchar2, data.P_SNOPROCESO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNUMREG", OracleDbType.Int64, data.P_NNUMREG, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFILENAME", OracleDbType.Varchar2, data.P_SFILENAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESERROR", OracleDbType.Varchar2, data.P_SDESERROR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLUMNA", OracleDbType.Varchar2, data.P_SCOLUMNA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int64, data.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, data.P_SIDDOC, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ResponseViewModel InsertarReniec(ReniecBindingModel data)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_VALIDA_CARGA_RENIEC";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SNOPROCESO", OracleDbType.Varchar2, data.P_SNOPROCESO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNUMREG", OracleDbType.Int64, data.P_NNUMREG, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFILENAME", OracleDbType.Varchar2, data.P_SFILENAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int64, data.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, data.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, data.P_SFIRSTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, data.P_SLASTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, data.P_SLASTNAME2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Varchar2, data.P_SSEXCLIEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIVILSTA", OracleDbType.Int64, data.P_NCIVILSTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, data.P_DBIRTHDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_DIRE", OracleDbType.Varchar2, data.P_STI_DIRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_DIRECCION", OracleDbType.Varchar2, data.P_SNOM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_DIRECCION", OracleDbType.Varchar2, data.P_SNUM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_BLOCKCHALET", OracleDbType.Varchar2, data.P_STI_BLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKCHALET", OracleDbType.Varchar2, data.P_SBLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_INTERIOR", OracleDbType.Varchar2, data.P_STI_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_INTERIOR", OracleDbType.Varchar2, data.P_SNUM_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_CJHT", OracleDbType.Varchar2, data.P_STI_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_CJHT", OracleDbType.Varchar2, data.P_SNOM_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SETAPA", OracleDbType.Varchar2, data.P_SETAPA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMANZANA", OracleDbType.Varchar2, data.P_SMANZANA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLOTE", OracleDbType.Varchar2, data.P_SLOTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREFERENCIA", OracleDbType.Varchar2, data.P_SREFERENCIA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Varchar2, data.P_NMUNICIPALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME_R", OracleDbType.Varchar2, data.P_SFIRSTNAME_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME_R", OracleDbType.Varchar2, data.P_SLASTNAME_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2_R", OracleDbType.Varchar2, data.P_SLASTNAME2_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSEXCLIEN_R", OracleDbType.Varchar2, data.P_SSEXCLIEN_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIVILSTA_R", OracleDbType.Int64, data.P_NCIVILSTA_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DBIRTHDAT_R", OracleDbType.Varchar2, data.P_DBIRTHDAT_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_DIRE_R", OracleDbType.Varchar2, data.P_STI_DIRE_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_DIRECCION_R", OracleDbType.Varchar2, data.P_SNOM_DIRECCION_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_DIRECCION_R", OracleDbType.Varchar2, data.P_SNUM_DIRECCION_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_BLOCKCHALET_R", OracleDbType.Varchar2, data.P_STI_BLOCKCHALET_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKCHALET_R", OracleDbType.Varchar2, data.P_SBLOCKCHALET_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_INTERIOR_R", OracleDbType.Varchar2, data.P_STI_INTERIOR_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_INTERIOR_R", OracleDbType.Varchar2, data.P_SNUM_INTERIOR_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_CJHT_R", OracleDbType.Varchar2, data.P_STI_CJHT_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_CJHT_R", OracleDbType.Varchar2, data.P_SNOM_CJHT_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SETAPA_R", OracleDbType.Varchar2, data.P_SETAPA_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMANZANA_R", OracleDbType.Varchar2, data.P_SMANZANA_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLOTE_R", OracleDbType.Varchar2, data.P_SLOTE_R, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.P_NUSERCODE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ResponseViewModel InsertarExitosos(ExitoValBindingModel data)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_EXITO_CARGA_MASIVA";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SNOPROCESO", OracleDbType.Varchar2, data.P_SNOPROCESO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNUMREG", OracleDbType.Int64, data.P_NNUMREG, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFILENAME", OracleDbType.Varchar2, data.P_SFILENAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int64, data.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, data.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, data.P_SFIRSTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, data.P_SLASTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, data.P_SLASTNAME2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, data.P_SLEGALNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Varchar2, data.P_SSEXCLIEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIVILSTA", OracleDbType.Int64, data.P_NCIVILSTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, data.P_DBIRTHDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_DIRE", OracleDbType.Varchar2, data.P_STI_DIRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_DIRECCION", OracleDbType.Varchar2, data.P_SNOM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_DIRECCION", OracleDbType.Varchar2, data.P_SNUM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_BLOCKCHALET", OracleDbType.Varchar2, data.P_STI_BLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKCHALET", OracleDbType.Varchar2, data.P_SBLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_DIRE", OracleDbType.Varchar2, data.P_TIPO_DIRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_INTERIOR", OracleDbType.Varchar2, data.P_STI_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_INTERIOR", OracleDbType.Varchar2, data.P_SNUM_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_CJHT", OracleDbType.Varchar2, data.P_STI_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_CJHT", OracleDbType.Varchar2, data.P_SNOM_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SETAPA", OracleDbType.Varchar2, data.P_SETAPA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMANZANA", OracleDbType.Varchar2, data.P_SMANZANA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLOTE", OracleDbType.Varchar2, data.P_SLOTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREFERENCIA", OracleDbType.Varchar2, data.P_SREFERENCIA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Int64, data.P_NMUNICIPALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPHONE_TYPE", OracleDbType.Int64, data.P_NPHONE_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, data.P_SPHONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_EMAIL", OracleDbType.Varchar2, data.P_TIPO_EMAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, data.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data.P_SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.P_NUSERCODE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public List<ExitoViewModel> ListarExitoso(ReportBindingModel request)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SPS_LIST_TRAMA_OK";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ExitoViewModel>  ListExitosos = new List<ExitoViewModel>();


            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SNOPROCESO", OracleDbType.Varchar2, request.P_SNOPROCESO, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ListExitosos, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListExitosos = dr.ReadRowsList<ExitoViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListExitosos;
        }
        public List<ErrorViewModel> ListarError(ReportBindingModel request)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SPS_LIST_TRAMA_ERROR";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ErrorViewModel> ListErrores = new List<ErrorViewModel>();


            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SNOPROCESO", OracleDbType.Varchar2, request.P_SNOPROCESO, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ListErrores, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListErrores = dr.ReadRowsList<ErrorViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListErrores;
        }
        public List<ReniecViewModel> ListarReniec(ReportBindingModel request)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SPS_LIST_TRAMA_VAL_RENIEC";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ReniecViewModel> ListReniec = new List<ReniecViewModel>();


            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SNOPROCESO", OracleDbType.Varchar2, request.P_SNOPROCESO, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ListReniec, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListReniec = dr.ReadRowsList<ReniecViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListReniec;
        }

 public ResponseViewModel InsertarInfoBancaria(ClientBindingModel request,InfoBancariaBindingModel request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_INS_CTA_BANCARIA";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, request2.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCUENTA_BANCARIA", OracleDbType.Varchar2, request2.P_SCUENTA_BANCARIA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_BANCO", OracleDbType.Int16, request2.P_NIDBANCO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SID_DOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_TIPO_CUENTA", OracleDbType.Int16, request2.P_NID_TIPO_CUENTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_MONEDA", OracleDbType.Int16, request2.P_NID_MONEDA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUMERO_CUENTA", OracleDbType.Varchar2, request2.P_SNUMERO_CUENTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUMERO_CUENTA_INTER", OracleDbType.Varchar2, request2.P_SNUMERO_CUENTA_INTER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUMERO_DETRACCION", OracleDbType.Varchar2, request2.P_SNUMERO_DETRACCION, ParameterDirection.Input));
                    
                parameter.Add(new OracleParameter("P_DEFECDATE", OracleDbType.Varchar2, "", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCTA_BANCARIA", OracleDbType.Varchar2, request2.P_NIDCTA_BANCARIA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NROW", OracleDbType.Varchar2, request2.P_NROW, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ResponseViewModel InsertarArchivoAdjuntos(ClientBindingModel request,DocumentosBindingModel request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_INS_DOC_ADJUNTO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, request2.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SID_DOC", OracleDbType.Varchar2, request.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ID_TIPO_DOC_ADJUNTO", OracleDbType.Varchar2, request2.P_NID_TIPO_DOC_ADJUNTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFORMAT_FILE", OracleDbType.Varchar2, request2.P_SFORMAT_FILE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNAME_FILE", OracleDbType.Varchar2, request2.P_SNAME_FILE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NROW", OracleDbType.Varchar2, request2.P_NROW, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, request.P_CodAplicacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public ResponseViewModel InsertarClienteFoto(string sclient, string P_SIDDOC, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = "PKG_BDU_CLIENTE.SP_INS_CLIENTE_FOTO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseViewModel result = new ResponseViewModel();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, sclient , ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, P_SIDDOC, ParameterDirection.Input));
                                
                //OUTPUT
                OracleParameter P_NIDCM = new OracleParameter("P_NIDCM", OracleDbType.Int32, result.P_NIDCM, ParameterDirection.Output);
                parameter.Add(P_NIDCM);
                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NIDCM = Convert.ToInt32(P_NIDCM.Value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}
