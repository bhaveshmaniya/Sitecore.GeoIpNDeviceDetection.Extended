using Sitecore.Analytics.Configuration;
using Sitecore.Analytics.Lookups;
using Sitecore.Analytics.Model;
using Sitecore.Diagnostics;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Helpers;

namespace Sitecore.GeoIpNDeviceDetection.Extended.Services
{
    public class GeoIpService : LookupProviderBase
    {
        public GeoIpService()
        { }

        public WhoIsInformation GetGeoIp()
        {
            return GetGeoIp(GetUserIpAddress());
        }

        public WhoIsInformation GetGeoIp(IPAddress ipAddress)
        {
            return GetInformationByIp(ipAddress.ToString());
        }

        public override WhoIsInformation GetInformationByIp(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                var ipaddress = GetUserIpAddress();
                if (ipaddress == null)
                    return null;
                ip = ipaddress.ToString();
            }
            string url = string.Format(Configuration.Settings.IpStackApiUrl, ip, Configuration.Settings.IpStackApiAccessKey);
            string jsonData = GetDataFromExternalService(url);
            if (string.IsNullOrEmpty(jsonData))
                return null;

            dynamic data = Json.Decode(jsonData);
            WhoIsInformation info = new WhoIsInformation
            {
                Country = data.country_code,
                City = data.city,
                Latitude = (double?)data.latitude,
                Longitude = (double?)data.longitude,
                Region = data.region_name,
                PostalCode = data.zip
            };

            return info;
        }

        public static IPAddress GetUserIpAddress()
        {
            IPAddress ip = null;

            try
            {
                if (!string.IsNullOrEmpty(AnalyticsSettings.ForwardedRequestHttpHeader))
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request.Headers[AnalyticsSettings.ForwardedRequestHttpHeader]))
                    {
                        // Forwarded Headers could be comma separated
                        string[] ipValues = HttpContext.Current.Request.Headers[AnalyticsSettings.ForwardedRequestHttpHeader].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        if (ipValues.Length > 0)
                        {
                            IPAddress.TryParse(ipValues[0], out ip);
                        }
                    }
                    else
                    {
                        IPAddress.TryParse(HttpContext.Current.Request.UserHostAddress, out ip);
                    }
                }
                else
                {
                    IPAddress.TryParse(HttpContext.Current.Request.UserHostAddress, out ip);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"GeoIpService.GetUserIpAddress(): Error while getting user Ip address.", ex);
            }

            return ip;
        }

        #region Private Method(s)
        private string GetDataFromExternalService(string uri)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"GeoIpService.GetDataFromExternalService(): Error while getting data from external service. URL: {uri}", ex);
            }

            return string.Empty;
        }

        #endregion
    }
}