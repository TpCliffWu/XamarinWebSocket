using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang.Reflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Droid.DependencyService;
using TestApp.Interface;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceWifiService))]
namespace TestApp.Droid.DependencyService
{
    public class DeviceWifiService : IDeviceWifiService
    {
        public string GetSSID()
        {

            WifiManager wifiManager = (WifiManager)(Android.App.Application.Context.GetSystemService(Context.WifiService));
     
            if (wifiManager != null && !string.IsNullOrEmpty(wifiManager.ConnectionInfo.SSID))
            {
                var ssid = wifiManager.ConnectionInfo.SSID;
                ssid = ssid.Replace("\"", "");
                ssid = ssid.Replace("\\", "");
                return ssid;
            }
            else
            {
                return "WiFiManager is NULL";
            }

        }
    }
}