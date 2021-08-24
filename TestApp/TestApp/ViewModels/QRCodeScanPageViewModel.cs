using GoogleVisionBarCodeScanner;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace TestApp.ViewModels
{
    public class QRCodeScanPageViewModel : ViewModelBase
    {
        public IPageDialogService _dialogService;

        public ICommand CameraViewOnDetectedCommand { get; set; }
        public QRCodeScanPageViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;

            CameraViewOnDetectedCommand = new DelegateCommand<List<BarcodeResult>>(async (scanResult) =>
            {
                var qrcode = scanResult.FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(qrcode.DisplayValue))
                {
                    try
                    {
                  
                        var isValid = JsonConvert.DeserializeObject<NetworkModel>(qrcode.DisplayValue);
                        var navParameters = new NavigationParameters { { "QRCode", qrcode.DisplayValue } };
                        await NavigationService.GoBackAsync(navParameters);
                    }
                    catch (Exception ex)
                    {
                        await _dialogService.DisplayActionSheetAsync("", $"Deserialize Erro", "OK");
                    }
                }

            });
        }

    }
}
