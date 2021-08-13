using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestApp.ViewModels
{
    public class SocketSendPageViewModel : BindableBase
    {
        public IPageDialogService _dialogService;
        public DelegateCommand SendCommand { get; set; }

        public DelegateCommand CloseCommand { get; set; }

        public TcpSocketClient _client;

        public CancellationTokenSource _cancelTokenSource;

        public int ListenPort = 11000;

        public ClientWebSocket clientWebSocket;

        private bool _connected;
        public bool Connected
        {
            get { return _connected; }
            set { SetProperty(ref _connected, value); }
        }

        private string _sendMessage;

        public string SendMessage
        {
            get { return _sendMessage; }
            set { SetProperty(ref _sendMessage, value); }
        }
        private string connectIP;
        public string ConnectIP
        {
            get { return connectIP; }
            set { SetProperty(ref connectIP, value); }
        }

        private string _responseMessage;
        public string ResponseMessage
        {
            get { return _responseMessage; }
            set { SetProperty(ref _responseMessage, value); }
        }

        private string _connectText;
        public string ConnectText
        {
            get { return _connectText; }
            set { SetProperty(ref _connectText, value); }
        }


        public SocketSendPageViewModel(IPageDialogService dialogService)
        {
            _dialogService = dialogService;
            ConnectIP = "192.168.40.201"; // 預設值 
            ConnectText = "Connect";

            SendCommand = new DelegateCommand(async () =>
            {
                Connect();
                SendMsg();
            });

            CloseCommand = new DelegateCommand(async () =>
            {
                Close();
            });

        }



        public async void Connect()
        {
            try
            {
                _client = new TcpSocketClient();

                await _client.ConnectAsync(ConnectIP, ListenPort);
                _cancelTokenSource = new CancellationTokenSource();

                Connected = true;
                Task.Factory.StartNew(() =>
                {
                    foreach (var msg in _client.ReadStrings(_cancelTokenSource.Token))
                    {
                        ResponseMessage += $"{msg.Text} {msg.DetailText} \n";
                    }
                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
            }

        }




        /// <summary>
        /// 傳送TCP訊息
        /// </summary>
        public async void SendMsg()
        {
            try
            {
                _client = new TcpSocketClient();

                await _client.ConnectAsync(ConnectIP, ListenPort);
                _cancelTokenSource = new CancellationTokenSource();

                // 傳送訊息
                await _client.WriteStringAsync(SendMessage);
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
            }
        }

        public async void Close()
        {
            try
            {
                if (Connected)
                {
                    // 結尾
                    var bytes = Encoding.UTF8.GetBytes("<EOF>");
                    await _client.WriteStream.WriteAsync(bytes, 0, bytes.Length);
                    await _client.WriteStream.FlushAsync();

                    _cancelTokenSource.Cancel();
                    await _client.DisconnectAsync();
                }
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
            }
        }
    }
}
