//
// DisplayCurrencyService.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
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
using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Interfaces;
using HodlWallet.Core.Services;
using HodlWallet.Core.Utils;
using System.Linq;
using System;

[assembly: Dependency(typeof(DisplayCurrencyService))]
namespace HodlWallet.Core.Services
{
    public enum DisplayCurrencyType
    {
        Bitcoin,
        Fiat
    }

    public class DisplayCurrencyService : ReactiveObject, IDisplayCurrencyService
    {
        IPrecioService PrecioService => DependencyService.Get<IPrecioService>();

        DisplayCurrencyType currencyType;
        public DisplayCurrencyType CurrencyType
        {
            get => currencyType;
            set => this.RaiseAndSetIfChanged(ref currencyType, value);
        }

        string fiatCurrencyCode;
        public string FiatCurrencyCode
        {
            get => fiatCurrencyCode;
            set => this.RaiseAndSetIfChanged(ref fiatCurrencyCode, value);
        }

        string bitcoinCurrencyCode;
        public string BitcoinCurrencyCode
        {
            get => bitcoinCurrencyCode;
            set => this.RaiseAndSetIfChanged(ref bitcoinCurrencyCode, value);
        }

        public void Load()
        {
            FiatCurrencyCode = SecureStorageService.GetFiatCurrencyCode();
            BitcoinCurrencyCode = SecureStorageService.GetBitcoinCurrencyCode();
            CurrencyType = SecureStorageService.GetDisplayCurrencyType();
        }

        public void Save()
        {
            SecureStorageService.SetFiatCurrencyCode(FiatCurrencyCode);
            SecureStorageService.SetBitcoinCurrencyCode(BitcoinCurrencyCode);
            SecureStorageService.SetDisplayCurrencyType(CurrencyType);
        }

        public string FiatAmountFormatted(decimal amount)
        {
            var symbol = Constants.CURRENCY_SYMBOLS[FiatCurrencyCode];

            if (PrecioService.Precio is null)
                return $"{symbol}{amount:N}";

            decimal rate;
            if (FiatCurrencyCode == "USD")
                rate = decimal.Parse(PrecioService.Precio.CRaw);
            else
                rate = (decimal)PrecioService.Rates.FirstOrDefault(r => r.Code == FiatCurrencyCode).Rate;

            var finalAmount = rate * amount;

            // Do not format really small numbers
            if (finalAmount < 0.01m) return $"{symbol}{finalAmount}";

            return $"{symbol}{finalAmount:N}";
        }

        public string BitcoinAmountFormatted(decimal amount)
        {
            var symbol = Constants.CURRENCY_SYMBOLS[BitcoinCurrencyCode];

            return BitcoinCurrencyCode switch
            {
                "BTC" => $"{symbol}{amount}",
                "SAT" => $"{(int)(amount * 100_000_000)} {symbol}",
                _ => $"{symbol}{amount}",
            };
        }
    }
}
