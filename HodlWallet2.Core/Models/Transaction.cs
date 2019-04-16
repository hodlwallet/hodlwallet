using System;

namespace HodlWallet2.Core.Models
{
    public class Transaction
    {
        public bool IsReceived { get; set; }
        public string Duration { get; set; }
        public string Status { get; set; }
        public string AtAddress { get; set; }
        public string Memo { get; set; }
        public string Confirmations { get; set; }
        public string IsAvailable { get; set; }
    }
}
