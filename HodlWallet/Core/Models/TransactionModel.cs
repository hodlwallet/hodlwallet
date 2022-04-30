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
using System.Reactive.Linq;
using System.Threading;

using Liviano.Models;
using NBitcoin;
using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Interfaces;
using HodlWallet.Core.Services;
using HodlWallet.UI.Locale;

namespace HodlWallet.Core.Models
{
    public class TransactionModel : BindableModel
    {
        Network Network => DependencyService.Get<IWalletService>().GetNetwork();

        IPrecioService PrecioService => DependencyService.Get<IPrecioService>();

        IDisplayCurrencyService DisplayCurrencyService => DependencyService.Get<IDisplayCurrencyService>();

        readonly CancellationTokenSource cts = new();

        public uint256 Id { get; set; }
        public string IdText { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string CreatedAtText { get; set; }

        public Money Amount { get; set; }

        string amountText;
        public string AmountText
        {
            get => amountText;
            set => SetProperty(ref amountText, value);
        }

        public string AmountWithFeeText { get; set; }

        public string Preposition { get; set; }

        public string Address { get; set; }
        public string AddressText { get; set; }

        string note;
        public string Note
        {
            get => note;
            set
            {
                if (value == note) return;
                if (note is null && value is null) return;

                SetProperty(ref note, value);
                SecureStorageService.SetNote(IdText, note);
            }
        }

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

        public TransactionModel(Tx transactionData)
        {
            TransactionData = transactionData;

            Id = TransactionData.Id;
            IdText = TransactionData.Id.ToString();

            IsSend = TransactionData.Type == TxType.Send;
            IsReceive = TransactionData.Type == TxType.Receive;

            Preposition = IsSend ? LocaleResources.TransactionDetails_to : LocaleResources.TransactionDetails_from;

            CreatedAt = TransactionData.CreatedAt;
            CreatedAtText = TransactionData.CreatedAt.ToString();

            Amount = GetAmount();

            AmountText = GetAmountText();

            AmountWithFeeText = GetAmountWithFeesText();

            Address = GetAddress();
            AddressText = GetAddressText();

            Note = GetNote();

            AddressTitle = GetAddressTitleText();

            BlockHeight = (int)(TransactionData.Height);

            IsSpendable = true;

            StatusText = GetStatusText();
            IsAvailableText = StatusText; // TODO why?
            ConfirmedBlockText = GetConfirmedBlockText();

            PrecioService
                .WhenAnyValue(service => service.Rates, service => service.Precio)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(_ => UpdateAmountText(), cts.Token);

            DisplayCurrencyService
                .WhenAnyValue(service => service.CurrencyType, service => service.FiatCurrencyCode)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(_ => UpdateAmountText(), cts.Token);
        }

        string GetNote()
        {
            return SecureStorageService.GetNote(IdText);
        }

        void UpdateAmountText()
        {
            var amount = GetAmountText();

            // This prevents updates that aren't not needed
            if (amount != AmountText)
                AmountText = amount;
        }

        string GetAmountText()
        {
            var amount = Amount ?? Money.Zero;
            if (DisplayCurrencyService.CurrencyType == DisplayCurrencyType.Bitcoin)
                return DisplayCurrencyService.BitcoinAmountFormatted(amount.ToDecimal(MoneyUnit.BTC), IsSend);
            return DisplayCurrencyService.FiatAmountFormatted(amount.ToDecimal(MoneyUnit.BTC), IsSend);
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
            if (other.Note != Note) return false;
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
            var blockHeight = TransactionData.Height;

            return blockHeight <= 0
                ? LocaleResources.Transactions_awaitingConfirmation
                : blockHeight.ToString();
        }

        string GetStatusText()
        {
            return TransactionData.Height > 0
                ? LocaleResources.Transactions_confirmed
                : LocaleResources.Transactions_awaitingConfirmation;
        }

        string GetAddress()
        {
            try
            {
                return TransactionData.ScriptPubKey.GetDestinationAddress(Network).ToString();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[GetAddress] Error: {e.Message}");
            }

            return string.Empty;
        }

        string GetAddressText()
        {
            var preposition = TransactionData.Type == TxType.Send ? LocaleResources.Transactions_isSendTo : LocaleResources.Transactions_isSendAt;

            return $"{preposition} {Address}";
        }

        Money GetAmount()
        {
            return TransactionData.Amount;
        }

        string GetAmountWithFeesText()
        {
            if (TransactionData.Fees == Money.Zero)
                return AmountText;

            var totalAmount = TransactionData.Amount ?? Money.Zero;
            var totalFees = TransactionData.Fees ?? Money.Zero;

            if (totalAmount == Money.Zero || totalFees == Money.Zero)
                return AmountText;

            var totalWithFees = totalAmount + totalFees;

            return DisplayCurrencyService.BitcoinAmountFormatted(totalWithFees.ToDecimal(MoneyUnit.BTC), IsSend);
        }

        string GetAddressTitleText()
        {
            if (TransactionData.Type == TxType.Send)
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
