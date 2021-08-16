using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TestApp.Interface;
using Xamarin.Essentials;
using Xamarin.Forms;

using static TestApp.App;

namespace TestApp.ViewModels
{
    public class SocketListenerPageViewModel : BindableBase
    {
        public List<ITcpSocketClient> _clients = new List<ITcpSocketClient>();
        public IPageDialogService _dialogService;
        public TcpSocketListener _listener;
        public CancellationTokenSource _canceller;
        public int ListenPort = 11000;

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand ResponseCommand { get; set; }

        public DelegateCommand GetWifiInfoCommand { get; set; }
        public string HostButtonText
        {
            get { return _hostButtonText; }
            set { SetProperty(ref _hostButtonText, value); }
        }

        public string IPInfo
        {
            get { return _iPInfo; }
            set { SetProperty(ref _iPInfo, value); }
        }
        public string ReceiveMessage
        {
            get { return _receiveMessage; }
            set { SetProperty(ref _receiveMessage, value); }
        }

        private string _responseMessage;
        public string ResponseMessage
        {
            get { return _responseMessage; }
            set { SetProperty(ref _responseMessage, value); }
        }

        private bool _listening;
        public bool Listening
        {
            get { return _listening; }
            set { SetProperty(ref _listening, value); }
        }


        private string _wifiInfo;
        public string WifiInfo
        {
            get { return _wifiInfo; }
            set { SetProperty(ref _wifiInfo, value); }
        }

        public SocketListenerPageViewModel(IPageDialogService dialogService)
        {
            _dialogService = dialogService;

            HostButtonText = "Start Listener";
            // 取得host ip
            try
            {
                IPInfo += $"Real IP: \n";
                // 真實IP
                string pubIp = new System.Net.WebClient().DownloadString("https://api.ipify.org");

                IPInfo += $"{pubIp} \n";
                IPInfo += $"DNS IP: \n";
                foreach (IPAddress adress in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (!string.IsNullOrWhiteSpace(adress.ToString()))
                    {
                        IPInfo += $"{adress.MapToIPv4()}\n";
                    }
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
            GetWifiInfoCommand = new DelegateCommand(async () =>
            {
                await GetWifiInfo();
            });

            StartCommand = new DelegateCommand(async () =>
            {
                CreateListener();
            });

            ResponseCommand = new DelegateCommand(async () =>
            {
                Response();
            });
        }

        public async Task GetWifiInfo()
        {

            // WIFI 資訊
            // 權限
            try
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    }
                }
                WifiInfo = "";
                WifiInfo += $"WIFI SSID: \n";
                WifiInfo += DependencyService.Get<IDeviceWifiService>().GetSSID();
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
            }
        }




        /// <summary>
        /// 開啟tcp監聽器
        /// </summary>
        public async void CreateListener()
        {
            if (!Listening)
            {
                // 開啟
                try
                {
                    _listener = new TcpSocketListener();

                    _listener.ConnectionReceived += async (sender, args) =>
                   {
                       var client = args.SocketClient;

                       Device.BeginInvokeOnMainThread(() => _clients.Add(client));

                       Task.Factory.StartNew(() =>
                       {
                           foreach (var msg in client.ReadStrings(_canceller.Token))
                           {
                               ReceiveMessage += $"{msg.Text} {msg.DetailText} \n";
                           }

                           Device.BeginInvokeOnMainThread(() => _clients.Remove(client));
                       }, TaskCreationOptions.LongRunning);
                   };


                    await _listener.StartListeningAsync(ListenPort, Global.DefaultCommsInterface);
                    _canceller = new CancellationTokenSource();
                    Listening = true;
                    HostButtonText = "Stop Listener";
                }
                catch (Exception ex)
                {
                    await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
                }
            }
            else
            {
                // 關閉
                try
                {
                    await _listener.StopListeningAsync();
                    _canceller.Cancel();
                    Listening = false;
                    HostButtonText = "Start Listener";
                }
                catch (Exception ex)
                {
                    await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
                }
            }

        }

        // 回傳
        public async void Response()
        {
            if (_clients.Any())
            {
                var sendTasks = _clients.Select(c => SocketExtensions.WriteStringAsync(c, ResponseMessage)).ToList();
                await Task.WhenAll(sendTasks);
            }
        }


        private string _hostButtonText;
        private string _receiveMessage;
        private string _iPInfo;
    }
}
