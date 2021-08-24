using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Text.Format;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceWifiService))]
namespace TestApp.Droid
{
    public class DeviceWifiService : IDeviceWifiService
    {
        public WifiManager.LocalOnlyHotspotReservation Reservation { get; set; }

        public async Task<List<string>> GetSSID()
        {
            var ssids = new List<string>();

            WifiManager wifiManager = (WifiManager)(ThisAndroid.CurrentContext.GetSystemService(Context.WifiService));

            if (wifiManager != null && !string.IsNullOrEmpty(wifiManager.ConnectionInfo.SSID))
            {
                var ssid = wifiManager.ConnectionInfo.SSID;
                ssid = ssid.Replace("\"", "");
                ssid = ssid.Replace("\\", "");
                ssids.Add(ssid);


                //var bssid = wifiManager.ConnectionInfo.BSSID;
                //ssid = ssid.Replace("\"", "");
                //ssid = ssid.Replace("\\", "");
                //ssids.Add(bssid);
            }
            return ssids;
        }

        public string WifiConnect(NetworkModel network)
        {
            string networkSSID = "\"" + network.NetworkName + "\"";
            string networkPass = "\"" + network.NetworkPassword + "\"";
            //string networkSSID = network.NetworkName;
            //string networkPass = network.NetworkPassword;

            WifiConfiguration conf = new WifiConfiguration();
            conf.Ssid = networkSSID;
            conf.PreSharedKey = networkPass;

            //conf.WepKeys[0] = networkPass;
            //conf.WepTxKeyIndex = 0;
            //conf.AllowedKeyManagement.Set((int)KeyManagementType.None);
            //conf.AllowedGroupCiphers.Set((int)Android.Net.Wifi.GroupCipherType.Wep40);

            WifiManager wifiManager = (WifiManager)(ThisAndroid.CurrentContext.GetSystemService(Context.WifiService));
            wifiManager.AddNetwork(conf);

            var newWorks = wifiManager.ConfiguredNetworks;
            foreach (var newWork in newWorks)
            {
                if (newWork.Ssid != null && newWork.Ssid.Contains(networkSSID))
                {
                    wifiManager.Disconnect();
                    wifiManager.EnableNetwork(newWork.NetworkId, true);
                    wifiManager.Reconnect();

                    return networkSSID;
                }
            }

            return "";

        }
    }
}