using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSGCliente.DataAccess;
using WSGCliente.Entities.ViewModel;
 using Master = WSGCliente.Entities.BindingModel.Intermediarios.Maestras;

namespace WSGCliente.Core
{
    public class MaestroCore
    {
        public async Task<IEnumerable<Master.TipoCuentaBindingModel>> GetTipoCuenta()
        {
            return await new MaestroDataAccess().GetTipoCuenta();
        }
        public async Task<IEnumerable<Master.TipoArchivoBindingModel>> GetTipoArchivo()
        {
            return await new MaestroDataAccess().GetTipoArchivo();
        }
        public async Task<IEnumerable<Master.SectorEmpresaBindingModel>> GetSectorEmpresa()
        {
            return await new MaestroDataAccess().GetSectorEmpresa();
        }
        public async Task<IEnumerable<Master.MonedaBindingModel>> GetTipoMoneda()
        {
            return await new MaestroDataAccess().GetTipoMoneda();
        }
        public async Task<IEnumerable<Master.BancoBindingModel>> GetBanco()
        {
            return await new MaestroDataAccess().GetBanco();
        }
        public async Task<IEnumerable<Master.AreasBindingModel>> GetAreaResponsable()
        {
            return await new MaestroDataAccess().GetAreaResponsable();
        }


    }
}
