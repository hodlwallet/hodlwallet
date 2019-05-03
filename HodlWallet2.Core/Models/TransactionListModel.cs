using System;
using System.Collections.Generic;
using Liviano.Models;

namespace HodlWallet2.Core.Models
{
    public class TransactionListModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }

        public TransactionListModel(IEnumerable<TransactionData> txList)
        {
            // TODO: Incorporate Wallet Service and MVVMCross when complete.
            Transactions = CreateList(txList);
        }

        private IEnumerable<Transaction> CreateList(IEnumerable<TransactionData> txList)
        {
            List<Transaction> result = new List<Transaction>();
            // TODO: Make sure to only add new transactions to the existing list (Async?).
            foreach (var tx in txList)
            {
                result.Add(new Transaction
                {
                    IsReceive = (bool)tx.IsReceive,
                    IsSent = (bool)tx.IsSend,
                    IsSpendable = tx.IsSpendable(),
                    IsComfirmed = tx.IsConfirmed(),
                    IsPropagated = (bool)tx.IsPropagated,
                    BlockHeight = (int)tx.BlockHeight,
                    IsAvailable = tx.IsSpendable() ? "Available to spend" : "",
                    Memo = "In Progress",
                    Status = tx.Amount.ToString()

                    // TODO: Add data for transaction model.
                });
            }
            return result;
        }
    }
}
