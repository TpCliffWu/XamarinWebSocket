﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public interface IDeviceWifiService
    {
        Task<List<string>> GetSSID();

        string WifiConnect(NetworkModel network);
    }
}
