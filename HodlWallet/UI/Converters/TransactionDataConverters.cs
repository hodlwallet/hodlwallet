//
// TransactionDataConverters.cs
//
// Copyright (c) 2019 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Globalization;

using Liviano.Models;
using Xamarin.Forms;

using HodlWallet.Core.Services;
using HodlWallet.Core.Utils;

namespace HodlWallet.UI.Converters
{
    public class MemoToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return 0;

            return 15;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TxTypeToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (TxType)value switch
            {
                TxType.Partial => "question_mark",
                TxType.Send => "arrow_upward",
                TxType.Receive => "arrow_downward",
                _ => "question_mark",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TxTypeToColorConverter : IValueConverter
    {
        Color FgSuccess => (Color)Application.Current.Resources["FgSuccess"];
        Color FgGreen => (Color)Application.Current.Resources["FgGreen"];
        Color Fg5 => (Color)Application.Current.Resources["Fg5"];


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (TxType)value switch
            {
                TxType.Partial => Fg5,
                TxType.Send => FgSuccess,
                TxType.Receive => FgGreen,
                _ => Fg5,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BitcoinCurrentCurrencyToRowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currencyType = (DisplayCurrencyType)value;

            return currencyType == DisplayCurrencyType.Bitcoin ? 0 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FiatCurrentCurrencyToRowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currencyType = (DisplayCurrencyType)value;

            return currencyType == DisplayCurrencyType.Fiat ? 0 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BitcoinCurrentCurrencyToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currencyType = (DisplayCurrencyType)value;

            return currencyType == DisplayCurrencyType.Bitcoin ?
                Device.GetNamedSize(NamedSize.Small, typeof(Label)) : Device.GetNamedSize(NamedSize.Micro, typeof(Label));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FiatCurrentCurrencyToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currencyType = (DisplayCurrencyType)value;

            return currencyType == DisplayCurrencyType.Fiat ?
                Device.GetNamedSize(NamedSize.Small, typeof(Label)) : Device.GetNamedSize(NamedSize.Micro, typeof(Label));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BitcoinCurrentCurrencyToTextColorConverter : IValueConverter
    {
        Color Fg => (Color)Application.Current.Resources["Fg"];
        Color Fg2 => (Color)Application.Current.Resources["Fg2"];

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currencyType = (DisplayCurrencyType)value;

            return currencyType == DisplayCurrencyType.Bitcoin ? Fg : Fg2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FiatCurrentCurrencyToTextColorConverter : IValueConverter
    {
        Color Fg => (Color)Application.Current.Resources["Fg"];
        Color Fg2 => (Color)Application.Current.Resources["Fg2"];

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currencyType = (DisplayCurrencyType)value;

            return currencyType == DisplayCurrencyType.Fiat ? Fg : Fg2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CreatedAtToDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return "???";

            var creationTime = (DateTimeOffset)value;

            return DateTimeOffsetOperations.ShortDate(creationTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
