﻿//
// HomeViewModel.cs
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
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Essentials;

using NBitcoin;

using Liviano;
using Liviano.Models;

using HodlWallet.Core.Interfaces;
using HodlWallet.Core.Models;
using HodlWallet.Core.Utils;
using HodlWallet.Core.Services;
using HodlWallet.Core.Extensions;
using HodlWallet.UI.Views;
using Liviano.Events;

namespace HodlWallet.Core.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        Serilog.ILogger _Logger;

        bool _IsViewVisible = true;

        public string SyncTitleText => "SYNCING";

        bool _AttachedWalletListeners = false;
        decimal _Amount;
        decimal _AmountFiat;
        float _NewRate;
        float _OldRate;
        bool _IsBtcEnabled;
        TransactionModel _CurrentTransaction;

        int _PriceUpdateDelay = 2_500; // 2.5 seconds

        public TransactionModel CurrentTransaction
        {
            get => _CurrentTransaction;
            set => SetProperty(ref _CurrentTransaction, value);
        }

        public bool IsBtcEnabled
        {
            get => _IsBtcEnabled;
            set => SetProperty(ref _IsBtcEnabled, value);
        }

        public decimal Amount
        {
            get => _Amount;
            set => SetProperty(ref _Amount, value);
        }

        public decimal AmountFiat
        {
            get => _AmountFiat;
            set => SetProperty(ref _AmountFiat, value);
        }

        private object _Lock = new object();
        public ObservableCollection<TransactionModel> Transactions { get; } = new ObservableCollection<TransactionModel>();

        Color _GradientStart;
        public Color GradientStart
        {
            get => _GradientStart;
            set => SetProperty(ref _GradientStart, value);
        }

        Color _GradientEnd;
        public Color GradientEnd
        {
            get => _GradientEnd;
            set => SetProperty(ref _GradientEnd, value);
        }

        string _PriceText;
        public string PriceText
        {
            get => _PriceText;
            set => SetProperty(ref _PriceText, value);
        }

        string _SyncDateText;
        public string SyncDateText
        {
            get => _SyncDateText;
            set => SetProperty(ref _SyncDateText, value);
        }

        double _SyncCurrentProgress;
        public double SyncCurrentProgress
        {
            get => _SyncCurrentProgress;
            set => SetProperty(ref _SyncCurrentProgress, value);
        }

        bool _SyncIsVisible;
        public bool SyncIsVisible
        {
            get => _SyncIsVisible;
            set => SetProperty(ref _SyncIsVisible, value);
        }

        public ICommand SwitchCurrencyCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand NavigateToTransactionDetailsCommand { get; }
        public ICommand NavigateToSendCommand { get; }
        public ICommand NavigateToReceiveCommand { get; }

        public HomeViewModel()
        {
            _Logger = _WalletService.Logger;

            NavigateToTransactionDetailsCommand = new Command(NavigateToTransactionDetails);
            SwitchCurrencyCommand = new Command(SwitchCurrency);
            SearchCommand = new Command(StartSearch);
            NavigateToSendCommand = new Command(NavigateToSend);
            NavigateToReceiveCommand = new Command(NavigateToReceive);

            PriceText = Constants.BTC_UNIT_LABEL_TMP;
        }

        public void View_OnDisappearing()
        {
            _IsViewVisible = false;
        }

        public void View_OnAppearing()
        {
            _IsViewVisible = true;

            _Logger = _WalletService.Logger;

            InitializeWalletAndPrecio();
            InitializePrecioAndWalletTimers(); // TODO see bellow
            InitializeWalletServiceTransactions();
        }

        public void InitializeWalletAndPrecio()
        {
            // FIXME This logic needs to change...
            if (_AttachedWalletListeners) return;

            InitializePrecioAndWalletTimers();

            InitializeWalletServiceTransactions();

            _AttachedWalletListeners = true;
        }

        void InitializeWalletServiceTransactions()
        {
            if (_WalletService.IsStarted)
            {
                LoadTransactions();
                AddWalletServiceEvents();
                GradientStart = Color.Purple;
                GradientEnd = Color.Black;
            }
            else
            {
                _WalletService.OnStarted += _WalletService_OnStarted;
            }

            // FIXME for now we gonna include the unconfirmed transactions, but this should not be the case
            if (_WalletService.IsStarted)
            {
                Amount = _WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                AmountFiat = Amount * (decimal)_NewRate;
            }
            else
            {
                _WalletService.OnStarted += _WalletService_OnStarted_ViewAppearing;
            }

            IsBtcEnabled = true;
        }

        void StartSearch()
        {
            Debug.WriteLine("[StartSearch] Search is not implemented yet!");

            MessagingCenter.Send(this, "DisplaySearchNotImplementedAlert");
        }

        void SwitchCurrency()
        {
            if (!_WalletService.IsStarted) return;

            var currency = Preferences.Get("currency", "BTC");
            if (currency == "BTC")
            {
                Preferences.Set("currency", "USD");

                Amount = _WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                AmountFiat = Amount * (decimal)_NewRate;

                UpdateTransanctions();

                IsBtcEnabled = false;
            }
            else
            {
                Preferences.Set("currency", "BTC");

                Amount = _WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                AmountFiat = Amount * (decimal)_NewRate;

                UpdateTransanctions();

                IsBtcEnabled = true;
            }

            MessagingCenter.Send(this, "SwitchCurrency");
        }

        void InitializePrecioAndWalletTimers()
        {
            // Run and schedule next times precio will be called
            using (var cts = new CancellationTokenSource())
            {
                Task.Factory.StartNew(async (options) =>
                {
                    while (true)
                    {
                        if (!_IsViewVisible)
                        {
                            Debug.WriteLine("[InitializePrecioAndWalletTimers] Stopped timer.");

                            cts.Cancel();
                            break;
                        }

                        // Gets first BTC-USD rate.
                        var rate = _PrecioService.Rate;
                        if (rate != null)
                        {
                            // Sets both old and new rate for comparison on timer to optimize fiat currency updates based on current rate.
                            _OldRate = _NewRate = rate.Rate;

                            AmountFiat = Amount * (decimal)_NewRate;

                            UpdateTransanctions();
                        }

                        await Task.Delay(_PriceUpdateDelay);
                    }
                }, TaskCreationOptions.LongRunning, cts.Token);
            }
        }

        void UpdateTransanctions()
        {
            if (Preferences.Get("currency", "BTC") != "BTC")
            {
                AmountFiat = Amount * (decimal)_NewRate;

                if (!_OldRate.Equals(_NewRate))
                {
                    _OldRate = _NewRate;

                    // DO NOT convert this into a foreach loop or LINQ statement.
                    for (int i = 0; i < Transactions.Count; i++)
                    {
                        Transactions[i].AmountText = (Transactions[i].Amount.ToDecimal(MoneyUnit.BTC) * (decimal)_NewRate).ToString("C");
                    }
                }

                return;
            }

            for (int i = 0; i < Transactions.Count; i++)
            {
                Transactions[i].AmountText = Transactions[i].Amount.ToDecimal(MoneyUnit.BTC).ToString();
            }
        }

        private void _WalletService_OnStarted_ViewAppearing(object sender, EventArgs e)
        {
            _Logger = _WalletService.Logger;

            Device.BeginInvokeOnMainThread(() =>
            {
                lock (_Lock)
                {
                    Amount = _WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                    AmountFiat = Amount * (decimal)_NewRate;

                    _WalletService.OnStarted -= _WalletService_OnStarted_ViewAppearing;
                }
            });
        }

        void NavigateToSend()
        {
            MessagingCenter.Send(this, "ChangeCurrentPageTo", RootView.Tabs.Send);
        }

        void NavigateToReceive()
        {
            MessagingCenter.Send(this, "ChangeCurrentPageTo", RootView.Tabs.Receive);
        }

        void NavigateToTransactionDetails()
        {
            if (CurrentTransaction is null) return;

            MessagingCenter.Send(this, "NavigateToTransactionDetail", CurrentTransaction);

            CurrentTransaction = null;
        }

        async Task RatesAsync()
        {
            var rates = await _PrecioHttpService.GetRates();

            foreach (var rate in rates)
            {
                if (rate.Code == "USD")
                {
                    var price = _NewRate = rate.Rate;
                    //PriceText = string.Format(CultureInfo.CurrentCulture, Constants.BTC_UNIT_LABEL, price);
                    PriceText = string.Format(CultureInfo.CurrentCulture, "{0:C}", price);
                    //PriceText = price.ToString("0.00");
                    break;
                }
            }
        }

        /*
        void WalletSyncManager_OnSyncProgressUpdate(object sender, WalletPositionUpdatedEventArgs e)
        {
            _Logger.Debug(
                "[{0}] e.NewPosition.Height => {1}, e.PreviousPosition.Height => {2}",
                nameof(WalletSyncManager_OnSyncProgressUpdate),
                e.NewPosition.Height,
                e.PreviousPosition.Height
            );
        }
        */

        void _WalletService_OnStarted(object sender, EventArgs e)
        {
            _Logger = _WalletService.Logger;

            Device.BeginInvokeOnMainThread(() =>
            {
                lock (_Lock)
                {
                    LoadTransactions();

                    AddWalletServiceEvents();

                    GradientStart = Color.Purple;
                    GradientEnd = Color.Black;
                }
            });
        }

        void AddWalletServiceEvents()
        {
            _WalletService.Wallet.ElectrumPool.OnNewTransaction += Wallet_OnNewTransaction;
            _WalletService.Wallet.ElectrumPool.OnUpdateTransaction += Wallet_OnUpdateTransaction;
        }

        void Wallet_OnNewTransaction(object sender, TxEventArgs e)
        {
            AddToTransactionsCollectionWith(e);
        }

        void Wallet_OnUpdateTransaction(object sender, TxEventArgs e)
        {
            UpdateTransactionsCollectionWith(e);
        }

        void AddToTransactionsCollectionWith(TxEventArgs txEventArgs)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (_Lock)
                {
                    // Double Check if the tx is there or not...
                    if (Transactions.Any(tx => tx.Id == txEventArgs.Tx.Id)) return;

                    Transactions.Insert(0, TransactionModel.FromTransactionData(txEventArgs.Tx));
                }
            });
        }

        void UpdateTransactionsCollectionWith(TxEventArgs txEventArgs)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (_Lock)
                {
                    var txModel = TransactionModel.FromTransactionData(txEventArgs.Tx);
                    var tx = Transactions.FirstOrDefault(tx1 => tx1.Id == txEventArgs.Tx.Id);

                    if (tx is null) return;
                    if (tx == txModel) return;

                    int index = Transactions.IndexOf(tx);

                    Transactions.Remove(tx);
                    Transactions.Insert(index, txModel);
                }
            });
        }

        void LoadTransactions()
        {
            var txs = _WalletService.GetCurrentAccountTransactions().OrderBy(
                (Tx txData) => txData.CreatedAt
            );

            Transactions.Clear();

            foreach (var tx in txs)
            {
                if (Transactions.Any(txModel => txModel.Id == tx.Id))
                {
                    var item = Transactions.FirstOrDefault(txModel => txModel.Id == tx.Id);
                    var newItem = TransactionModel.FromTransactionData(tx);

                    if (item == newItem) continue;

                    int index = Transactions.IndexOf(item);
                    Transactions.Remove(item);
                    Transactions.Insert(index, newItem);

                    continue;
                }

                Transactions.Insert(0, TransactionModel.FromTransactionData(tx));
            }
        }

        string GetAmountLabelText(Tx tx)
        {
            if (tx is null)
            {
                _Logger.Debug("Tx is null, because we need to figure out how to change amounts");

                return "";
            }

            var preferences = Preferences.Get("currency", "BTC");

            if (preferences == "BTC")
            {
                if (tx.IsSend == true)
                    return string.Format(Constants.SENT_AMOUNT, preferences, $"-{tx.AmountSent}");

                return string.Format(Constants.RECEIVE_AMOUNT, preferences, tx.SpendableAmount(false));
            }

            if (tx.IsSend == true)
                return string.Format(
                Constants.SENT_AMOUNT,
                preferences,
                $"-{tx.AmountSent.ToUsd((decimal)_NewRate):F2}");

            return string.Format(
                Constants.RECEIVE_AMOUNT,
                preferences,
                $"{tx.SpendableAmount(false).ToUsd((decimal)_NewRate):F2}");
        }
    }
}