using System;
using NBitcoin;

namespace HodlWallet2.Core.Interactions
{
    public class SendTransactionQuestion
    {
        public Transaction TransactionToSend { get; set; }
        public Money Fees { get; set; }
        public Action<bool> AnswerCallback { get; set; }
    }
}
