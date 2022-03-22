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
using System.Collections.Generic;

namespace HodlWallet.Core.Utils
{
    public static class Constants
    {
        // Precio API
        public const string PRECIO_HOST_URL = "https://precio.bitstop.co";
        public const string PRECIO_WS_HOST_URL = "wss://precio.bitstop.co";
        public static readonly string DEFAULT_FIAT_CURRENCY_CODE = "USD";
        public static readonly string EMPTY_CURRENCY_SYMBOL_KEY = "EMPCKEY";
        static readonly string EMPTY_CURRENCY_SYMBOL_VALUE = "";
        public const string BTC_LABEL = "BTC";
        public static readonly Dictionary<string, string> CURRENCY_SYMBOLS = new()
        {
            //TODO: Complete currencies symbols. CODE is used by default. 
            { EMPTY_CURRENCY_SYMBOL_KEY, EMPTY_CURRENCY_SYMBOL_VALUE },
            { "USD", "\u0024" }, //US dollar - Hex Code
            { "EUR", "\u20AC" }, //Euro  - CSS Code
            { "GBP", "\u00A3" }, //Sterling Pound - HTML code
            { "JPY", "\u00A5" }, //Japanese yen  - HTML Entity
            { "CNY", "\u00A5" }, //Chinese Renminbi yuan
            { "KRW", "\u20A9" }, // South Korean Won
            { "BTC", "\u20BF" }, // Bitcoin
            { "SAT", "SAT" },    // Satoshi
            { "BCH", "Ƀ" },      // Bitcoin Cash
            { "CAD", "CA$" },    //Canadian dollar
            { "AUD", "AU$" },    //Australian dollar
            { "NZD", "NZ$" },    // New Zealand Dollar
            { "ARS", "AR$" },    // Argentine Peso
            { "BBD", "BBD$" },   // Barbadian Dollar
            { "BND", "B$" },     // Brunei Dollar
            { "BRL", "R$" },     // Brazilian Real
            { "BSD", "B$" },     // Bahamian Dollar
            { "CHF", "\u20A3" }, // Swiss franc
            { "SEK", "kr" },     // Swedish krona
            { "XRP", "XRP" },    // Ripple
            { "BAM", "KM" },     // Bosnia-Herzegovina Convertible Mark
            { "AOA", "Kz" },     // Angolan Kwanza
            { "AWG", "Afl" },    // Aruban Florin
            { "BIF", "FBu" },    // Burundian Franc
            { "BUSD", "BUSD" },  // Binance USD
            { "BOB", "Bs" },     // Bolivian Boliviano
            { "BTN", "Nu" },     // Bhutanese Ngultrum
            { "ALL", "L" },      // Albanian Lek
            { "ANG", "NAƒ" },    // Netherlands Antillean Guilder
            { "ETH", "Ξ" },      // Ethereum
            { "LTC", "Ł" },      // Lite Coin
            { "AED", "د.إ" },    // UAE Dirham
            { "AFN", "\u060b" }, // Afghan Afghani
            { "AMD", "֏" },      // Armenian Dram
            { "AZN", "\u20bc" }, // Azerbaijani Manat
            { "BDT", "৳" },      // Bangladeshi Taka
            { "BGN", "лв" },     // Bulgarian Lev
            { "BHD", ".د.ب" },   // Bahraini Dinar
            { "BMD", "BMD" },    // Bermudan Dollar
            { "BWP", "BWP" },    // Botswanan Pula
            { "BYN", "BYN" },    // Belarusian Ruble
            { "BZD", "BZD" },    // Belize Dollar
            { "CDF", "CDF" },    // Congolese Franc
            { "CLF", "CLF" },    // Chilean Unit of Account (UF)
            { "CLP", "CLP" },    // Chilean Peso
            { "COP", "\u0024" }, // Colombian Peso
            { "CRC", "\u20A1" }, // Costa Rican Colón
            { "CUP", "CUP" },    // Cuban Peso
            { "CVE", "CVE" },    // Cape Verdean Escudo
            { "CZK", "CZK" },    // Czech Koruna
            { "DAI", "DAI" },    // Dai
            { "DJF", "DJF" },    // Djiboutian Franc
            { "DKK", "DKK" },    // Danish Krone
            { "DOGE", "DOGE" },  // Dogecoin
            { "DOP", "DOP" },    // Dominican Peso
            { "DZD", "DZD" },    // Algerian Dinar
            { "EGP", "EGP" },    // Egyptian Pound
            { "ETB", "ETB" },    // Ethiopian Birr
            { "FJD", "FJ$" },    // Fijian Dollar
            { "FKP", "FKP" },    // Falkland Islands Pound
            { "GEL", "GEL" },    // Georgian Lari
            { "GHS", "GH₵" },    // Ghanaian Cedi
            { "GIP", "GIP" },    // Gibraltar Pound
            { "GMD", "GMD" },    // Gambian Dalasi
            { "GNF", "GNF" },    // Guinean Franc
            { "GTQ", "Q" },      // Guatemalan Quetzal
            { "GUSD", "GUSD" },  // Gemini US Dollar
            { "GYD", "GYD" },    // Guyanaese Dollar
            { "HKD", "$" },      // Hong Kong Dollar
            { "HNL", "L" },      // Honduran Lempira
            { "HRK", "kn" },     // Croatian Kuna
            { "HTG", "HTG" },    // Haitian Gourde
            { "HUF", "HUF" },    // Hungarian Forint
            { "IDR", "Rp" },     // Indonesian Rupiah
            { "ILS", "₪" },      // Israeli Shekel
            { "INR", "\u20b9" }, // Indian Rupee
            { "IQD", "IQD" },    // Iraqi Dinar
            { "IRR", "IRR" },    // Iranian Rial
            { "ISK", "kr" },     // Icelandic Króna
            { "JEP", "JEP" },    // Jersey Pound
            { "JMD", "J$" },     // Jamaican Dollar
            { "JOD", "JOD" },    // Jordanian Dinar
            { "KES", "KES" },    // Kenyan Shilling
            { "KGS", "KGS" },    // Kyrgystani Som
            { "KHR", "KHR" },    // Cambodian Riel
            { "KMF", "KMF" },    // Comorian Franc
            { "KPW", "KPW" },    // North Korean Won
            { "KWD", "KWD" },    // Kuwaiti Dinar
            { "KYD", "KYD" },    // Cayman Islands Dollar
            { "KZT", "\u20b8" }, // Kazakhstani Tenge
            { "LAK", "\u20ad" }, // Laotian Kip
            { "LBP", "LBP" },    // Lebanese Pound
            { "LKR", "₨" },      // Sri Lankan Rupee
            { "LRD", "LRD" },    // Liberian Dollar
            { "LSL", "LSL" },    // Lesotho Loti
            { "LYD", "LYD" },    // Libyan Dinar
            { "MAD", "MAD" },    // Moroccan Dirham
            { "MDL", "MDL" },    // Moldovan Leu
            { "MGA", "MGA" },    // Malagasy Ariary
            { "MKD", "MKD" },    // Macedonian Denar
            { "MMK", "K" },      // Myanma Kyat
            { "MNT", "\u20ae" }, // Mongolian Tugrik
            { "MOP", "MOP" },    // Macanese Pataca
            { "MRU", "MRU" },    // Mauritanian Ouguiya
            { "MUR", "MUR" },    // Mauritian Rupee
            { "MVR", "MVR" },    // Maldivian Rufiyaa
            { "MWK", "MWK" },    // Malawian Kwacha
            { "MXN", "$" },      // Mexican Peso
            { "MYR", "RM" },     // Malaysian Ringgit
            { "MZN", "MZN" },    // Mozambican Metical
            { "NAD", "NAD" },    // Namibian Dollar
            { "NGN", "NGN" },    // Nigerian Naira
            { "NIO", "C$" },     // Nicaraguan Córdoba
            { "NOK", "NOK" },    // Norwegian Krone
            { "NPR", "NPR" },    // Nepalese Rupee
            { "OMR", "OMR" },    // Omani Rial
            { "PAB", "B/." },    // Panamanian Balboa
            { "PAX", "PAX" },    // Paxos Standard USD
            { "PEN", "S/." },    // Peruvian Nuevo Sol
            { "PGK", "PGK" },    // Papua New Guinean Kina
            { "PHP", "₱" },      // Philippine Peso
            { "PKR", "₨" },      // Pakistani Rupee
            { "PLN", "PLN" },    // Polish Zloty
            { "PYG", "\u20b2" }, // Paraguayan Guarani
            { "QAR", "QAR" },    // Qatari Rial
            { "RON", "RON" },    // Romanian Leu
            { "RSD", "RSD" },    // Serbian Dinar
            { "RUB", "\u20bd" }, // Russian Ruble
            { "RWF", "RWF" },    // Rwandan Franc
            { "SAR", "SAR" },    // Saudi Riyal
            { "SBD", "SBD" },    // Solomon Islands Dollar
            { "SCR", "SCR" },    // Seychellois Rupee
            { "SDG", "SDG" },    // Sudanese Pound
            { "SGD", "S$" },     // Singapore Dollar
            { "SHP", "SHP" },    // Saint Helena Pound
            { "SLL", "SLL" },    // Sierra Leonean Leone
            { "SOS", "SOS" },    // Somali Shilling
            { "SRD", "SRD" },    // Surinamese Dollar
            { "STN", "STN" },    // São Tomé and Príncipe Dobra
            { "SVC", "SVC" },    // Salvadoran Colón
            { "SYP", "SYP" },    // Syrian Pound
            { "SZL", "SZL" },    // Swazi Lilangeni
            { "THB", "\u0e3f" }, // Thai Baht
            { "TJS", "TJS" },    // Tajikistani Somoni
            { "TMT", "TMT" },    // Turkmenistani Manat
            { "TND", "DT" },     // Tunisian Dinar
            { "TOP", "TOP" },    // Tongan Paʻanga
            { "TRY", "\u20ba" }, // Turkish Lira
            { "TTD", "TTD" },    // Trinidad and Tobago Dollar
            { "TWD", "NT$" },    // New Taiwan Dollar
            { "TZS", "TZS" },    // Tanzanian Shilling
            { "UAH", "\u20b4" }, // Ukrainian Hryvnia
            { "UGX", "UGX" },    // Ugandan Shilling
            { "USDC", "USDC" },  // Circle USD Coin
            { "UYU", "UYU" },    // Uruguayan Peso
            { "UZS", "UZS" },    // Uzbekistan Som
            { "VES", "Bs. S" },  // Venezuelan Bolívar Soberano
            { "VND", "\u20ab" }, // Vietnamese Dong
            { "VUV", "VUV" },    // Vanuatu Vatu
            { "WBTC", "WBTC" },  // Wrapped BTC
            { "SHIB", "SHIB" },  // Shiba Inu
            { "WST", "WST" },    // Samoan Tala
            { "XAF", "XAF" },    // CFA Franc BEAC
            { "XAG", "XAG" },    // Silver (troy ounce)
            { "XAU", "XAU" },    // Gold (troy ounce)
            { "XCD", "XCD" },    // East Caribbean Dollar
            { "XPF", "XPF" },    // CFP Franc
            { "XOF", "XOF" },    // CFA Franc BCEAO
            { "YER", "YER" },    // Yemeni Rial
            { "ZAR", "ZAR" },    // South African Rand
            { "ZMW", "ZMW" },    // Zambian Kwacha
            { "ZWL", "Z$" }      // Zimbabwean Dollar
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