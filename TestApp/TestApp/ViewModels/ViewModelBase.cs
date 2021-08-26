using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _networkStatus;
        public string NetworkStatus
        {
            get { return _networkStatus; }
            set { SetProperty(ref _networkStatus, value); }
        }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

     

        public async Task NetworkStatusUpdate()
        {
            NetworkStatus = "";
            var ipList = NetworkTools.GetDNSIP();
            if (ipList.Any())
            {
                NetworkStatus += $"DNS IP: \n";
                ipList.ForEach(ip =>
                {
                    NetworkStatus += $"\t{ip}\n";
                });
            }

            var wifiList = await NetworkTools.GetWifiSSID();
            if (wifiList.Any())
            {
                NetworkStatus += "WiFi SSID:\n";
                wifiList.ForEach(ssid =>
                {
                    NetworkStatus += $"\t{ssid}\n";
                });
            }
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
