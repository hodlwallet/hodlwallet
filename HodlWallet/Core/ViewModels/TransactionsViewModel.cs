//
// TransactionsViewModel.cs
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
using HodlWallet.Core.Models;
using Liviano.Interfaces;
using Liviano.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace HodlWallet.Core.ViewModels
{
    class TransactionsViewModel : BaseViewModel
    {
        public ObservableCollection<TransactionModel> Transactions { get; } = new ObservableCollection<TransactionModel>();

        public ICommand NavigateToTransactionDetailsCommand { get; }

        TransactionModel currentTransaction;
        public TransactionModel CurrentTransaction
        {
            get => currentTransaction;
            set => SetProperty(ref currentTransaction, value);
        }

        IAccount CurrentAccount => WalletService.Wallet.CurrentAccount;

        public TransactionsViewModel()
        {
            NavigateToTransactionDetailsCommand = new Command(NavigateToTransactionDetails);

            if (WalletService.IsStarted) Init();
            else WalletService.OnStarted += (_, _) => Init();
        }

        void Init()
        {
            LoadTxsFromWallet();

            CurrentAccount.Txs.CollectionChanged += Txs_CollectionChanged;
        }

        void NavigateToTransactionDetails(object obj)
        {
            if (CurrentTransaction is null) return;

            MessagingCenter.Send(this, "NavigateToTransactionDetail", CurrentTransaction);

            CurrentTransaction = null;
        }

        void LoadTxsFromWallet()
        {
            var txs = CurrentAccount.Txs.ToList().Where(tx =>
            {
                // FIXME this is a bug on the abandon abandon about mnemonic
                // this code should not be used if the bug is fixed on Liviano
                if (tx.IsSend) return tx.SentScriptPubKey is not null;
                else return tx.ScriptPubKey is not null;
            }).OrderByDescending(tx => tx.CreatedAt);
            //var txs = CurrentAccount.Txs.ToList().OrderByDescending(tx => tx.CreatedAt);

            foreach (var tx in txs) Transactions.Add(TransactionModel.FromTransactionData(tx));
        }

        void Txs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
                foreach (Tx item in e.NewItems)
                {
                    // FIXME this is a bug on the abandon abandon about mnemonic
                    // this code should not be used if the bug is fixed on Liviano
                    if (item.IsSend && item.SentScriptPubKey is null) continue;
                    else if (item.IsReceive && item.ScriptPubKey is null) continue;

                    Transactions.Insert(0, TransactionModel.FromTransactionData(item));
                }

            if (e.OldItems is not null)
                foreach (Tx item in e.OldItems)
                    Transactions.Remove(TransactionModel.FromTransactionData(item));
        }
    }
}