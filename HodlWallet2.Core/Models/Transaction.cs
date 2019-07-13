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
        public bool IsConfirmed { get; set; }
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
                if (Memo == null) return "0";
                if (Memo.Length == 0) return "0";

                return "15";
            }
        }

        /// <summary>
        /// Check if the tx is equal to another tx by value not object equality
        /// </summary>
        /// <param name="obj">The other tx model</param>
        /// <returns>A <see cref="bool"/> that represents if they're equal or not</returns>
        public override bool Equals(object obj)
        {
            var other = obj as Transaction;

            if (other.Id != Id) return false;
            if (other.IsReceive != IsReceive) return false;
            if (other.IsSent != IsSent) return false;
            if (other.IsSpendable != IsSpendable) return false;
            if (other.IsConfirmed != IsConfirmed) return false;
            if (other.IsPropagated != IsPropagated) return false;
            if (other.BlockHeight != BlockHeight) return false;
            if (other.StatusColor != StatusColor) return false;
            if (other.Duration != Duration) return false;
            if (other.DateAndTime != DateAndTime) return false;
            if (other.Amount != Amount) return false;
            if (other.Address != Address) return false;
            if (other.AtAddress != AtAddress) return false;
            if (other.Memo != Memo) return false;
            if (other.Confirmations != Confirmations) return false;
            if (other.IsAvailable != IsAvailable) return false;

            return true;
        }

        public static bool operator ==(Transaction x, Transaction y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Transaction x, Transaction y)
        {
            return !(x == y);
        }

    }
}
