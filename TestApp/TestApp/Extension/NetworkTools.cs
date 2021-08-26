using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestApp
{
    public static class NetworkTools
    {
        /// <summary>
        /// 取得WIFI SSID
        /// </summary>
        public static async Task<List<string>> GetWifiSSID()
        {
            var list = new List<string>();
            try
            {
                // 檢查權限
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted)
                {
                  return await DependencyService.Get<IDeviceWifiService>().GetSSID();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
            return list;
        }


        public static List<string> GetDNSIP()
        {
            var list = new List<string>();
            try
            {
                foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    //if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    //    netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    //{
                        foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                var ipAddress = addrInfo.Address;

                                // use ipAddress as needed ...
                                list.Add($"{ipAddress.MapToIPv4()}");
                            }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
            return list;
        }
    }
}
