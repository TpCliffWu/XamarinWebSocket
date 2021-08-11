using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using static TestApp.App;

namespace TestApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand SendCommand { get; set; }
        public DelegateCommand StartCommand { get; set; }
        IPageDialogService _dialogService;



        public List<ITcpSocketClient> _clients = new List<ITcpSocketClient>();
        public TcpSocketListener _listener;
        public TcpSocketClient _client;
        public CancellationTokenSource _canceller;


        public int ListenPort = 11000;
        private bool _listening;



        private string hostIP;
        public string HostIP
        {
            get { return hostIP; }
            set { SetProperty(ref hostIP, value); }
        }


        private string connectIP;
        public string ConnectIP
        {
            get { return connectIP; }
            set { SetProperty(ref connectIP, value); }
        }

        private string hostButtonText;
        public string HostButtonText
        {
            get { return hostButtonText; }
            set { SetProperty(ref hostButtonText, value); }
        }

        private string _sendMessage;
        public string SendMessage
        {
            get { return _sendMessage; }
            set { SetProperty(ref _sendMessage, value); }
        }

        private string _receiveMessage;
        public string ReceiveMessage
        {
            get { return _receiveMessage; }
            set { SetProperty(ref _receiveMessage, value); }
        }


        public MainPageViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService)
        {

            _dialogService = dialogService;
            Title = "Main Page";

            ConnectIP = "192.168.40.201"; // 預設值 

            HostButtonText = "Start Listener";

            // 取得host ip
            try
            {
                foreach (IPAddress adress in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (!string.IsNullOrWhiteSpace(adress.ToString()))
                    {
                        if (adress.ToString().Contains("192"))
                        {
                            HostIP = adress.ToString();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }

            StartCommand = new DelegateCommand(async () =>
            {
                CreateListener();
            });

            SendCommand = new DelegateCommand(async () =>
            {
                SendMsg();
            });
        }

        /// <summary>
        /// 開啟tcp監聽器
        /// </summary>
        public async void CreateListener()
        {
            if (!_listening)
            {
                // 開啟
                try
                {
                    _listener = new TcpSocketListener();

                    _listener.ConnectionReceived += async (sender, args) =>
                    {
                        var client = args.SocketClient;
                        foreach (var msg in client.ReadStrings(_canceller.Token))
                        {
                            ReceiveMessage += $"{msg.Text}\n";
                        }
                    };

                    
                    await _listener.StartListeningAsync(ListenPort, Global.DefaultCommsInterface);
                    _canceller = new CancellationTokenSource();
                    _listening = true;
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
                    _listening = false;
                    HostButtonText = "Start Listener";
                }
                catch (Exception ex)
                {
                    await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
                }
            }

        }






        /// <summary>
        /// 傳送TCP訊息
        /// </summary>
        public async void SendMsg()
        {
            var address = ConnectIP;
            var port = ListenPort;

            try
            {
                // 傳送訊息
                _client = new TcpSocketClient();
                await _client.ConnectAsync(ConnectIP, ListenPort);
                _canceller = new CancellationTokenSource();
                await _client.WriteStringAsync(SendMessage);

                // 結尾
                var bytes = Encoding.UTF8.GetBytes("<EOF>");
                await _client.WriteStream.WriteAsync(bytes, 0, bytes.Length);
                await _client.WriteStream.FlushAsync();

                _canceller.Cancel();
                await _client.DisconnectAsync();

            }
            catch (Exception ex)
            {
                await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
            }
        }
    }
}
