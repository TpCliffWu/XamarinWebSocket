using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace XamarinWiFi
{
    public static class NetworkTools
    {
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
