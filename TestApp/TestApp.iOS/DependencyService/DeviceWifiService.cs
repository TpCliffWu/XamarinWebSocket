

using System;
using CoreLocation;
using Foundation;
using NetworkExtension;
using SystemConfiguration;
using TestApp.Interface;
using TestApp.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceWifiService))]
namespace TestApp.iOS
{
    public class DeviceWifiService : IDeviceWifiService
    {

        private void GetLocationConsent()
        {
            var manager = new CLLocationManager();
            manager.AuthorizationChanged += (sender, args) => {

                Console.WriteLine("Authorization changed to: {0}", args.Status);
            };
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                manager.RequestWhenInUseAuthorization();
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                manager.AllowsBackgroundLocationUpdates = true;
            }

        }

        public string GetSSID()
        {
            try {

               // GetLocationConsent();

                string[] supportedInterfaces;
                StatusCode status;
                if ((status = CaptiveNetwork.TryGetSupportedInterfaces(out supportedInterfaces)) != StatusCode.OK)
                {
                    return "NO INFO";
                }
                else
                {
                    foreach (var item in supportedInterfaces)
                    {
                        NSDictionary info;
                        status = CaptiveNetwork.TryCopyCurrentNetworkInfo(item, out info);
                        if (status != StatusCode.OK)
                        {
                            continue;
                        }

                        if (info != null)
                        {
                            return info[CaptiveNetwork.NetworkInfoKeySSID].ToString();
                        }
                    }
                }
                return "NO INFO";
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex}");
                return "INFO ERROR";
            }

        }
    }
}