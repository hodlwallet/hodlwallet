//
// Constants.cs
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
using HodlWallet.Core.Models;
using System.Collections.Generic;

namespace HodlWallet.Core.Utils
{
    public static class Constants
    {
        // Precio API
        public const string PRECIO_HOST_URL = "https://precio.bitstop.co";
        public const string PRECIO_WS_HOST_URL = "wss://precio.bitstop.co";
        public static readonly string[] CURRENCY_CODES = { "USD" };
        public static readonly List<CurrencySymbolEntity> CURRENCY_SYMBOLS = new() {
                            new CurrencySymbolEntity { Code = "USD", Symbol = "$" },
                            new CurrencySymbolEntity { Code = "AUD", Symbol = "AU$" },
                            new CurrencySymbolEntity { Code = "ALL", Symbol = "L" },
                            new CurrencySymbolEntity { Code = "DZD", Symbol = "دج" },
                            new CurrencySymbolEntity { Code = "ARS", Symbol = "$" },
                            new CurrencySymbolEntity { Code = "AFN", Symbol = "؋" },
                            new CurrencySymbolEntity { Code = "AOA", Symbol = "Kz" },
                            new CurrencySymbolEntity { Code = "AMD", Symbol = "֏" },
                            new CurrencySymbolEntity { Code = "AZN", Symbol = "₼" }
                            };

        public const string BTC_UNIT_LABEL = "1 BTC ≈ {0:C}";
        public const string BTC_UNIT_LABEL_TMP = "";
        public const string SAT_PER_BYTE_UNIT_LABEL = "{0} sat/byte";
        public const int PRECIO_TIMER_INTERVAL = 5; // seconds

        // Block Explorer URLs
        public const string BLOCK_EXPLORER_ADDRESS_MAINNET_URI = "https://blockstream.info/address/{0}";
        public const string BLOCK_EXPLORER_ADDRESS_TESTNET_URI = "https://blockstream.info/testnet/address/{0}";
        public const string BLOCK_EXPLORER_TRANSACTION_MAINNET_URI = "https://blockstream.info/tx/{0}";
        public const string BLOCK_EXPLORER_TRANSACTION_TESTNET_URI = "https://blockstream.info/testnet/tx/{0}";

        // Colors
        public const string SYNC_GRADIENT_START_COLOR_HEX = "#DAAB28";
        public const string GRAY_TEXT_TINT_COLOR_HEX = "#A3A8AD";
        public const string HEX_CHAR = "#";
        public const string DEFAULT_ACCOUNT_COLOR_CODE = "10";
        public const string PREFIX_NAME_STYLE_ACCOUNT_MENU = "MenuItemLabelClass";

        // Localized Strings
        public const string HODL_WALLET = "HODL Wallet";
        public const string SENT_AMOUNT = "Sent {0} {1}"; //Only appears in a unused method in HomeviewModel.cs
        public const string RECEIVE_AMOUNT = "Received {0} {1}";
        public const string SHARE_TEXT_INTENT_TITLE = "Share via"; 
        
        // MessagingCenter
        public const string DISPLAY_ALERT_ERROR_TITLE = "Error"; 
        public const string DISPLAY_ALERT_PASTE_MESSAGE = "Pasteboard is empty or invalid."; 
        public const string DISPLAY_ALERT_SCAN_MESSAGE = "Invalid QR code."; 
        public const string DISPLAY_ALERT_ERROR_SEND_TO_YOURSELF = "Send to yourself is disabled."; 
        public const string DISPLAY_ALERT_ERROR_BIP70 = "BIP70 is not supported."; 
        public const string DISPLAY_ALERT_TRANSACTION_MESSAGE = "There was an error broadcasting your transaction."; 
        public const string DISPLAY_ALERT_AMOUNT_MESSAGE = "Unable to send, check your amount, address and fee"; 

        // Temporary Values
        public const string USE_ADDRESS_FROM_CLIPBOARD = "The Bitcoin address '{0} is in your clipboard, would you like to send to that address?";
        public const string MEMO_LABEL = "In Progress";

        // Build info temp values
        public const string BUILD_INFO_CONTENT = "Branch {0}\nCommit: {1}";
        public const string BUILD_INFO_MESSAGE_TITLE = "Build Info";
        public const string BUILD_INFO_COPIED_TO_CLIPBOARD = "Build info has been copied to the clipboard!";

        // Android Service Values
        public const string TEXT_PLAIN_INTENT_TYPE = "text/plain";
        public const string IMAGE_PNG_INTENT_TYPE = "image/png";
        public const string IMAGE_PNG_ADDRESS_NAME = ".png";
    }
}
