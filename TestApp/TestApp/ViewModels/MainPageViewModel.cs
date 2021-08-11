using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Sockets.Plugin;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace TestApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand SendCommand { get; set; }
        public DelegateCommand StartCommand { get; set; }
        IPageDialogService _dialogService;


        public int ListenPort = 11000;

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



        public MainPageViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService)
        {

            _dialogService = dialogService;
            Title = "Main Page";
            ConnectIP = "192.168.40.201";
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
            catch
            {

            }

            StartCommand = new DelegateCommand(async () =>
            {
                var listener = new TcpSocketListener();

                // when we get connections, read byte-by-byte from the socket's read stream
                listener.ConnectionReceived += async (sender, args) =>
                {
                    var client = args.SocketClient;

                    var bytesRead = -1;
                    var buf = new byte[1];

                    while (bytesRead != 0)
                    {
                        bytesRead = await args.SocketClient.ReadStream.ReadAsync(buf, 0, 1);
                        if (bytesRead > 0)
                        {
                            Debug.Write(buf[0]);
                        }
                    }
                };

                // bind to the listen port across all interfaces
                await listener.StartListeningAsync(ListenPort);
            });

            SendCommand = new DelegateCommand(async () =>
            {
                var address = ConnectIP;
                var port = ListenPort;
                var r = new Random();

                try
                {
                    var client = new TcpSocketClient();
                    await client.ConnectAsync(address, port);

                    // we're connected!
                    for (int i = 0; i < 5; i++)
                    {
                        // write to the 'WriteStream' property of the socket client to send data
                        var nextByte = (byte)r.Next(0, 254);
                        client.WriteStream.WriteByte(nextByte);
                        await client.WriteStream.FlushAsync();

                        // wait a little before sending the next bit of data
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                    }

                    await client.DisconnectAsync();
                }
                catch (Exception ex)
                {
                    await _dialogService.DisplayActionSheetAsync("", $"{ex}", "OK");
                }
            });
        }

        public byte[] AddByteToArray(byte[] bArray, byte newByte)
        {
            var len = bArray == null ? 0 : bArray.Length;

            byte[] newArray = new byte[len + 1];

            bArray.CopyTo(newArray, 1);

            newArray[0] = newByte;
            return newArray;
        }
    }
}
