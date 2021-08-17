using Sockets.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp
{
    public class TcpSocketClientWithUser
    {
        public ITcpSocketClient tcpSocketClient { get; set; }

        public string DeviceName { get; set; }

        public string IPAddress { get; set; }
    }
}
