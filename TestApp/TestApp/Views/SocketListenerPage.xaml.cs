using Xamarin.Forms;

namespace TestApp
{
    public partial class SocketListenerPage : ContentPage
    {
        public SocketListenerPage()
        {
            InitializeComponent();
            var option = qrcode_image.BarcodeOptions;
            option.Width = 400;
            option.Height = 400;
        }
    }
}
