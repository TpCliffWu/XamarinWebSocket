using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamarinWiFi.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";

            this.ToServerCommand = new DelegateCommand(async()=>
            {
                await NavigationService.NavigateAsync("ServerPage");
            });

            this.ToClientCommand = new DelegateCommand(async () =>
            {
                await NavigationService.NavigateAsync("ClientPage");
            });
        }

        public DelegateCommand ToServerCommand { get; set; }

        public DelegateCommand ToClientCommand { get; set; }
    }
}
