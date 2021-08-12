using Prism;
using Prism.Ioc;
using Sockets.Plugin;
using TestApp.ViewModels;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace TestApp
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();

            containerRegistry.RegisterForNavigation<SocketListenerPage, SocketListenerPageViewModel>();
            containerRegistry.RegisterForNavigation<SocketSendPage, SocketSendPageViewModel>();
        }

        public static class Global
        {
            public static CommsInterface DefaultCommsInterface { get; set; }
        }
    }
}
