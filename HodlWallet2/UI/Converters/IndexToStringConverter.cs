using System;
using System.Globalization;

using Xamarin.Forms;

namespace HodlWallet2.UI.Converters
{
    public class IndexToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{(int)value + 1} of 12";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}