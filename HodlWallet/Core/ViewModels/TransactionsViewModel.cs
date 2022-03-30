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
using System.Reactive.Linq;
using System.Windows.Input;

using Liviano.Interfaces;
using Liviano.Models;
using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Models;

namespace HodlWallet.Core.ViewModels
{
    class TransactionsViewModel : BaseViewModel
    {
        public ObservableCollection<TransactionModel> Transactions { get; } = new ObservableCollection<TransactionModel>();

        const int TXS_ITEMS_SIZE = 30;

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

        //List<Tx> Txs => CurrentAccount.Txs.Where(tx =>
        //{
        //    // FIXME this is a bug on the abandon abandon about mnemonic
        //    // this code should not be used if the bug is fixed on Liviano
        //    return tx.ScriptPubKey is not null || tx.SentScriptPubKey is not null;
        //}).OrderByDescending(tx => tx.CreatedAt).ToList();

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

            WalletService.Wallet.OnNewTransaction += Wallet_OnNewTransaction;
            CurrentAccount.Txs.CollectionChanged += Txs_CollectionChanged;
        }

        void Wallet_OnNewTransaction(object sender, Liviano.Events.TxEventArgs e)
        {
            Debug.WriteLine($"New transaction on wallet {e.Tx.Id}");
        }

        void NavigateToTransactionDetails(object obj)
        {
            if (CurrentTransaction is null) return;

            MessagingCenter.Send(this, "NavigateToTransactionDetail", CurrentTransaction);

            CurrentTransaction = null;
        }

        void LoadTxsFromWallet()
        {
            IsLoading = true;

            foreach (var tx in Txs.Take(TXS_ITEMS_SIZE))
            {
                var txModel = TransactionModel.FromTransactionData(tx);
                TransactionsAddInPlace(txModel);
            }

            MessagingCenter.Send(this, "ScrollToTop");

            IsLoading = false;
        }

        void RemainingItemsThresholdReached(object _)
        {
            if (IsLoading) return;

            IsLoading = true;

            foreach (var tx in Txs.Skip(Transactions.Count).Take(TXS_ITEMS_SIZE))
            {
                var txModel = TransactionModel.FromTransactionData(tx);
                TransactionsAddInPlace(txModel, expand: true);
            }

            IsLoading = false;
        }

        void Txs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
                foreach (Tx item in e.NewItems)
                {
                    // FIXME this is a bug on the abandon abandon about mnemonic
                    // this code should not be used if the bug is fixed on Liviano
                    //if (item.SentScriptPubKey is null && item.ScriptPubKey is null) continue;

                    var txModel = TransactionModel.FromTransactionData(item);
                    TransactionsAddInPlace(txModel);
                }

            if (e.OldItems is not null)
                foreach (Tx item in e.OldItems)
                {
                    var txModel = TransactionModel.FromTransactionData(item);
                    TransactionsAddInPlace(txModel);
                }

            MessagingCenter.Send(this, "ScrollToTop");
        }

        /// <summary>
        /// Tries to position a transaction in the right place
        /// depending on the CreatedAt attribute of the model
        /// </summary>
        /// <param name="model">A model of a tx</param>
        /// <param name="expand">if we need to add more or not</param>
        void TransactionsAddInPlace(TransactionModel model, bool expand = false)
        {
            if (Transactions.Contains(model)) return;

            // TODO Remove this code, since it should never
            // be the case that a transaction doesn't have any
            // address to or from... is a bug on Liviano
            //if (string.IsNullOrEmpty(txModel.Address))
            //{
            //    txModel.IsSend = !txModel.IsSend;
            //    txModel.IsReceive = !txModel.IsSend;

            //    if (string.IsNullOrEmpty(txModel.Address))
            //    {
            //        return;
            //    }
            //}
            // Remove ^^

            var newerTxs = Transactions
                .ToList()
                .Where(tx => tx.CreatedAt > model.CreatedAt)
                .OrderByDescending(tx => tx.CreatedAt);

            int index;
            if (newerTxs.Any())
                index = Transactions.IndexOf(newerTxs.FirstOrDefault());
            else index = 0;

            Observable
                .Start(() =>
                {
                    Transactions.Insert(index, model);

                    if (expand) return;
                    if (Transactions.Count < TXS_ITEMS_SIZE) return;

                    Transactions.RemoveAt(Transactions.Count);
                }, RxApp.TaskpoolScheduler);
        }
    }
}