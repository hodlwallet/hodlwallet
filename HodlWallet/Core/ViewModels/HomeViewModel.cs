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
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Essentials;

using NBitcoin;

using HodlWallet.Core.Models;
using HodlWallet.Core.Utils;
using HodlWallet.Core.Extensions;

using Liviano.Events;
using Liviano.Interfaces;
using Liviano.Models;

namespace HodlWallet.Core.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        Serilog.ILogger logger;

        bool isViewVisible = true;

        bool attachedWalletListeners = false;
        decimal balance;
        decimal balanceFiat;
        float newRate;
        float oldRate;
        bool isBtcEnabled;
        TransactionModel currentTransaction;

        readonly int priceUpdateDelay = 2_500; // 2.5 seconds

        public TransactionModel CurrentTransaction
        {
            get => currentTransaction;
            set => SetProperty(ref currentTransaction, value);
        }

        public bool IsBtcEnabled
        {
            get => isBtcEnabled;
            set => SetProperty(ref isBtcEnabled, value);
        }

        decimal rate;
        public decimal Rate
        {
            get => rate;
            set => SetProperty(ref rate, value);
        }

        string accountName;
        public string AccountName
        {
            get => accountName;
            set => SetProperty(ref accountName, value);
        }

        public decimal Balance
        {
            get => balance;
            set => SetProperty(ref balance, value);
        }

        public decimal BalanceFiat
        {
            get => balanceFiat;
            set => SetProperty(ref balanceFiat, value);
        }

        readonly object @lock = new();
        public ObservableCollection<TransactionModel> Transactions { get; } = new ObservableCollection<TransactionModel>();

        string priceText;
        public string PriceText
        {
            get => priceText;
            set => SetProperty(ref priceText, value);
        }

        string syncDateText;
        public string SyncDateText
        {
            get => syncDateText;
            set => SetProperty(ref syncDateText, value);
        }

        double syncCurrentProgress;
        public double SyncCurrentProgress
        {
            get => syncCurrentProgress;
            set => SetProperty(ref syncCurrentProgress, value);
        }

        bool syncIsVisible;
        public bool SyncIsVisible
        {
            get => syncIsVisible;
            set => SetProperty(ref syncIsVisible, value);
        }

        string currency;
        public string Currency
        {
            get
            {
                if (!string.IsNullOrEmpty(currency))
                    return currency;

                SetProperty(ref currency, Preferences.Get("currency", "BTC"));

                return currency;
            }
            set
            {
                // TODO Add validation
                SetProperty(ref currency, value);
                Preferences.Set("currency", currency);
            }
        }

        public ICommand SwitchCurrencyCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand NavigateToTransactionDetailsCommand { get; }
        public ICommand NavigateToSendCommand { get; }
        public ICommand NavigateToReceiveCommand { get; }

        public HomeViewModel()
        {
            logger = WalletService.Logger;

            NavigateToTransactionDetailsCommand = new Command(NavigateToTransactionDetails);
            SwitchCurrencyCommand = new Command(SwitchCurrency);
            SearchCommand = new Command(StartSearch);
            NavigateToSendCommand = new Command(NavigateToSend);
            NavigateToReceiveCommand = new Command(NavigateToReceive);

            PriceText = Constants.BTC_UNIT_LABEL_TMP;
        }

        public void View_OnDisappearing()
        {
            isViewVisible = false;
        }

        public void View_OnAppearing()
        {
            isViewVisible = true;

            InitializePrecioAndWalletTimers(); // TODO see bellow
            InitializeWalletServiceTransactions();
            InitializeWalletAndPrecio();
        }

        public void InitializeWalletAndPrecio()
        {
            // FIXME This logic needs to change...
            if (attachedWalletListeners) return;

            InitializePrecioAndWalletTimers();

            InitializeWalletServiceTransactions();

            attachedWalletListeners = true;
        }

        void InitializeWalletServiceTransactions()
        {
            if (WalletService.IsStarted)
            {
                LoadTransactions();
                AddWalletServiceEvents();

                AccountName = WalletService.Wallet.CurrentAccount.Name;
                Balance = WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                Rate = (decimal)newRate;
                if (Currency == "USD")
                { 
                    Balance = Balance * Rate;
                }
                Preferences.Set("Balance", Balance.ToString());
            }
            else
            {
                WalletService.OnStarted += WalletService_OnStarted;
            }

            IsBtcEnabled = true;
        }

        void StartSearch()
        {
            Debug.WriteLine("[StartSearch] Search is not implemented yet!");

            MessagingCenter.Send(this, "DisplaySearchNotImplementedAlert");
        }

        public void SwitchCurrency()
        {
            if (!WalletService.IsStarted) return;

            if (Currency == "BTC")
            {
                Currency = "USD";
                Rate = (decimal)newRate;
                
                Balance = WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                Balance = Balance * Rate;
                Preferences.Set("Balance", Balance.ToString());

                UpdateTransanctions();

                IsBtcEnabled = false;
            }
            else
            {
                Currency = "BTC";

                Balance = WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                Preferences.Set("Balance", Balance.ToString());

                UpdateTransanctions();

                IsBtcEnabled = true;
            }

            MessagingCenter.Send(this, "SwitchCurrency");
        }

        public void SwitchAccount(IAccount account)
        {
            Debug.WriteLine($"[SwitchAccount] AccountID: {account.Id}");

            var wallet = WalletService.Wallet;
            for (int i = 0; i < wallet.Accounts.Count; i++)
            {
                if (wallet.Accounts[i].Id != account.Id) continue;

                WalletService.Wallet.CurrentAccount = WalletService.Wallet.Accounts[i];

                break;
            }

            WalletService.Wallet.Storage.Save();
        }

        void InitializePrecioAndWalletTimers()
        {
            // Run and schedule next times precio will be called
            using var cts = new CancellationTokenSource();
            Task.Factory.StartNew(async (options) =>
            {
                while (true)
                {
                    if (!isViewVisible)
                    {
                        Debug.WriteLine("[InitializePrecioAndWalletTimers] Stopped timer.");

                        cts.Cancel();
                        break;
                    }

                    // Gets first BTC-USD rate.
                    //var rate = PrecioService.Rate;
                    //if (rate != null)
                    if (Rate == 0)
                    {
                        // Sets both old and new rate for comparison on timer to optimize fiat currency updates based on current rate.
                        //oldRate = newRate = rate.Rate;
                        oldRate = newRate = 10000;
                        Rate = (decimal)newRate;

                        Balance = WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                        if (Currency == "USD")
                        {
                            //BalanceFiat = Balance * Rate;
                            Balance = Balance * Rate;
                        }
                        Preferences.Set("Balance", Balance.ToString());
                    }

                    await Task.Delay(priceUpdateDelay);
                }
            }, TaskCreationOptions.LongRunning, cts.Token);
        }

        void UpdateTransanctions()
        {
            if (Currency != "BTC")
            {
                Rate = (decimal)newRate;
                Balance = WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                Balance = Balance * Rate;
                Preferences.Set("Balance", Balance.ToString());

                if (!oldRate.Equals(newRate))
                {
                    oldRate = newRate;

                    // DO NOT convert this into a foreach loop or LINQ statement.
                    for (int i = 0; i < Transactions.Count; i++)
                    {
                        Transactions[i].AmountText = (Transactions[i].Amount.ToDecimal(MoneyUnit.BTC) * (decimal)newRate).ToString("C");
                    }
                }

                return;
            }

            for (int i = 0; i < Transactions.Count; i++)
            {
                Transactions[i].AmountText = Transactions[i].Amount.ToDecimal(MoneyUnit.BTC).ToString("C");
            }
        }

        private void WalletService_OnStarted_ViewAppearing(object sender, EventArgs e)
        {
            logger = WalletService.Logger;

            Device.BeginInvokeOnMainThread(() =>
            {
                lock (@lock)
                {
                    AccountName = WalletService.Wallet.CurrentAccount.Name;
                    Balance = WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                    Rate = (decimal)newRate;
                    if (Currency == "USD")
                    {
                        Balance = Balance * Rate;
                    }
                    Preferences.Set("Balance", Balance.ToString());

                    WalletService.OnStarted -= WalletService_OnStarted_ViewAppearing;
                }
            });
        }

        async void NavigateToSend()
        {
            await Shell.Current.GoToAsync("send");
        }

        async void NavigateToReceive()
        {
            await Shell.Current.GoToAsync("receive");
        }

        void NavigateToTransactionDetails()
        {
            if (CurrentTransaction is null) return;

            MessagingCenter.Send(this, "NavigateToTransactionDetail", CurrentTransaction);

            CurrentTransaction = null;
        }

        async Task RatesAsync()
        {
            var rates = await PrecioHttpService.GetRates();

            foreach (var rate in rates)
            {
                if (rate.Code == "USD")
                {
                    //var price = newRate = rate.Rate;
                    var price = newRate = 10000;
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

        void WalletService_OnStarted(object sender, EventArgs e)
        {
            logger = WalletService.Logger;

            Device.BeginInvokeOnMainThread(() =>
            {
                lock (@lock)
                {
                    Balance = WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                    if (Currency == "USD")
                    {
                        Balance = Balance * Rate;
                    }
                    Preferences.Set("Balance", Balance.ToString());
                    LoadTransactions();

                    AddWalletServiceEvents();
                }
            });
        }

        void AddWalletServiceEvents()
        {
            WalletService.Wallet.ElectrumPool.OnNewTransaction += Wallet_OnNewTransaction;
            WalletService.Wallet.ElectrumPool.OnUpdateTransaction += Wallet_OnUpdateTransaction;
            WalletService.Wallet.OnCurrentAccountChanged += Wallet_OnCurrentAccountChanged;
        }

        private void Wallet_OnCurrentAccountChanged(object sender, EventArgs e)
        {
            LoadTransactions();

            Balance = WalletService.Wallet.CurrentAccount.GetBalance().ToUnit(MoneyUnit.BTC);
            Preferences.Set("Balance", Balance.ToString());
            AccountName = WalletService.Wallet.CurrentAccount.Name;

            WalletService.Wallet.Disconnect();

            Task.Factory.StartNew(
                () => WalletService.Wallet.Sync(),
                WalletService.Wallet.Cts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );
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
                lock (@lock)
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
                lock (@lock)
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
            var txs = WalletService.GetCurrentAccountTransactions().OrderBy(
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
                logger.Debug("Tx is null, because we need to figure out how to change amounts");

                return "";
            }

            if (Currency == "BTC")
            {
                if (tx.IsSend == true)
                    return string.Format(Constants.SENT_AMOUNT, Currency, $"-{tx.AmountSent}");

                return string.Format(Constants.RECEIVE_AMOUNT, Currency, tx.SpendableAmount(false));
            }

            if (tx.IsSend == true)
                return string.Format(
                Constants.SENT_AMOUNT,
                Currency,
                $"-{tx.AmountSent.ToUsd((decimal)newRate):F2}");

            return string.Format(
                Constants.RECEIVE_AMOUNT,
                Currency,
                $"{tx.SpendableAmount(false).ToUsd((decimal)newRate):F2}");
        }
    }
}
