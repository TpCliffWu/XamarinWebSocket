using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using static TestApp.App;

namespace TestApp.ViewModels
{
    public class SocketListenerPageViewModel : BindableBase
    {
        public SocketListenerPageViewModel(IPageDialogService dialogService)
        {
            _dialogService = dialogService;

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
        }

        IPageDialogService _dialogService;
        private string hostButtonText;
        public string HostButtonText
        {
            get { return hostButtonText; }
            set { SetProperty(ref hostButtonText, value); }
        }

        private string hostIP;
        public string HostIP
        {
            get { return hostIP; }
            set { SetProperty(ref hostIP, value); }
        }

        public DelegateCommand StartCommand { get; set; }

        private bool _listening;
        public TcpSocketListener _listener;
        public int ListenPort = 11000;
        public CancellationTokenSource _canceller;

        private string _receiveMessage;
        public string ReceiveMessage
        {
            get { return _receiveMessage; }
            set { SetProperty(ref _receiveMessage, value); }
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
    }
}
