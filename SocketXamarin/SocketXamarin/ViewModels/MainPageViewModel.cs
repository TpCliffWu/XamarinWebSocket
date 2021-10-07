using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketXamarin.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand ToServerPageCommand { get; set; }
        public DelegateCommand ToClientPageCommand { get; set; }
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";

            ToServerPageCommand = new DelegateCommand(ToServerPage);
            ToClientPageCommand = new DelegateCommand(ToClientPage);
        }

        private void ToClientPage()
        {
            this.NavigationService.NavigateAsync("ClientPage");
        }

        private void ToServerPage()
        {
            this.NavigationService.NavigateAsync("ServerPage");
        }
    }
}
