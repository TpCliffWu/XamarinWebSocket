using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestApp.ViewModels
{
    public class SocketSendPageViewModel : BindableBase
    {
        public SocketSendPageViewModel(IPageDialogService dialogService)
        {
            _dialogService = dialogService;
            ConnectIP = "192.168.40.201"; // 預設值 

            SendCommand = new DelegateCommand(async () =>
            {
                SendMsg();
            });
        }

        IPageDialogService _dialogService;
        public DelegateCommand SendCommand { get; set; }

        public int ListenPort = 11000;

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

        public TcpSocketClient _client;

        public CancellationTokenSource _canceller;

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
