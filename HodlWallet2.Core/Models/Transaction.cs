using System;

namespace HodlWallet2.Core.Models
{
    // TODO: Populate with TransactionData from Liviano.
    public class Transaction
    {
        public bool? IsReceive { get; set; }
        public bool? IsSent { get; set; }
        public bool IsSpendable { get; set; }
        public bool IsComfirmed { get; set; }
        public bool? IsPropagated { get; set; }
        public int? BlockHeight { get; set; }
        public Xamarin.Forms.Color StatusColor { get; set; }
        public string Duration { get; set; }
        public string AmountStatus { get; set; }
        public string AtAddress { get; set; }
        public string Memo { get; set; }
        public string Confirmations { get; set; }
        public string IsAvailable { get; set; }
        public string TXID { get; set; }

        public override string ToString()
        {
            return TXID;
        }
    }
}
