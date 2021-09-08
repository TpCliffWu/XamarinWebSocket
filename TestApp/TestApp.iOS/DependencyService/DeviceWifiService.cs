

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using NetworkExtension;
using SystemConfiguration;
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
            manager.AuthorizationChanged += (sender, args) =>
            {

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

        public async Task<List<string>> GetSSID()
        {
            var list = new List<string>();
            try
            {

                // GetLocationConsent();

                string[] supportedInterfaces;
                StatusCode status;
                if ((status = CaptiveNetwork.TryGetSupportedInterfaces(out supportedInterfaces)) != StatusCode.OK)
                {
                    return list;
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
                            list.Add(info[CaptiveNetwork.NetworkInfoKeySSID].ToString());
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                return list;
            }

        }

        public void WifiConnect(NetworkModel network)
        {
            throw new NotImplementedException();
        }
    }
}