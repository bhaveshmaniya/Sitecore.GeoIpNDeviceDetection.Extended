using System.Web.Http;
using System.Web.Routing;

namespace Sitecore.GeoIpNDeviceDetection.Extended
{
    public class WebApiConfig
    {
        public void Process(Sitecore.Pipelines.PipelineArgs args)
        {
            var config = GlobalConfiguration.Configuration;
            var mvcRoutes = RouteTable.Routes;

            config.Routes.MapHttpRoute(
               name: "WhoAmI",
               routeTemplate: "api/who-am-i",
               defaults: new { controller = "Home", action = "WhoAmI" }
               );

        }
    }
}