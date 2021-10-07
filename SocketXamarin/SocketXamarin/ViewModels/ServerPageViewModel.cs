using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketXamarin.ViewModels
{
    public class ServerPageViewModel : ViewModelBase
    {
        public ServerPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Server";
        }
    }
}
