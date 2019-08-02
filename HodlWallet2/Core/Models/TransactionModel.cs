using System;

using Xamarin.Forms;

using NBitcoin;

using Liviano.Models;

using HodlWallet2.Core.Extensions;
using HodlWallet2.Core.Utils;
using HodlWallet2.Core.Interfaces;

namespace HodlWallet2.Core.Models
{
    public class TransactionModel
    {
        Network _Network => DependencyService.Get<IWalletService>().GetNetwork();

        public uint256 Id { get; set; }
        public string IdText { get; set; }

        public DateTimeOffset CreationTime { get; set; }
        public string CreationTimeText { get; set; }

        public Money Amount { get; set; }
        public string AmountText { get; set; }
        public string AmountWithFeeText { get; set; }

        public string Address { get; set; }
        public string AddressText { get; set; }

        public string MemoText { get; set; }

        public string AddressTitle { get; set; }

        public string StatusText { get; set; }
        public string ConfirmedBlockText { get; set; }

        public bool IsReceive { get; set; }
        public bool IsSend { get; set; }

        public bool IsSpendable { get; set; }
        public bool IsConfirmed { get; set; }

        public bool IsPropagated { get; set; }
        public int BlockHeight { get; set; }

        public string ConfirmationsText { get; set; }
        public string IsAvailableText { get; set; }

        public TransactionData TransactionData { get; set; }

        public TransactionModel() { }

        public TransactionModel(TransactionData transactionData)
        {
            TransactionData = transactionData;

            Id = TransactionData.Id;
            IdText = TransactionData.Id.ToString();

            IsSend = (bool)TransactionData.IsSend;
            IsReceive = (bool)TransactionData.IsReceive;

            CreationTime = TransactionData.CreationTime;
            CreationTimeText = TransactionData.CreationTime.ToString();

            Amount = GetAmount();
            AmountText = GetAmountText();
            AmountWithFeeText = GetAmountWithFeesText();

            Address = GetAddress();
            AddressText = GetAddressText();

            MemoText = TransactionData.Memo;

            AddressTitle = GetAddressTitleText();

            BlockHeight = TransactionData.BlockHeight ?? -1;

            IsPropagated = (bool)TransactionData.IsPropagated;
            IsSpendable = TransactionData.IsSpendable();

            StatusText = GetStatusText();
            IsAvailableText = StatusText; // TODO why?
            ConfirmedBlockText = GetConfirmedBlockText();
        }

        public static TransactionModel FromTransactionData(TransactionData transactionData)
        {
            return new TransactionModel(transactionData);
        }

        /// <summary>
        /// Check if the tx is equal to another tx by value not object equality
        /// </summary>
        /// <param name="obj">The other tx model</param>
        /// <returns>A <see cref="bool"/> that represents if they're equal or not</returns>
        public override bool Equals(object obj)
        {
            var other = obj as TransactionModel;

            if (other.Id != Id) return false;
            if (other.IsReceive != IsReceive) return false;
            if (other.IsSend != IsSend) return false;
            if (other.CreationTime != CreationTime) return false;
            if (other.IsSpendable != IsSpendable) return false;
            if (other.IsConfirmed != IsConfirmed) return false;
            if (other.IsPropagated != IsPropagated) return false;
            if (other.BlockHeight != BlockHeight) return false;
            if (other.Amount != Amount) return false;
            if (other.Address != Address) return false;
            if (other.MemoText != MemoText) return false;
            if (other.ConfirmationsText != ConfirmationsText) return false;
            if (other.IsAvailableText != IsAvailableText) return false;

            return true;
        }

        public static bool operator ==(TransactionModel x, TransactionModel y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(TransactionModel x, TransactionModel y)
        {
            return !(x == y);
        }

        string GetConfirmedBlockText()
        {
            var blockHeight = TransactionData.BlockHeight;

            return blockHeight is null
                ? "Awaiting Confirmation"
                : blockHeight.ToString();
        }

        string GetStatusText()
        {
            return TransactionData.IsConfirmed()
                ? "Confirmed"
                : "Awaiting Confirmation";
        }

        string GetAddress()
        {
            return TransactionData.IsSend == true
                ? TransactionData.SentToScriptPubKey.GetDestinationAddress(_Network).ToString()
                : TransactionData.ScriptPubKey.GetDestinationAddress(_Network).ToString();
        }

        string GetAddressText()
        {
            return TransactionData.IsSend == true
                ? $"To: {Address}"
                : $"At: {Address}";
        }

        Money GetAmount()
        {
            return TransactionData.IsSend == true
                ? TransactionData.AmountSent
                : TransactionData.Amount;
        }

        string GetAmountText()
        {
            decimal decimalAmount = Amount.ToDecimal(MoneyUnit.BTC);
            string amountStr = decimalAmount.Normalize().ToString();

            if (TransactionData.IsSend == true)
                amountStr = $"-{amountStr}";

            return amountStr;
        }

        string GetAmountWithFeesText()
        {
            if (TransactionData.TotalFees == Money.Zero)
                return AmountText;

            var totalAmount = TransactionData.TotalAmount ?? Money.Zero;
            var totalFees = TransactionData.TotalFees ?? Money.Zero;

            if (totalAmount == Money.Zero || totalFees == Money.Zero)
                return AmountText;

            string totalWithFees = (totalAmount + totalFees).Normalize().ToString();

            return $"${totalWithFees} ({Amount.Normalize().ToString()} + {totalFees})";
        }


        string GetAddressTitleText()
        {
            if (TransactionData.IsSend == true)
                return Constants.TRANSACTION_DETAILS_SENT_ADDRESS_TITLE;

            return Constants.TRANSACTION_DETAILS_RECEIVED_ADDRESS_TITLE;
        }
    }
}
