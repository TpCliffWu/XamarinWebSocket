using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XamarinWiFi
{
    public interface IDeviceWifiService
    {
        Task<List<string>> GetSSID();
    }
}
