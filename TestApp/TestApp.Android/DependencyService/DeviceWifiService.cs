using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Text.Format;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Droid;
using TestApp.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Android.Net.ConnectivityManager;
using Android.Provider;

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
            }
            return ssids;
        }

        public void WifiConnect(NetworkModel network)
        {
            if (Android.OS.Build.VERSION.SdkInt < BuildVersionCodes.Q)
            {
                AddNetwork(network);
            }
            if (Android.OS.Build.VERSION.SdkInt == BuildVersionCodes.Q)
            {
                WifiConnect();
            }
            if (Android.OS.Build.VERSION.SdkInt > BuildVersionCodes.Q)
            {
                WifiConnect();
            }
        }

        public void AddNetwork(NetworkModel network)
        {
            try
            {
                WifiManager wifiManager = (WifiManager)(ThisAndroid.CurrentContext.GetSystemService(Context.WifiService));

                string networkSSID = $"\"{network.NetworkSSID}\"";
                string networkPass = $"\"{network.NetworkPassword}\"";

                WifiConfiguration wifiConfig = new WifiConfiguration();
                wifiConfig.Ssid = networkSSID;
                wifiConfig.PreSharedKey = networkPass;
                var netId = wifiManager.AddNetwork(wifiConfig);

                wifiManager.Disconnect();
                wifiManager.EnableNetwork(netId, true);
                wifiManager.Reconnect();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }

        public void WifiConnect()
        {
            // 開啟wifi連線panel
            var activity = (MainActivity)ThisAndroid.CurrentContext;
            activity.StartActivityForResult(new Intent(Settings.Panel.ActionWifi), ThisAndroid.ENABLE_WIFI_REQUEST);

            MessagingCenter.Subscribe<ActivityResultMessage>(this, ActivityResultMessage.Key, (sender) =>
            {
                if (sender.RequestCode == ThisAndroid.ENABLE_WIFI_REQUEST)
                {
                    Console.WriteLine("Connected");
                }
            });
        }
    }

    public class MyNetworkCallback : ConnectivityManager.NetworkCallback
    {
        public override void OnAvailable(Network network)
        {
            base.OnAvailable(network);
            WiFiCenter.ConnectivityManager.BindProcessToNetwork(network);
        }

        public override void OnLost(Network network)
        {
            base.OnLost(network);
            WiFiCenter.ConnectivityManager.BindProcessToNetwork(null);
            WiFiCenter.ConnectivityManager.UnregisterNetworkCallback(this);
        }
    }

    [BroadcastReceiver(Enabled = true)]
    public class MyBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (!intent.Action.Equals(WifiManager.ActionWifiNetworkSuggestionPostConnection))
            {
                return;
            }
        }
    }
}
