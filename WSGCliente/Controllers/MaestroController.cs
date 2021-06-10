using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSGCliente.Core;

namespace WSGCliente.Controllers
{
    [RoutePrefix("Api/Maestro")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MaestroController : ApiController
    {
        [Route("GetTipoCuenta")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTipoCuenta()
        {
            var Response = await new MaestroCore().GetTipoCuenta();
            return Content(HttpStatusCode.OK, Response);
        }
        [Route("GetTipoArchivo")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTipoArchivo()
        {
            var Response = await new MaestroCore().GetTipoArchivo();
            return Content(HttpStatusCode.OK,Response);
        }
        [Route("GetSectorEmpresa")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSectorEmpresa()
        {
            var Response = await new MaestroCore().GetSectorEmpresa();
            return Content(HttpStatusCode.OK, Response);
        }
        [Route("GetTipoMoneda")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTipoMoneda()
        {
            var Response = await new MaestroCore().GetTipoMoneda();
            return Content(HttpStatusCode.OK, Response);
        }
        [Route("GetBanco")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBanco()
        {
            var Response = await new MaestroCore().GetBanco();
            return Content(HttpStatusCode.OK, Response);
        }
        [Route("GetAreaResponsable")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAreaResponsable()
        {
            var Response = await new MaestroCore().GetAreaResponsable();
            return Content(HttpStatusCode.OK, Response);
        }



    }
}
