using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(HotspotService))]
namespace TestApp.iOS
{
    public class HotspotService : IHotspotService

    {
        public void HotspotClose()
        {
            throw new NotImplementedException();
        }

        public Task<NetworkModel> HotspotSetup()
        {
            throw new NotImplementedException();
        }
    }
}