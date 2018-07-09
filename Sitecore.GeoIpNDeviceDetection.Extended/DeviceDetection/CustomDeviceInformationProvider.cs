using DeviceDetectorNET;
using Sitecore.CES.DeviceDetection;
using Sitecore.Diagnostics;
using Sitecore.Threading.Locks;
using System.Threading;

namespace Sitecore.GeoIpNDeviceDetection.Extended
{
    public class CustomDeviceInformationProvider : DeviceInformationProviderBase
    {
        private readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();

        public override bool IsEnabled => true;

        protected override string DoGetExtendedProperty(string userAgent, string propertyName)
        {
            return string.Empty;
        }

        public override DeviceInformation GetDeviceInformation(string userAgent)
        {
            Assert.ArgumentNotNull(userAgent, "userAgent");
            return base.GetDeviceInformation(userAgent);
        }

        protected override DeviceInformation DoGetDeviceInformation(string userAgent)
        {
            Assert.ArgumentNotNull(userAgent, "userAgent");

            using (new ReadScope(_readerWriterLockSlim))
            {
                var dd = new DeviceDetector(userAgent);
                dd.Parse();

                var browserMatchResult = dd.GetBrowserClient().Match;
                DeviceType deviceType = DeviceType.Computer;
                if (dd.IsMobile())
                    deviceType = DeviceType.MobilePhone;

                DeviceInformation deviceInformation = new DeviceInformation
                {
                    Browser = browserMatchResult?.Name ?? "desktop",
                    DeviceIsSmartphone = dd.IsMobile(),
                    DeviceModelName = dd.GetDeviceName(),
                    DeviceType = deviceType,
                    DeviceVendor = dd.GetModel()
                };

                return deviceInformation;
            }
        }

    }
}