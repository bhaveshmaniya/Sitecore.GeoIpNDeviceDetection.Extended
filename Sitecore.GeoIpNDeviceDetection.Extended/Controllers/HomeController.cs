using Sitecore.Analytics.Lookups;
using Sitecore.Analytics.Model;
using Sitecore.CES.DeviceDetection;
using Sitecore.Diagnostics;
using Sitecore.GeoIpNDeviceDetection.Extended.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sitecore.GeoIpNDeviceDetection.Extended.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage WhoAmI()
        {
            HttpResponseMessage response;

            try
            {
                var userIpAddress = GeoIpService.GetUserIpAddress();

                WhoIsInformation whoami = LookupManager.GetInformationByIp(userIpAddress.ToString());

                var userAgent = Request.Headers.UserAgent.ToString();
                var di = DeviceDetectionManager.GetDeviceInformation(userAgent);

                response = Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Ip = userIpAddress.ToString(),
                    whoami.Country,
                    whoami.City,
                    whoami.Region,
                    whoami.PostalCode,
                    Latitude = whoami.Latitude ?? 0,
                    Longitude = whoami.Longitude ?? 0,
                    Device = new
                    {
                        di.DeviceIsSmartphone,
                        di.DeviceModelName,
                        di.Browser,
                        DeviceType = Enum.GetName(typeof(DeviceType), di.DeviceType),
                        di.DeviceVendor
                    }
                });

                return response;
            }
            catch (Exception ex)
            {
                Log.Error($"HomeController.WhoAmI(): Error while getting user's GeoIP and Device Detection information.", ex, this);

                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Oops, it looks like something went wrong. Please try again.");
                return response;
            }

        }
    }
}
