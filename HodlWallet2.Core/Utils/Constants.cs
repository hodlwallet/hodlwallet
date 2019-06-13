using System;
namespace HodlWallet2.Core.Utils
{
    public static class Constants
    {
        // Precio API
        public const string PrecioHostUrl = "https://precio.bitstop.co";
        public static readonly string[] CurrencyCodes = { "USD" };
        public const string BtcUnit = "1 BTC = {0:C}";
        public const string SatByteUnit = "{0} sat/byte";
        public const int PrecioTimerInterval = 5;

        // Colors
        public const string SyncGradientStartHex = "#DAAB28";
        public const string GrayTextTintHex = "#A3A8AD";

        // Localized Strings (TODO in MvvmCross)
        public const string IsAvailable = "Available to spend";
        public const string SentAmount = "Sent BTC {0}";
        public const string ReceivedAmount = "Received BTC {0}";
        public const string SentReceivedNull = "Send and Receive is NULL";
        public const string SyncDate = "{0}, Block: {1}";

        //Temporary Values
        public const string Memo = "In Progress";
    }
}
