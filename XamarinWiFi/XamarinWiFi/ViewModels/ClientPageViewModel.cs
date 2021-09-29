using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace XamarinWiFi.ViewModels
{
    public class ClientPageViewModel : ViewModelBase
    {
        readonly ClientWebSocket client;
        readonly CancellationTokenSource cts;
        readonly string username;


        private string messageText;
        public string MessageText
        {
            get { return messageText; }
            set { SetProperty(ref messageText, value); }
        }

        private ObservableCollection<Message> messages;
        public ObservableCollection<Message> Messages
        {
            get { return messages; }
            set { SetProperty(ref messages, value); }
        }

        public DelegateCommand ConnectCommand { get; set; }
        public DelegateCommand SendCommand { get; set; }



        public ClientPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            client = new ClientWebSocket();
            cts = new CancellationTokenSource();
            messages = new ObservableCollection<Message>();
            username = DeviceInfo.Name;

            ConnectCommand = new DelegateCommand(Connect);
            SendCommand = new DelegateCommand(() =>
            {
                SendMessageAsync("Hello");
            });
        }

        private async void Connect()
        {
            await client.ConnectAsync(new Uri("ws://10.0.2.2:5000"), cts.Token);

            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    WebSocketReceiveResult result;
                    var message = new ArraySegment<byte>(new byte[4096]);
                    do
                    {
                        result = await client.ReceiveAsync(message, cts.Token);
                        var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                        string serialisedMessae = Encoding.UTF8.GetString(messageBytes);

                        try
                        {
                            var msg = JsonConvert.DeserializeObject<Message>(serialisedMessae);
                            Messages.Add(msg);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Invalide message format. {ex.Message}");
                        }

                    } while (!result.EndOfMessage);
                }
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        }

        async void SendMessageAsync(string message)
        {
            var msg = new Message
            {
                Name = username,
                MessagDateTime = DateTime.Now,
                Text = message,
                UserId = DeviceInfo.Name
            };

            string serialisedMessage = JsonConvert.SerializeObject(msg);

            var byteMessage = Encoding.UTF8.GetBytes(serialisedMessage);
            var segmnet = new ArraySegment<byte>(byteMessage);

            await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts.Token);
            MessageText = string.Empty;
        }


    }
}
