using System;
using System.Globalization;
using HodlWallet2.Core.Utils;
using Xamarin.Forms;

namespace HodlWallet2.UI.Converters
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

    public class IsSendToStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSend = (bool)value;

            return isSend
                ? Color.FromHex(Constants.SYNC_GRADIENT_START_COLOR_HEX)
                : Color.FromHex(Constants.GRAY_TEXT_TINT_COLOR_HEX);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsSendToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSend = (bool)value;

            return isSend
                ? "sent.png"
                : "received.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CreationTimeToDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var creationTime = (DateTimeOffset)value;

            return DateTimeOffsetOperations.ShortDate(creationTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
