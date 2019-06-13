using System;
namespace HodlWallet2.Core.Utils
{
    public static class HodlConstants
    {
        // Precio API
        public const string HostUrl = "https://precio.bitstop.co";
        public static readonly string[] CurrencyCodes = { "USD" };
        public const string BtcUnit = "1 BTC = ";
        public const string SatByteUnit = " sat/byte";

        // Colors
        public const string SyncGradientStartHex = "#DAAB28";
        public const string GrayTextTintHex = "#A3A8AD";

        // Localized Strings (TODO in MvvmCross)
        public const string IsAvailable = "Available to spend";
        public const string SentAmount = "Sent BTC ";
        public const string ReceivedAmount = "Received BTC ";
        public const string SentReceivedNull = "Send and Receive is NULL";

        //Temporary Values
        public const string Memo = "In Progress";
    }
}
