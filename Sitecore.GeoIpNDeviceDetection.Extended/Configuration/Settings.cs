namespace Sitecore.GeoIpNDeviceDetection.Extended.Configuration
{
    public static class Settings
    {
        public static string IpStackApiUrl
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting("GeoIp.IpStackApi.Url", string.Empty);
            }
        }

        public static string IpStackApiAccessKey
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting("GeoIp.IpStackApi.AccessKey", string.Empty);
            }
        }
    }
}