using System;
using System.Windows.Input;
using MvvmCross.Commands;
﻿using Xamarin.Forms;

namespace HodlWallet2.Core.Models
{
    public class Transaction
    {
        public bool? IsReceive { get; set; }
        public bool? IsSent { get; set; }
        public bool IsSpendable { get; set; }
        public bool IsComfirmed { get; set; }
        public bool? IsPropagated { get; set; }
        public int? BlockHeight { get; set; }
        public Color StatusColor { get; set; }
        public string Duration { get; set; }
        public string DateAndTime { get; set; }
        public string Amount { get; set; }
        public string Address { get; set; }
        public string AtAddress { get; set; }
        public string Memo { get; set; }
        public string Confirmations { get; set; }
        public string IsAvailable { get; set; }
        public string Id { get; set; }

        /// FIXME probably not the right place for this...
        /// Calculated properties... ummm prob against the protocol
        /// <summary>
        /// This returns the hight of the memo row
        /// if there's a memo add space for it ("15"),
        /// if there's not, then add no space ("0")
        /// </summary>
        /// <value>
        /// A <see cref="string"/> that represents
        /// the hight of the row that has the memo
        /// </value>
        public string MemoHeight
        {
            get
            {
                // Test how this works...
                //var rng = new Random();

                //bool hasAMemo = rng.Next(0, 100) % 2 == 0;

                //if (hasAMemo)
                //    Memo = "Groceries";

                if (Memo.Length == 0) return "0";

                return "15";
            }
        }
    }
}
