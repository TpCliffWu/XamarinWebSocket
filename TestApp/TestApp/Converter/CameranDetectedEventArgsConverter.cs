using GoogleVisionBarCodeScanner;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace TestApp
{
    public class CameranDetectedEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cameraDetectedEventArgs = value as OnDetectedEventArg;
            if (cameraDetectedEventArgs == null)
            {
                throw new ArgumentException("Expected value to be of type OnDetectedEventArg", nameof(value));
            }
            // 回傳  List<BarcodeResult> 
            return cameraDetectedEventArgs.BarcodeResults;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
