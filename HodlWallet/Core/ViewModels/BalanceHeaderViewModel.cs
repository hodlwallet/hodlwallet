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

using NBitcoin;
using ReactiveUI;

using Liviano.Interfaces;

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

        IAccount CurrentAccount => WalletService.Wallet.CurrentAccount;
        decimal AccountBalance => CurrentAccount.GetBalance().ToDecimal(MoneyUnit.BTC);

        public BalanceHeaderViewModel()
        {
            if (WalletService.IsStarted) Setup();
            else WalletService.OnStarted += (_, _) => Setup();
        }

        void Setup()
        {
            PrecioService
                .WhenAnyValue(service => service.Precio)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateBalanceFiat());

            CurrentAccount.Txs.CollectionChanged += (_, _) => UpdateBalance();

            UpdateBalance();
        }

        void UpdateBalanceFiat()
        {
            BalanceFiat = $"{decimal.Parse(PrecioService.Precio.CRaw) * AccountBalance:C}";

            Console.WriteLine($"Balnace Fiat: {BalanceFiat}");
        }

        void UpdateBalance()
        {
            Balance = $"{AccountBalance} BTC";

            UpdateBalanceFiat();
        }
    }
}