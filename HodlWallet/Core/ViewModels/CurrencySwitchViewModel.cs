//
// CurrencySwitchViewModel.cs
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
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Services;
using HodlWallet.Core.Utils;

namespace HodlWallet.Core.ViewModels
{
    public partial class CurrencySwitchViewModel : LightBaseViewModel
    {
        [ObservableProperty]
        string fiatSymbol = "$";

        public CurrencySwitchViewModel()
        {
            if (WalletService.IsStarted) Setup();
            else WalletService.OnStarted += (_, _) => Setup();
        }

        void Setup()
        {
            DisplayCurrencyService
                .WhenAnyValue(service => service.CurrencyType)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ct => SwitchCurrencyTo(ct));

            DisplayCurrencyService
                .WhenAnyValue(service => service.FiatCurrencyCode)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(cc => UpdateFiatSymbol(cc));

            SwitchCurrencyTo(DisplayCurrencyService.CurrencyType);
            UpdateFiatSymbol(DisplayCurrencyService.FiatCurrencyCode);
        }

        void UpdateFiatSymbol(string cc)
        {
            FiatSymbol = Constants.CURRENCY_SYMBOLS[cc];
        }

        void SwitchCurrencyTo(DisplayCurrencyType ct)
        {
            if (ct == DisplayCurrencyType.Bitcoin) MessagingCenter.Send(this, "ActivateBitcoin");
            else MessagingCenter.Send(this, "ActivateFiat");
        }

        [ICommand]
        void ToggleCurrency()
        {
            DisplayCurrencyService.ToggleCurrency();
        }
    }
}