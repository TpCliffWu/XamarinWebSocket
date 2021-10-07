using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketXamarin.ViewModels
{
    public class ClientPageViewModel : ViewModelBase
    {
        public ClientPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Client";
        }
    }
}
