using System;
using System.Collections.Generic;

using HodlWallet2.Core.Services;

namespace HodlWallet2.Core.Models
{
    public class TransactionListModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }

        public TransactionListModel(WalletService Instance)
        {
            // TODO: Incorporate Wallet Service and MVVMCross when complete.
            Transactions = CreateList(Instance);
        }

        private IEnumerable<Transaction> CreateList(WalletService Instance)
        {
            List<Transaction> result = new List<Transaction>();
            // TODO: Make sure to only add new transactions to the existing list (Async?).
            foreach (var tx in Instance.GetCurrentAccountTransactions())
            {
                result.Add(new Transaction
                {
                    IsReceive = (bool)tx.IsReceive,
                    IsSent = (bool)tx.IsSend,
                    IsSpendable = tx.IsSpendable(),
                    IsComfirmed = tx.IsConfirmed(),
                    IsPropagated = (bool)tx.IsPropagated,
                    BlockHeight = (int)tx.BlockHeight
                    // TODO: Add data for transaction model.
                });
            }
            return result;
        }
    }
}
