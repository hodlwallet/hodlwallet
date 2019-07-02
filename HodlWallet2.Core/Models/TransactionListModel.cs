using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Liviano.Models;

namespace HodlWallet2.Core.Models
{
    public class TransactionListModel
    {
        public ObservableCollection<Transaction> TransactionList { get; set; }

        public TransactionListModel(ObservableCollection<TransactionData> txList)
        {
            // TODO: Incorporate Wallet Service and MVVMCross when complete.
            TransactionList = CreateList(txList);
        }

        public ObservableCollection<Transaction> CreateList(ObservableCollection<TransactionData> txList)
        {
            ObservableCollection<Transaction> result = new ObservableCollection<Transaction>();
            // TODO: Make sure to only add new transactions to the existing list (Async?).
            foreach (var tx in txList)
            {
                result.Add(new Transaction
                {
                    IsReceive = tx.IsReceive,
                    IsSent = tx.IsSend,
                    IsSpendable = tx.IsSpendable(),
                    IsComfirmed = tx.IsConfirmed(),
                    IsPropagated = tx.IsPropagated,
                    BlockHeight = tx.BlockHeight,
                    IsAvailable = tx.IsSpendable() ? "Available to spend" : "",
                    Memo = "",
                    Amount = tx.Amount.ToString()

                    // TODO: Add data for transaction model.
                });
            }
            return result;
        }
    }
}
