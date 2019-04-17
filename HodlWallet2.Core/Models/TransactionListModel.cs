using System;
using System.Collections.Generic;

namespace HodlWallet2.Core.Models
{
    public class TransactionListModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }

        public TransactionListModel()
        {
            // TODO: Incorporate Wallet Service and MVVMCross when complete.
        }
    }
}
