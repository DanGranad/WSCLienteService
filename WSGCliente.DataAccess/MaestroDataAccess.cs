using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSGCliente.Util;
using Master = WSGCliente.Entities.BindingModel.Intermediarios.Maestras;
namespace WSGCliente.DataAccess
{
    public class MaestroDataAccess : ConnectionBase
    {
        private static string sPackageName = "PKG_BDU_CLIENTE";

        public async Task<IEnumerable<Master.TipoCuentaBindingModel>> GetTipoCuenta()
        {
           
            IEnumerable<Master.TipoCuentaBindingModel> ListaTipoCuenta;
            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>
                {
                    new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output)
                };
                using (OracleDataReader dr =  (OracleDataReader) await this.ExecuteByStoredProcedureVTAsync($"{sPackageName}.SP_SEL_TYPE_CUENTA_BANC", parameter))
                {
                    ListaTipoCuenta = dr.ReadRowsList<Master.TipoCuentaBindingModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListaTipoCuenta;
        }

        public async Task<IEnumerable<Master.TipoArchivoBindingModel>> GetTipoArchivo()
        {

            IEnumerable<Master.TipoArchivoBindingModel> ListaTipoArchivo;
            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>
                {
                    new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output)
                };
                using (OracleDataReader dr = (OracleDataReader)await this.ExecuteByStoredProcedureVTAsync($"{sPackageName}.SP_SEL_TYPE_ARCHIVO_ADJUNTO", parameter))
                {
                    ListaTipoArchivo = dr.ReadRowsList<Master.TipoArchivoBindingModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListaTipoArchivo;
        }

        public async Task<IEnumerable<Master.SectorEmpresaBindingModel>> GetSectorEmpresa()
        {

            IEnumerable<Master.SectorEmpresaBindingModel> ListaSectorEmpresa;
            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>
                {
                    new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output)
                };
                using (OracleDataReader dr = (OracleDataReader)await this.ExecuteByStoredProcedureVTAsync($"{sPackageName}.SP_SEL_TYPE_SECTOR_EMPRESA", parameter))
                {
                    ListaSectorEmpresa = dr.ReadRowsList<Master.SectorEmpresaBindingModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListaSectorEmpresa;
        }

        public async Task<IEnumerable<Master.MonedaBindingModel>> GetTipoMoneda()
        {

            IEnumerable<Master.MonedaBindingModel> ListaTipoMoneda;
            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>
                {
                    new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output)
                };
                using (OracleDataReader dr = (OracleDataReader)await this.ExecuteByStoredProcedureVTAsync($"{sPackageName}.SP_SEL_TYPE_TIPO_MONEDA", parameter))
                {
                    ListaTipoMoneda = dr.ReadRowsList<Master.MonedaBindingModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListaTipoMoneda;
        }

        public async Task<IEnumerable<Master.BancoBindingModel>> GetBanco()
        {

            IEnumerable<Master.BancoBindingModel> ListaBanco;
            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>
                {
                    new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output)
                };
                using (OracleDataReader dr = (OracleDataReader)await this.ExecuteByStoredProcedureVTAsync($"{sPackageName}.SP_SEL_LIST_BANCO", parameter))
                {
                    ListaBanco = dr.ReadRowsList<Master.BancoBindingModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListaBanco;
        }

        public async Task<IEnumerable<Master.AreasBindingModel>> GetAreaResponsable()
        {

            IEnumerable<Master.AreasBindingModel> ListAreaResponsable;
            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>
                {
                    new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output)
                };
                using (OracleDataReader dr = (OracleDataReader)await this.ExecuteByStoredProcedureVTAsync($"{sPackageName}.SP_SEL_LIST_AREA_RESPONSABLE", parameter))
                {
                    ListAreaResponsable = dr.ReadRowsList<Master.AreasBindingModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListAreaResponsable;
        }

    }
}
