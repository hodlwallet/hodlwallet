//
// AmountEntryViewModel.cs
//
// Copyright (c) 2022 HODL Wallet
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
using System.Reactive.Linq;

using ReactiveUI;

using HodlWallet.Core.Services;
using HodlWallet.Core.Utils;
using NBitcoin;

namespace HodlWallet.Core.ViewModels
{
    public class AmountEntryViewModel : BaseViewModel
    {
        string currencySymbol = "₿";
        public string CurrencySymbol
        {
            get { return currencySymbol; }
            set { SetProperty(ref currencySymbol, value); }
        }

        string placeholderAmount = "0.00";
        public string PlaceholderAmount
        {
            get { return placeholderAmount; }
            set { SetProperty(ref placeholderAmount, value); }
        }

        string amountText = "";
        public string AmountText
        {
            get { return amountText; }
            set { SetProperty(ref amountText, value); }
        }

        Money amount = Money.Zero;
        public Money Amount
        {
            get { return amount; }
            set { SetProperty(ref amount, value); }
        }

        public AmountEntryViewModel()
        {
            if (WalletService.IsStarted) Setup();
            else WalletService.OnStarted += (_, _) => Setup();
        }

        void Setup()
        {
            DisplayCurrencyService
                .WhenAnyValue(service => service.CurrencyType)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateCurrency(DisplayCurrencyService.FiatCurrencyCode));

            DisplayCurrencyService
                .WhenAnyValue(service => service.FiatCurrencyCode)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(cc => UpdateCurrency(cc));

            UpdateCurrency(DisplayCurrencyService.FiatCurrencyCode);
        }

        void UpdateCurrency(string cc)
        {
            if (DisplayCurrencyService.CurrencyType == DisplayCurrencyType.Bitcoin)
            {
                CurrencySymbol = "₿";
                PlaceholderAmount = "0.00000000";
            }
            else
            {
                CurrencySymbol = Constants.CURRENCY_SYMBOLS[cc];
                PlaceholderAmount = "0.00";
            }
        }
    }
}