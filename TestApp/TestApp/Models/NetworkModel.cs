using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp
{
    public class NetworkModel
    {
        /// <summary>
        /// 無線基地台名稱
        /// </summary>
        public string NetworkName { get; set; }

        /// <summary>
        /// 無線基地台密碼
        /// </summary>
        public string NetworkPassword { get; set; }

        /// <summary>
        /// 基地台IP
        /// </summary>
        public List<NetworkIPAddress> NetworkIPAddresses { get; set; }
    }

    public class NetworkIPAddress
    {
        public string Name { get; set; }
        public string IPAddress { get; set; }
    }
}
