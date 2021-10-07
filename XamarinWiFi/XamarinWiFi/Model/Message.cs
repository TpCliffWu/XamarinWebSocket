using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace XamarinWiFi
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime MessagDateTime { get; set; }

        public bool IsIncoming => UserId != DeviceInfo.Name;

        public string Name { get; set; }
        public string UserId { get; set; }
    }
}
