using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestApp.ViewModels
{
    public class SocketSendPageViewModel : ViewModelBase
    {
        public IPageDialogService _dialogService;
        public DelegateCommand SendCommand { get; set; }

        public DelegateCommand CloseCommand { get; set; }

        public DelegateCommand ConnectCommand { get; set; }

        public DelegateCommand GetIPInfoCommand { get; set; }

        public DelegateCommand ScanCommand { get; set; }

        public DelegateCommand WiFiConnectCommand { get; set; }


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

        /// <summary>
        ///  連線的IP
        /// </summary>
        private List<string> _connectIPAddresses = new List<string>();
        public List<string> ConnectIPAddresses
        {
            get { return _connectIPAddresses; }
            set { SetProperty(ref _connectIPAddresses, value); }
        }

        /// <summary>
        /// 選擇的連線picker index
        /// </summary>
        private int _pickerSelectedIndex = 0;
        public int PickerSelectedIndex
        {
            get { return _pickerSelectedIndex; }
            set { SetProperty(ref _pickerSelectedIndex, value); }
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

        private NetworkModel _qrCodeScanResult;
        public NetworkModel QRCodeScanResult
        {
            get { return _qrCodeScanResult; }
            set { SetProperty(ref _qrCodeScanResult, value); }
        }

        private ObservableCollection<TCPMessage> _receiveMessageList = new ObservableCollection<TCPMessage>();
        public ObservableCollection<TCPMessage> ReceiveMessageList
        {
            get { return _receiveMessageList; }
            set { SetProperty(ref _receiveMessageList, value); }
        }

        public SocketSendPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;
            Connected = false;

            ConnectCommand = new DelegateCommand(async () =>
             {
                 Connect();

             });

            SendCommand = new DelegateCommand(async () =>
            {
                if (!Connected)
                {
                    Connect();
                }

                SendMsg();
            });

            CloseCommand = new DelegateCommand(async () =>
            {
                Close();
            });
            GetIPInfoCommand = new DelegateCommand(async () =>
            {
                await base.NetworkStatusUpdate();
            });
            ScanCommand = new DelegateCommand(async () =>
            {
                if (await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission())
                {
                    await NavigationService.NavigateAsync("QRCodeScanPage");
                }
                else
                {
                    await _dialogService.DisplayActionSheetAsync("", $"You have to provide Camera permission", "OK");
                }
            });

            WiFiConnectCommand = new DelegateCommand(async () =>
            {
                await ConnectWiFi();
            });
        }

        public ClientWebSocket webSocketClient;

        public async void WebScocketConnect()
        {
            webSocketClient = new ClientWebSocket();
            _cancelTokenSource = new CancellationTokenSource();

            await webSocketClient.ConnectAsync(new Uri($"ws://{ConnectIP}:{ListenPort}"), _cancelTokenSource.Token);
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        WebSocketReceiveResult result;
                        var message = new ArraySegment<byte>(new byte[4096]);
                        do
                        {
                            result = await webSocketClient.ReceiveAsync(message, _cancelTokenSource.Token);
                            var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                            string serialisedMessae = Encoding.UTF8.GetString(messageBytes);

                            try
                            {
                                var msg = JsonConvert.DeserializeObject<WebSocketMessage>(serialisedMessae);
                                ResponseMessage += $"{msg.Text} {msg.MessagDateTime}\n";
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Invalide message format. {ex.Message}");
                            }

                        } while (!result.EndOfMessage);
                    }
                }, _cancelTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
            }
        }

        public async void WebScocketSend()
        {
            try
            {
                var msg = new WebSocketMessage
                {
                    Name = DeviceInfo.Name,
                    MessagDateTime = DateTime.Now,
                    Text = this.SendMessage,
                    UserId = DeviceInfo.Name
                };

                string serialisedMessage = JsonConvert.SerializeObject(msg);

                var byteMessage = Encoding.UTF8.GetBytes(serialisedMessage);
                var segmnet = new ArraySegment<byte>(byteMessage);

                await webSocketClient.SendAsync(segmnet, WebSocketMessageType.Text, true, _cancelTokenSource.Token);
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
            }
        }

        /// <summary>
        /// 取得掃描結果
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var parameter = parameters.GetValue<string>("QRCode");
            // 掃描結果ip帶入選單中
            if (!string.IsNullOrWhiteSpace(parameter))
            {
                var network = JsonConvert.DeserializeObject<NetworkModel>(parameter);

                QRCodeScanResult = network;

                WifiInfo = $"Server Wi-Fi Name :{network.NetworkSSID} \n";
                WifiInfo += $"Server Wi-Fi 密碼 :{network.NetworkPassword} \n";

                await Clipboard.SetTextAsync(network.NetworkPassword);

                ConnectIPAddresses = network.NetworkIPAddresses.Select(o => o.IPAddress).ToList();
            }
        }

        /// <summary>
        /// 連結Wi-Fi
        /// </summary>
        public async Task ConnectWiFi()
        {
            DependencyService.Get<IDeviceWifiService>().WifiConnect(QRCodeScanResult);

            await base.NetworkStatusUpdate();
        }


        public async void Connect()
        {
            try
            {
                _client = new TcpSocketClient();


                await _client.ConnectAsync(ConnectIPAddresses[PickerSelectedIndex], ListenPort);
                _cancelTokenSource = new CancellationTokenSource();

                Connected = true;
                Task.Factory.StartNew(() =>
                {
                    foreach (var msg in _client.ReadStrings(_cancelTokenSource.Token))
                    {
                        ResponseMessage += $"{msg.Text} {msg.DetailText} \n";
                        ReceiveMessageList.Add(msg);
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

                await _client.ConnectAsync(ConnectIPAddresses[PickerSelectedIndex], ListenPort);
                _cancelTokenSource = new CancellationTokenSource();

                // 傳送訊息
                //   await _client.WriteStringAsync(SendMessage);
                var msg = new TCPMessage();
                msg.Text = SendMessage;
                msg.DeviceName = $"{DeviceInfo.Manufacturer}/{DeviceInfo.Name}";

                await _client.WriteStringAsync(msg);
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
                    Connected = false;
                }
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
            }
        }

        private string _wifiInfo;
        public string WifiInfo
        {
            get { return _wifiInfo; }
            set { SetProperty(ref _wifiInfo, value); }
        }
    }
}
