//
// TransactionModel.cs
//
// Copyright (c) 2019 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Diagnostics;

using Liviano.Models;
using NBitcoin;
using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Extensions;
using HodlWallet.Core.Interfaces;
using HodlWallet.UI.Locale;

namespace HodlWallet.Core.Models
{
    public class TransactionModel : ReactiveObject
    {
        Network Network => DependencyService.Get<IWalletService>().GetNetwork();

        public uint256 Id { get; set; }
        public string IdText { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string CreatedAtText { get; set; }

        public Money Amount { get; set; }

        string amountText;
        public string AmountText
        {
            get => amountText;
            set
            {
                this.RaiseAndSetIfChanged(ref amountText, value);
            }
        }

        public Money AmountFiat { get; set; }

        string amountFiatText;
        public string AmountFiatText
        {
            get => amountFiatText;
            set
            {
                this.RaiseAndSetIfChanged(ref amountFiatText, value);
            }
        }

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

        public int BlockHeight { get; set; }

        public string ConfirmationsText { get; set; }
        public string IsAvailableText { get; set; }

        public Tx TransactionData { get; set; }

        public TransactionModel() { }

        public TransactionModel(Tx transactionData)
        {
            TransactionData = transactionData;

            Id = TransactionData.Id;
            IdText = TransactionData.Id.ToString();

            IsSend = (bool)TransactionData.IsSend;
            IsReceive = (bool)TransactionData.IsReceive;

            CreatedAt = TransactionData.CreatedAt.GetValueOrDefault();
            CreatedAtText = TransactionData.CreatedAt.ToString();

            Amount = GetAmount();
            AmountText = GetAmountText();
            AmountWithFeeText = GetAmountWithFeesText();

            Address = GetAddress();
            AddressText = GetAddressText();

            MemoText = TransactionData.Memo;

            AddressTitle = GetAddressTitleText();

            BlockHeight = (int)(TransactionData.BlockHeight ?? -1);

            IsSpendable = TransactionData.IsSpendable();

            StatusText = GetStatusText();
            IsAvailableText = StatusText; // TODO why?
            ConfirmedBlockText = GetConfirmedBlockText();
        }

        public static TransactionModel FromTransactionData(Tx transactionData)
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
            if (other.CreatedAt != CreatedAt) return false;
            if (other.IsSpendable != IsSpendable) return false;
            if (other.IsConfirmed != IsConfirmed) return false;
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
                ? LocaleResources.Transactions_awaitingConfirmation
                : blockHeight.ToString();
        }

        string GetStatusText()
        {
            return TransactionData.IsConfirmed()
                ? LocaleResources.Transactions_confirmed
                : LocaleResources.Transactions_awaitingConfirmation;
        }

        string GetAddress()
        {
            try
            {
                return TransactionData.IsSend
                    ? TransactionData.SentScriptPubKey.GetDestinationAddress(Network).ToString()
                    : TransactionData.ScriptPubKey.GetDestinationAddress(Network).ToString();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[GetAddress] Error: {e.Message}");
            }

            return string.Empty;
        }

        string GetAddressText()
        {
            return TransactionData.IsSend
                ? LocaleResources.Transactions_isSendTo + $"{Address}"
                : LocaleResources.Transactions_isSendAt + $"{Address}";
        }

        Money GetAmount()
        {
            return TransactionData.IsSend
                ? TransactionData.AmountSent
                : TransactionData.AmountReceived;
        }

        string GetAmountText()
        {
            decimal decimalAmount = Amount.ToDecimal(MoneyUnit.BTC);
            string amountStr = decimalAmount.Normalize().ToString();

            if (TransactionData.IsSend)
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

            return $"{totalWithFees} ({Amount.Normalize()} + {totalFees})";
        }

        string GetAddressTitleText()
        {
            if (TransactionData.IsSend)
                return LocaleResources.TransactionDetails_sendAddressTitle;

            return LocaleResources.TransactionDetails_receivedAddressTitle;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
