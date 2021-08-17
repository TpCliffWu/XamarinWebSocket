using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp
{
    public class TCPMessage
    {
        // 傳送訊息
        public string Text { get; set; }

        // 傳送裝置
        public string DeviceName { get; set; }



        // 接收到的ip
        public string IPAddress { get; set; }

        // 接收到的時間
        public string MessageDateTime { get; set; }

        public string DetailText { get; set; }
    }
}
