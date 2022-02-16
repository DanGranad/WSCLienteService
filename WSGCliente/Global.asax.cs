using System.Net;
using System.Web.Http;
using System.Web.Routing;

namespace WSGCliente
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        }
    }
}
