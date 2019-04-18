using System;
using System.Globalization;
using MvvmCross.Converters;
using MvvmCross.Forms.Converters;

namespace HodlWallet2.Converters
{
    public class IndexToStringConverter : MvxFormsValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            //TODO: Localize string in converter.
            return $"{value + 1} of 12";
        }
    }
}