using GoogleVisionBarCodeScanner;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace TestApp
{
    public partial class QRCodeScanPage : ContentPage
    {
        public QRCodeScanPage()
        {
            InitializeComponent();
            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(BarcodeFormats.Itf | BarcodeFormats.QRCode);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }
        protected override void OnDisappearing()
        {
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);
            base.OnDisappearing();
        }
    }
}
