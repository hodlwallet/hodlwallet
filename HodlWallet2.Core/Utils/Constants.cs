﻿using System;
namespace HodlWallet2.Core.Utils
{
    public static class Constants
    {
        // Precio API
        public const string PRECIO_HOST_URL = "https://precio.bitstop.co";
        public static readonly string[] CURRENCY_CODES = { "USD" };
        public const string BTC_UNIT_LABEL = "1 BTC = {0:C}";
        public const string BTC_UNIT_LABEL_TMP = "1 BTC = 1 BTC";
        public const string SAT_PER_BYTE_UNIT_LABEL = "{0} sat/byte";
        public const int PRECIO_TIMER_INTERVAL = 5;

        // Colors
        public const string SYNC_GRADIENT_START_COLOR_HEX = "#DAAB28";
        public const string GRAY_TEXT_TINT_COLOR_HEX = "#A3A8AD";

        // Localized Strings (TODO in MvvmCross)
        public const string IS_AVAILABLE = "Available to spend";
        public const string SENT_AMOUNT = "Sent BTC {0}";
        public const string RECEIVE_AMOUNT = "Received BTC {0}";
        public const string SENT_RECEIVED_NULL = "Send and Receive is NULL";
        public const string SYNC_DATE_LABEL = "{0}, Block: {1}";
        public const string SHARE_TEXT_INTENT_TITLE = "Share via";

        // Temporary Values
        public const string USE_ADDRESS_FROM_CLIPBOARD =
            "The Bitcoin address '{0} is in your clipboard, would you like to send to that address?";
        public const string MEMO_LABEL = "In Progress";

        // Android Service Values
        public const string TEXT_PLAIN_INTENT_TYPE = "text/plain";
        public const string IMAGE_PNG_INTENT_TYPE = "image/png";
		public const string IMAGE_PNG_ADDRESS_NAME = ".png";
    }
}
