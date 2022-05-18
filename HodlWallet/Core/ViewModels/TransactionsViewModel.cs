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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Liviano.Interfaces;
using Liviano.Events;
using Liviano.Models;
using NBitcoin;
using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Models;
using HodlWallet.UI.Extensions;

namespace HodlWallet.Core.ViewModels
{
    public partial class TransactionsViewModel : LightBaseViewModel
    {
        const int TXS_DEFAULT_ITEMS_SIZE = 10;

        const int PROCESS_QUEUE_JOB_DELAY_MS = 420;

        readonly CancellationTokenSource cts = new();

        bool isEmpty = true;

        IAccount CurrentAccount => WalletService.Wallet.CurrentAccount;

        List<Tx> Txs => CurrentAccount.Txs.OrderByDescending(tx => tx.CreatedAt).ToList();

        readonly ConcurrentQueue<uint256> queue = new();

        [ObservableProperty]
        ObservableCollection<TransactionModel> transactions = new();

        [ObservableProperty]
        TransactionModel currentTransaction;

        [ObservableProperty]
        int remainingItemsThreshold = 1;

        public TransactionsViewModel()
        {
            if (WalletService.IsStarted) Init();
            else WalletService.OnStarted += (_, _) => Init();
        }

        void Init()
        {
            SubscribeToMessages();
            SubscribeToEvents();
            LoadTxsFromWallet();
            SetupBackgroundJobs();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<WalletSettingsViewModel>(this, nameof(ClearTransactions), ClearTransactions);
        }

        void SubscribeToEvents()
        {
            CurrentAccount.Txs.CollectionChanged += Txs_CollectionChanged;
            WalletService.OnSyncFinished += Wallet_OnSyncFinished;
            WalletService.Wallet.OnNewTransaction += Wallet_OnNewTransaction;
            WalletService.Wallet.OnUpdateTransaction += Wallet_OnUpdateTransaction;
        }

        void LoadTxsFromWallet(int size = -1)
        {
            if (size == -1) size = TXS_DEFAULT_ITEMS_SIZE;

            foreach (var tx in Txs.Take(size))
                queue.Enqueue(tx.Id);
        }

        void SetupBackgroundJobs()
        {
            BackgroundService.Start("ProcessQueueJob", async () =>
            {
                Observable
                    .Interval(TimeSpan.FromMilliseconds(PROCESS_QUEUE_JOB_DELAY_MS), RxApp.TaskpoolScheduler)
                    .Subscribe(async _ => await ProcessQueue(), cts.Token);

                await Task.CompletedTask;
            });
        }

        void ClearTransactions(WalletSettingsViewModel _)
        {
            MessagingCenter.Send(this, "HideNonContentViews");
            MessagingCenter.Send(this, "ShowEmptyAfterTimeout");

            Transactions.Clear();
        }

        async Task ProcessQueue()
        {
            if (IsLoading) return;

            try
            {
                await DoProcessQueue();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ProcessQueue] {msg}", ex.Message);

                IsLoading = false;
            }
        }

        async Task DoProcessQueue()
        {
            if (queue.IsEmpty) return;

            var txs = Transactions.ToList();
            while (queue.TryDequeue(out var id))
            {
                IsLoading = true;

                var tx = Txs.FirstOrDefault(tx => tx.Id == id);
                var currentModel = txs.FirstOrDefault(tx => tx.Id == id);

                if (tx is null)
                {
                    IsLoading = true;

                    var res = currentModel is not null;

                    // Remove
                    lock (Transactions) Device.BeginInvokeOnMainThread(() => res = Transactions.Remove(currentModel));

                    if (res) Debug.WriteLine("[ProcessQueue] Removed tx: {txId}", id);
                    else Debug.WriteLine("[ProcessQueue] Failed to remove tx: {txId}", id);

                    continue;
                }

                // Do not add partial txs...
                // FIXME This makes partial txs pointless in HODL
                // shouldn't be that way, but, the Collection
                // is having a hard time updating them
                if (tx.Type == TxType.Partial) continue;

                var model = TransactionModel.FromTransactionData(tx);
                int index = currentModel is null ? -1 : txs.FindIndex(t => t.Id == currentModel.Id);
                if (currentModel is not null && index > -1)
                {
                    // Change
                    try
                    {
                        lock (Transactions) Device.BeginInvokeOnMainThread(() => Transactions[index] = model);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[DoProcessQueue] Error {msg}", ex.Message);

                        if (!queue.Contains(id)) queue.Enqueue(id);
                    }

                    txs = Transactions.ToList();
                }
                else
                {
                    try
                    {
                        // Add
                        lock (Transactions) Device.BeginInvokeOnMainThread(() => Transactions.Add(model));

                        if (isEmpty)
                        {
                            MessagingCenter.Send(this, "ShowNonContentViews");

                            isEmpty = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[DoProcessQueue] Error {msg}", ex.Message);

                        if (queue.Contains(id)) queue.Enqueue(id);
                    }

                    txs = Transactions.ToList();
                }
            }

            lock (Transactions) Device.BeginInvokeOnMainThread(() => Transactions.Sort());

            IsLoading = false;

            await Task.CompletedTask;
        }

        void Wallet_OnUpdateTransaction(object sender, TxEventArgs e)
        {
            var id = e.Tx.Id;

            if (queue.Contains(id)) return;

            queue.Enqueue(id);
        }

        void Wallet_OnNewTransaction(object sender, TxEventArgs e)
        {
            var id = e.Tx.Id;

            if (queue.Contains(id)) return;

            queue.Enqueue(id);
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
            foreach (var tx in Txs)
            {
                if (!models.Any(x => x.Id == tx.Id))
                {
                    queue.Enqueue(tx.Id);

                    added++;
                }
            }
        }

        void Txs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
                foreach (Tx item in e.NewItems)
                {
                    if (!queue.Contains(item.Id)) queue.Enqueue(item.Id);
                }

            if (e.OldItems is not null)
                foreach (Tx item in e.OldItems)
                {
                    if (!queue.Contains(item.Id)) queue.Enqueue(item.Id);
                }

            // FIXME This breaks the UI
            // MessagingCenter.Send(this, "ScrollToTop");
        }

        [ICommand]
        void NavigateToTransactionDetails(object obj)
        {
            if (CurrentTransaction is null) return;

            MessagingCenter.Send(this, "NavigateToTransactionDetail", CurrentTransaction);

            CurrentTransaction = null;
        }


        [ICommand]
        void RemainingItemsThresholdReached(object _)
        {
            if (WalletService.Wallet is null) return;
            if (CurrentAccount is null) return;
            if (Txs is null) return;
            if (Transactions.Count == Txs.Count) return;
            if (WalletService.Wallet.SyncStatus.StatusType == SyncStatusTypes.Syncing) return;

            foreach (var tx in Txs.Skip(Transactions.Count).Take(TXS_DEFAULT_ITEMS_SIZE))
                queue.Enqueue(tx.Id);
        }

        [ICommand]
        void LogDebugInfo(object obj)
        {
#if DEBUG
            Debug.Write("[LogDebugInfo] ");

            var txs = CurrentAccount.Txs.ToList();
            var count = txs.Count();

            Debug.WriteLine("Wallet transactions ({0}):", count);

            for (int i = 0; i < count; i++)
            {
                var tx = txs[i];

                Debug.WriteLine(
                    "\tTx: {0} (type: {1}, address: {2})",
                    tx.Id, tx.Type, tx.ScriptPubKey.GetDestinationAddress(CurrentAccount.Network)
                );
            }
#endif
        }

    }
}