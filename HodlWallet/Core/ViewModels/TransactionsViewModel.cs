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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;

using Liviano.Interfaces;
using Liviano.Models;
using NBitcoin;
using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Models;
using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using HodlWallet.UI.Extensions;

namespace HodlWallet.Core.ViewModels
{
    class TransactionsViewModel : BaseViewModel
    {
        ObservableCollection<TransactionModel> transactions = new();
        public ObservableCollection<TransactionModel> Transactions
        {
            get => transactions;
            set => SetProperty(ref transactions, value);
        }

        const int TXS_DEFAULT_ITEMS_SIZE = 10;

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

        readonly ConcurrentQueue<uint256> queue = new();

        public TransactionsViewModel()
        {
            IsLoading = false;

            NavigateToTransactionDetailsCommand = new Command(NavigateToTransactionDetails);
            RemainingItemsThresholdReachedCommand = new Command(RemainingItemsThresholdReached);

            if (WalletService.IsStarted) Init();
            else WalletService.OnStarted += (_, _) => Init();
        }

        void Init()
        {
            SubscribeToEvents();
            LoadTxsFromWallet();

            Observable.Start(async () => await ProcessQueue(), RxApp.MainThreadScheduler);
        }

        void SubscribeToEvents()
        {
            CurrentAccount.Txs.CollectionChanged += Txs_CollectionChanged;
            WalletService.OnSyncFinished += Wallet_OnSyncFinished;
        }

        async Task ProcessQueue()
        {
            if (queue.IsEmpty) return;

            while (queue.TryDequeue(out var id))
            {
                var tx = Txs.FirstOrDefault(tx => tx.Id == id);
                var model = Transactions.FirstOrDefault(tx => tx.Id == id);

                if (tx is null)
                {
                    IsLoading = true;

                    var res = model is not null;

                    // Remove
                    res = Transactions.Remove(model);

                    if (res) Debug.WriteLine("[ProcessQueue] Removed tx: {txId}", id);
                    else Debug.WriteLine("[ProcessQueue] Failed to remove tx: {txId}", id);

                    continue;
                }

                // Do not add partial txs...
                // FIXME This makes partial txs pointless in HODL
                // shouldn't be that way, but, the Collection
                // is having a hard time updating them
                if (tx.Type == TxType.Partial) continue;

                IsLoading = true;

                if (model is null) model = TransactionModel.FromTransactionData(tx);

                if (Transactions.Any(x => x == model))
                {
                    var index = Transactions.IndexOf(model);

                    // Change
                    Transactions.RemoveAt(index);
                    Transactions.Insert(index, model);
                }
                else
                {
                    // Add
                    Transactions.Add(model);
                }
            }

            IsLoading = false;
            
            Transactions.Sort();

            await Task.CompletedTask;
        }

        void Wallet_OnSyncFinished(object sender, DateTimeOffset e)
        {
            var models = Transactions.ToList();

            // Check changes and removal
            foreach (var model in models)
            {
                var tx = Txs.FirstOrDefault(x => x.Id == model.Id);

                if (tx is null) // Remove
                    queue.Enqueue(model.Id);
                else if (TransactionModel.FromTransactionData(tx) != model) // Change
                    queue.Enqueue(model.Id);
            }

            // Check for new txs
            var added = 0;
            foreach (var tx in Txs.Take(models.Count))
            {
                if (!models.Any(x => x.Id == tx.Id))
                {
                    queue.Enqueue(tx.Id);

                    added++;
                }
            }

            Observable.Start(async () => await ProcessQueue(), RxApp.MainThreadScheduler);
        }

        void NavigateToTransactionDetails(object obj)
        {
            if (CurrentTransaction is null) return;

            MessagingCenter.Send(this, "NavigateToTransactionDetail", CurrentTransaction);

            CurrentTransaction = null;
        }

        void LoadTxsFromWallet(int size = -1)
        {
            IsLoading = true;

            if (size == -1) size = TXS_DEFAULT_ITEMS_SIZE;

            foreach (var tx in Txs.Take(size))
                queue.Enqueue(tx.Id);
        }

        void RemainingItemsThresholdReached(object _)
        {
            if (WalletService.Wallet is null) return;
            if (CurrentAccount is null) return;
            if (Txs is null) return;

            foreach (var tx in Txs.Skip(Transactions.Count).Take(TXS_DEFAULT_ITEMS_SIZE))
                queue.Enqueue(tx.Id);

            Observable.Start(async () => await ProcessQueue(), RxApp.MainThreadScheduler);
        }

        void Txs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
                foreach (Tx item in e.NewItems)
                {
                    queue.Enqueue(item.Id);
                }

            if (e.OldItems is not null)
                foreach (Tx item in e.OldItems)
                {
                    queue.Enqueue(item.Id);
                }

            Observable.Start(async () => await ProcessQueue(), RxApp.MainThreadScheduler);

            MessagingCenter.Send(this, "ScrollToTop");
        }
    }
}