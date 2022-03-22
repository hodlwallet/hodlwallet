﻿//
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

[assembly: Dependency(typeof(DisplayCurrencyService))]
namespace HodlWallet.Core.Services
{
    public enum DisplayCurrencyType
    {
        BTC,
        Satoshi,
        Fiat
    }

    public class DisplayCurrencyService : ReactiveObject, IDisplayCurrencyService
    {
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

        public void Load()
        {
            throw new System.NotImplementedException();
        }
    }
}