//
// BalanceHeaderViewModel.cs
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

using NBitcoin;
using Liviano.Interfaces;
using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Services;

namespace HodlWallet.Core.ViewModels
{
    class BalanceHeaderViewModel : BaseViewModel
    {
        string balance;
        public string Balance
        {
            get => balance;
            set => SetProperty(ref balance, value);
        }

        string balanceFiat;
        public string BalanceFiat
        {
            get => balanceFiat;
            set => SetProperty(ref balanceFiat, value);
        }

        DisplayCurrencyType currencyType;
        public DisplayCurrencyType CurrencyType
        {
            get => currencyType;
            set => SetProperty(ref currencyType, value);
        }

        IAccount CurrentAccount => WalletService.Wallet.CurrentAccount;
        decimal AccountBalance => CurrentAccount.GetBalance().ToDecimal(MoneyUnit.BTC);

        public ICommand SwitchCurrencyCommand { get; }

        public BalanceHeaderViewModel()
        {
            SwitchCurrencyCommand = new Command(SwitchCurrency);

            if (WalletService.IsStarted) Setup();
            else WalletService.OnStarted += (_, _) => Device.BeginInvokeOnMainThread(() => Setup());
        }

        void SwitchCurrency(object _)
        {
            DisplayCurrencyService.ToggleCurrency();

            UpdateCurrencyType();
        }

        void Setup()
        {
            PrecioService
                .WhenAnyValue(service => service.Rates)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateBalances());

            DisplayCurrencyService
                .WhenAnyValue(service => service.CurrencyType)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateCurrencyType());

            DisplayCurrencyService
                .WhenAnyValue(service => service.BitcoinCurrencyCode, service => service.FiatCurrencyCode)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateBalances());

            CurrentAccount.Txs.CollectionChanged += (_, _) => UpdateBalances();
            WalletService.Wallet.OnSyncFinished += (_, _) => UpdateBalances();

            UpdateBalances();
        }

        void UpdateBalanceFiat()
        {
            BalanceFiat = DisplayCurrencyService.FiatAmountFormatted(AccountBalance);
        }

        void UpdateBalanceBtc()
        {
            Balance = DisplayCurrencyService.BitcoinAmountFormatted(AccountBalance);
        }

        void UpdateBalances()
        {
            UpdateBalanceBtc();
            UpdateBalanceFiat();
        }

        void UpdateCurrencyType()
        {
            CurrencyType = DisplayCurrencyService.CurrencyType;
        }
    }
}