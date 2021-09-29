using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XamarinWiFi.ViewModels
{
    public class ServerPageViewModel : ViewModelBase
    {
        INavigationService navigationService;
        public ServerPageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
    }
}
