//
// TransactionsViewModel.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Liviano.Interfaces;
using Liviano.Models;
using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Models;
using HodlWallet.UI.Extensions;
using System;

namespace HodlWallet.Core.ViewModels
{
    class TransactionsViewModel : BaseViewModel
    {
        public ObservableCollection<TransactionModel> Transactions { get; } = new ObservableCollection<TransactionModel>();

        const int TXS_DEFAULT_ITEMS_SIZE = 10;

        bool isLoadingCollection = false;

        public ICommand RemainingItemsThresholdReachedCommand { get; }

        public ICommand NavigateToTransactionDetailsCommand { get; }

        TransactionModel currentTransaction;
        public TransactionModel CurrentTransaction
        {
            get => currentTransaction;
            set => SetProperty(ref currentTransaction, value);
        }

        int remainingItemsThreshold = 1;

        public int RemainingItemsThreshold
        {
            get => remainingItemsThreshold;
            set => SetProperty(ref remainingItemsThreshold, value);
        }

        IAccount CurrentAccount => WalletService.Wallet.CurrentAccount;

        List<Tx> Txs => CurrentAccount.Txs.OrderByDescending(tx => tx.CreatedAt).ToList();

        public TransactionsViewModel()
        {
            NavigateToTransactionDetailsCommand = new Command(NavigateToTransactionDetails);
            RemainingItemsThresholdReachedCommand = new Command(RemainingItemsThresholdReached);

            if (WalletService.IsStarted) Init();
            else WalletService.OnStarted += (_, _) => Init();
        }

        void Init()
        {
            LoadTxsFromWallet();
            SubscribeToEvents();
        }

        void SubscribeToEvents()
        {
            WalletService.Wallet.OnNewTransaction += Wallet_OnNewTransaction;
            CurrentAccount.Txs.CollectionChanged += Txs_CollectionChanged;
            WalletService.OnSyncFinished += Wallet_OnSyncFinished;
        }

        void Wallet_OnSyncFinished(object sender, DateTimeOffset e)
        {
            Observable.Start(() =>
            {
                Transactions.Clear();

                LoadTxsFromWallet();
            }, RxApp.MainThreadScheduler);
        }

        void Wallet_OnNewTransaction(object sender, Liviano.Events.TxEventArgs e)
        {
            Debug.WriteLine($"[Wallet_OnNewTransaction] New transaction on wallet {e.Tx.Id}");
        }

        void NavigateToTransactionDetails(object obj)
        {
            if (CurrentTransaction is null) return;

            MessagingCenter.Send(this, "NavigateToTransactionDetail", CurrentTransaction);

            CurrentTransaction = null;
        }

        async void LoadTxsFromWallet(int size = -1)
        {
            isLoadingCollection = true;
            if (size == -1) size = TXS_DEFAULT_ITEMS_SIZE;

            IsLoading = true;
            foreach (var tx in Txs.Take(size))
            {
                var txModel = TransactionModel.FromTransactionData(tx);

                TransactionsAdd(txModel);
            }
            await Task.Delay(420);
            IsLoading = false;

            // FIXME Bug on iOS
            //MessagingCenter.Send(this, "ScrollToTop");

            isLoadingCollection = false;
        }

        void RemainingItemsThresholdReached(object _)
        {
            if (WalletService.Wallet is null) return;
            if (CurrentAccount is null) return;
            if (isLoadingCollection) return;

            isLoadingCollection = true;

            foreach (var tx in Txs.Skip(Transactions.Count).Take(TXS_DEFAULT_ITEMS_SIZE))
            {
                var txModel = TransactionModel.FromTransactionData(tx);
                TransactionsAdd(txModel);
            }

            isLoadingCollection = false;
        }

        void Txs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
            {
                var size = Transactions.Count + e.NewItems.Count;

                Transactions.Clear();

                LoadTxsFromWallet(size);
            }

            //if (e.NewItems is not null)
            //    foreach (Tx item in e.NewItems)
            //    {
            //        var txModel = TransactionModel.FromTransactionData(item);
            //        TransactionsAdd(txModel, 0);
            //    }

            //if (e.OldItems is not null)
            //    foreach (Tx item in e.OldItems)
            //    {
            //        var txModel = TransactionModel.FromTransactionData(item);
            //        TransactionChanged(txModel);
            //    }

            ////Transactions.Sort();

            // FIXME this performs poorly
            //MessagingCenter.Send(this, "ScrollToTop");
        }

        /// <summary>
        /// Add to the observable collection not duplicating
        /// </summary>
        /// <param name="model">A model of a tx</param>
        /// <param name="index">Index where to insert the tx</param>
        void TransactionsAdd(TransactionModel model, int index = -1)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                if (index < 0)
                    Transactions.Add(model);
                else
                    Transactions.Insert(index, model);
            });
        }
    }
}