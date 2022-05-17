//
// AmountEntryViewModel.cs
//
// Copyright (c) 2022 HODL Wallet
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
using System.Linq;
using System.Reactive.Linq;

using NBitcoin;
using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Services;
using HodlWallet.Core.Utils;

namespace HodlWallet.Core.ViewModels
{
    public class AmountEntryViewModel : BaseViewModel
    {
        string currencySymbol = "₿";
        public string CurrencySymbol
        {
            get { return currencySymbol; }
            set { SetProperty(ref currencySymbol, value); }
        }

        string placeholderAmount = "0.00";
        public string PlaceholderAmount
        {
            get { return placeholderAmount; }
            set { SetProperty(ref placeholderAmount, value); }
        }

        string amountText = "";
        public string AmountText
        {
            get { return amountText; }
            set
            {
                try
                {
                    value = NormalizeAmountText(value);

                    SetProperty(ref amountText, value);

                    Observable.Start(() =>
                    {
                        UpdateAmount();
                        ValidateWithBalance();
                    }, RxApp.TaskpoolScheduler);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"[AmountText_set] Error: {e}");
                }
            }
        }

        Money amount = Money.Zero;
        public Money Amount
        {
            get { return amount; }
            set { SetProperty(ref amount, value); }
        }

        bool trackBalance = false;

        Money balance = Money.Zero;
        public Money Balance
        {
            get { return balance; }
            set { SetProperty(ref balance, value); }
        }

        string addressToSend;
        public string AddressToSend
        {
            get { return addressToSend; }
            set
            {
                SetProperty(ref addressToSend, value);
                ValidateWithBalance();
            }
        }

        decimal fee = 1;
        public decimal Fee
        {
            get { return fee; }
            set
            {
                SetProperty(ref fee, value);
                ValidateWithBalance();
            }
        }

        internal void TrackBalance()
        {
            if (WalletService.IsStarted) DoTrackBalance();
            else WalletService.OnStarted += (_, _) => DoTrackBalance();
        }

        void DoTrackBalance()
        {
            var acc = WalletService.Wallet.CurrentAccount;
            Balance = acc.GetBalance();

            trackBalance = true;

            acc.Txs.CollectionChanged += (_, _) =>
            {
                Balance = acc.GetBalance();
            };
        }

        void ValidateWithBalance()
        {
            if (!trackBalance) return;

            if (Amount > Balance)
            {
                MessagingCenter.Send(this, "ShowBalanceError");
                return;
            }

            var success = false;
            var fees = 1m;
            if (!string.IsNullOrEmpty(addressToSend))
            {
                try
                {
                    (success, _, fees, _) = WalletService.CreateTransaction(
                        Amount.ToDecimal(MoneyUnit.BTC), AddressToSend, Fee, string.Empty
                    );
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ValidateWithBalance] Error: {ex}");

                    fees = 144 * Fee;
                    success = true;
                }
            }

            if (!success)
            {
                if (Amount > Balance)
                    MessagingCenter.Send(this, "ShowBalanceError");
                else
                    MessagingCenter.Send(this, "ShowBalanceSuccess");

                return;
            }

            if ((Amount + new Money(fees, MoneyUnit.Satoshi)) > Balance)
                MessagingCenter.Send(this, "ShowBalanceError");
            else
                MessagingCenter.Send(this, "ShowBalanceSuccess");
        }

        public AmountEntryViewModel()
        {
            if (WalletService.IsStarted) Setup();
            else WalletService.OnStarted += (_, _) => Setup();
        }

        void Setup()
        {
            DisplayCurrencyService
                .WhenAnyValue(service => service.CurrencyType)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateCurrency(DisplayCurrencyService.FiatCurrencyCode));

            DisplayCurrencyService
                .WhenAnyValue(service => service.FiatCurrencyCode)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(cc => UpdateCurrency(cc));
        }

        void UpdateAmount()
        {
            if (string.IsNullOrEmpty(AmountText))
            {
                Amount = Money.Zero;

                return;
            }

            var value = AmountText.Split(CurrencySymbol).Last();
            if (DisplayCurrencyService.CurrencyType == DisplayCurrencyType.Bitcoin)
            {
                Amount = Money.Parse(value);

                return;
            }

            var finalAmount = decimal.Parse(value) / GetRate();

            Amount = new Money(finalAmount, MoneyUnit.BTC);
        }

        void UpdateCurrency(string cc)
        {
            if (string.IsNullOrEmpty(AmountText))
            {
                if (DisplayCurrencyService.CurrencyType == DisplayCurrencyType.Bitcoin)
                {
                    CurrencySymbol = "₿";
                    PlaceholderAmount = "0.00000000";
                }
                else
                {
                    CurrencySymbol = Constants.CURRENCY_SYMBOLS[cc];
                    PlaceholderAmount = "0.00";
                }

                return;
            }

            var rate = GetRate();
            string amount;
            if (DisplayCurrencyService.CurrencyType == DisplayCurrencyType.Bitcoin)
            {
                var symbol = Constants.CURRENCY_SYMBOLS[cc];
                amount = AmountText.Replace(symbol, string.Empty);
            }
            else
            {
                amount = AmountText[1..];
            }

            if (decimal.TryParse(amount, out var amountDecimal))
            {
                if (DisplayCurrencyService.CurrencyType == DisplayCurrencyType.Bitcoin)
                {
                    CurrencySymbol = "₿";
                    PlaceholderAmount = "0.00000000";
                    AmountText = (amountDecimal / rate).ToString("0.########");
                }
                else
                {
                    CurrencySymbol = Constants.CURRENCY_SYMBOLS[cc];
                    PlaceholderAmount = "0.00";
                    AmountText = (amountDecimal * rate).ToString("0.##");
                }
            }
        }

        string NormalizeAmountText(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Equals(CurrencySymbol))
                return string.Empty;
            else if (value.Equals("."))
                return $"{CurrencySymbol}0.";
            else if (value.EndsWith("."))
                return value;
            else if (!value.StartsWith(CurrencySymbol))
                value = $"{CurrencySymbol}{value}";

            return value;
        }

        decimal GetRate()
        {
            decimal rate;
            if (DisplayCurrencyService.FiatCurrencyCode == "USD")
                rate = decimal.Parse(PrecioService.Precio.CRaw);
            else
                rate = (decimal)PrecioService.Rates.FirstOrDefault(r => r.Code == DisplayCurrencyService.FiatCurrencyCode).Rate;

            return rate;
        }
    }
}