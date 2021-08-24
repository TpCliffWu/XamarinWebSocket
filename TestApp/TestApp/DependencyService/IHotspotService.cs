using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public interface IHotspotService
    {
        Task<NetworkModel> HotspotSetup();

        void HotspotClose();
    }
}
