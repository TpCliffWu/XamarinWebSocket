using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Text.Format;
using Android.Views;
using Android.Widget;
using Java.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(HotspotService))]
namespace TestApp.Droid
{
    public class HotspotService : IHotspotService
    {
        public void HotspotClose()
        {
            if (WiFiCenter.Reservation != null)
            {
                WiFiCenter.Reservation.Close();
            }
        }

        public async Task<NetworkModel> HotspotSetup()
        {
            WifiManager wifiManager = (WifiManager)(ThisAndroid.CurrentContext.GetSystemService(Context.WifiService));

            if (WiFiCenter.Reservation == null)
            {
                wifiManager.StartLocalOnlyHotspot(new CustomHotspotCallback(), new Handler());
            }

            await Task.Delay(3000);

            var config = WiFiCenter.Reservation.WifiConfiguration;

            var network = new NetworkModel();
            network.NetworkSSID = config.Ssid;
            network.NetworkPassword = config.PreSharedKey;
            network.NetworkIPAddresses = new List<NetworkIPAddress>();

            var enumNetworkInterfaces = NetworkInterface.NetworkInterfaces;
            while (enumNetworkInterfaces.HasMoreElements)
            {
                var networkInterface = (NetworkInterface)enumNetworkInterfaces.NextElement();
                var enumInetAddress = networkInterface.InetAddresses;
                while (enumInetAddress.HasMoreElements)
                {
                    var inetAddress = (InetAddress)enumInetAddress.NextElement();

                    if (inetAddress.IsSiteLocalAddress)
                    {
                        var net = new NetworkIPAddress();
                        net.Name = networkInterface.DisplayName;
                        net.IPAddress = inetAddress.HostAddress;
                        network.NetworkIPAddresses.Add(net);
                    }
                }
            }
            return network;
        }
    }



    public class CustomHotspotCallback : WifiManager.LocalOnlyHotspotCallback
    {
        public override void OnStarted(WifiManager.LocalOnlyHotspotReservation reservation)
        {
            base.OnStarted(reservation);
            WiFiCenter.Reservation = reservation;
        }

        public override void OnStopped()
        {
            base.OnStopped();
            WiFiCenter.Reservation = null;
        }

        public override void OnFailed([GeneratedEnum] LocalOnlyHotspotCallbackErrorCode reason)
        {
            base.OnFailed(reason);
        }
    }
}