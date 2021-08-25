using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

using static TestApp.App;

namespace TestApp.ViewModels
{
    public class SocketListenerPageViewModel : ViewModelBase
    {
        public List<ITcpSocketClient> _tcpClients = new List<ITcpSocketClient>();

        public IPageDialogService _dialogService;
        public TcpSocketListener _listener;
        public CancellationTokenSource _canceller;
        public int ListenPort = 11000;

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand ResponseCommand { get; set; }

        public DelegateCommand GetIPInfoCommand { get; set; }
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


        private string _wifiInfo = "";
        public string WifiInfo
        {
            get { return _wifiInfo; }
            set { SetProperty(ref _wifiInfo, value); }
        }

        private ObservableCollection<TCPMessage> _receiveMessageList = new ObservableCollection<TCPMessage>();
        public ObservableCollection<TCPMessage> ReceiveMessageList
        {
            get { return _receiveMessageList; }
            set
            {
                SetProperty(ref _receiveMessageList, value);
            }
        }

        private ObservableCollection<TcpSocketClientWithUser> tcpSocketClientWithUsers = new ObservableCollection<TcpSocketClientWithUser>();
        public ObservableCollection<TcpSocketClientWithUser> TcpSocketClientWithUsers
        {
            get { return tcpSocketClientWithUsers; }
            set { SetProperty(ref tcpSocketClientWithUsers, value); }
        }

        private int _pickerSelectedIndex = 0;
        public int PickerSelectedIndex
        {
            get { return _pickerSelectedIndex; }
            set { SetProperty(ref _pickerSelectedIndex, value); }
        }

        public SocketListenerPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;

            HostButtonText = "Start Listener";
            // 取得host ip
            GetIPInfo();

            var tcp = new TcpSocketClientWithUser();
            tcp.DeviceName = "ALL";
            TcpSocketClientWithUsers.Add(tcp);


            GetIPInfoCommand = new DelegateCommand(async () =>
            {
                await GetIPWIFIInfo();
            });

            StartCommand = new DelegateCommand(async () =>
            {
                // 開啟WIFI分享
                if (await LocationPermission())
                {
                    await SetupHotspot();

                    await GetIPInfo();
                    // 開啟站台
                    CreateListener();
                }
            });

            ResponseCommand = new DelegateCommand(async () =>
            {
                Response();
            });
        }

        public async Task GetIPWIFIInfo()
        {
            //await GetWifiInfo();
            await GetIPInfo();
        }

        public async Task GetIPInfo()
        {
            try
            {
                IPInfo = "";
                //IPInfo += $"Real IP: \n";
                //// 真實IP
                //string pubIp = new System.Net.WebClient().DownloadString("https://api.ipify.org");
                //   IPInfo += $"{pubIp} \n";

                IPInfo += $"DNS IP: \n";
                //foreach (IPAddress adress in Dns.GetHostAddresses(Dns.GetHostName()))
                //{
                //    if (!string.IsNullOrWhiteSpace(adress.ToString()))
                //    {
                //        var _ip = $"{adress.MapToIPv4()}";

                //            IPInfo += $"{adress.MapToIPv4()}\n";

                //    }
                //}
                foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                var ipAddress = addrInfo.Address;

                                // use ipAddress as needed ...
                                IPInfo += $"{ipAddress.MapToIPv4()}\n";
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
        }

        //public async Task GetWifiInfo()
        //{

        //    // WIFI 資訊
        //    // 權限
        //    try
        //    {

        //        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        //        if (status != PermissionStatus.Granted)
        //        {
        //            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        //        }

        //        if (status != PermissionStatus.Granted)
        //        {
        //            return;
        //        }


        //        WifiInfo = "";
        //        WifiInfo += $"WIFI SSID: \n";

        //        var list = await DependencyService.Get<IDeviceWifiService>().GetSSID();

        //        list.ForEach(wifissid =>
        //        {
        //            if (!string.IsNullOrWhiteSpace(wifissid))
        //            {
        //                WifiInfo += $"{wifissid}\n";
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
        //    }
        //}


        public async Task<bool> LocationPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
            return status == PermissionStatus.Granted;
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

                       Device.BeginInvokeOnMainThread(() => _tcpClients.Add(client));

                       Task.Factory.StartNew(() =>
                       {
                           foreach (var msg in client.ReadStrings(_canceller.Token))
                           {
                               ReceiveMessage += $"{msg.Text} {msg.DetailText} \n";
                               ReceiveMessageList.Add(msg);

                               Device.BeginInvokeOnMainThread(() => AddNewTCPUser(client, msg));
                           }

                           Device.BeginInvokeOnMainThread(() => _tcpClients.Remove(client));
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
            try
            {
                if (_tcpClients.Any())
                {
                    var msg = new TCPMessage();
                    msg.Text = ResponseMessage;
                    msg.DeviceName = $"{DeviceInfo.Manufacturer}/{DeviceInfo.Name}";

                    if (PickerSelectedIndex == 0)
                    {
                        var sendTasks = _tcpClients.Select(c => SocketExtensions.WriteStringAsync(c, msg)).ToList();
                        await Task.WhenAll(sendTasks);
                    }
                    else
                    {
                        var _client = TcpSocketClientWithUsers[PickerSelectedIndex];
                        var sendTasks = SocketExtensions.WriteStringAsync(_client.tcpSocketClient, msg);
                        await Task.WhenAll(sendTasks);
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
            }
        }

        /// <summary>
        /// 下拉選單加入回傳對象
        /// </summary>
        /// <param name="tcpSocketClient"></param>
        /// <param name="msg"></param>
        public void AddNewTCPUser(ITcpSocketClient tcpSocketClient, TCPMessage msg)
        {

            if (TcpSocketClientWithUsers.Where(o => o.DeviceName == msg.DeviceName && o.IPAddress == msg.IPAddress).Any())
            {
                return;
            }

            var tcp = new TcpSocketClientWithUser();
            tcp.tcpSocketClient = tcpSocketClient;
            tcp.DeviceName = msg.DeviceName;
            tcp.IPAddress = msg.IPAddress;
            TcpSocketClientWithUsers.Add(tcp);
        }

        public async Task SetupHotspot()
        {
            var network = await DependencyService.Get<IHotspotService>().HotspotSetup();
            WifiInfo = JsonConvert.SerializeObject(network);
        }

        private string _hostButtonText;
        private string _receiveMessage;
        private string _iPInfo;
    }
}
