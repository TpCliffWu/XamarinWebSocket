using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace TestApp
{
   public class WebSocketMessage
    {
        public string Text { get; set; }
        public DateTime MessagDateTime { get; set; }

        public bool IsIncoming => UserId != DeviceInfo.Name;

        public string Name { get; set; }
        public string UserId { get; set; }
    }
}
