//
// AmountEntry.xaml.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
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
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Linq;

using NBitcoin;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ReactiveUI;

using HodlWallet.Core.Services;
using HodlWallet.Core.ViewModels;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AmountEntry : Entry
    {
        public AmountEntryViewModel ViewModel => BindingContext as AmountEntryViewModel;

        Color FgSuccess => (Color)Application.Current.Resources["FgSuccess"];
        Color FgError => (Color)Application.Current.Resources["FgError"];

        public event PropertyChangedEventHandler AmountChanged = (e, a) => { };

        bool showBalanceAnimation;
        public bool ShowBalanceAnimation
        {
            get { return showBalanceAnimation; }
            set
            {
                showBalanceAnimation = value;
                OnPropertyChanged(nameof(ShowBalanceAnimation));
            }
        }

        public static readonly BindableProperty AddressToSendProperty = BindableProperty.Create(
            nameof(AddressToSend),
            typeof(string),
            typeof(AmountEntry),
            string.Empty
        );
        public string AddressToSend
        {
            get { return (string)GetValue(AddressToSendProperty); }
            set { SetValue(AddressToSendProperty, value); }
        }

        public static readonly BindableProperty FeeProperty = BindableProperty.Create(
            nameof(Fee),
            typeof(decimal),
            typeof(AmountEntry),
            1.0m
        );
        public decimal Fee
        {
            get { return (decimal)GetValue(FeeProperty); }
            set { SetValue(FeeProperty, value); }
        }

        public AmountEntry()
        {
            InitializeComponent();
            SubscribeToMessages();
        }

        public string GetBitcoinAmountFormatted()
        {
            return ViewModel.Amount.ToDecimal(MoneyUnit.BTC).ToString("0.########");
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<AmountEntryViewModel>(this, "ShowBalanceError", ShowBalanceError);
            MessagingCenter.Subscribe<AmountEntryViewModel>(this, "ShowBalanceSuccess", ShowBalanceSuccess);
        }

        async void AnimateControl()
        {
            uint timeout = 50;

            await this.TranslateTo(-5, 0, timeout);
            await this.TranslateTo(5, 0, timeout);
            await this.TranslateTo(-4, 0, timeout);
            await this.TranslateTo(4, 0, timeout);
            await this.TranslateTo(-3, 0, timeout);
            await this.TranslateTo(3, 0, timeout);
            await this.TranslateTo(-2, 0, timeout);
            await this.TranslateTo(2, 0, timeout);
            await this.TranslateTo(-1, 0, timeout);
            await this.TranslateTo(1, 0, timeout);

            TranslationX = 0;
        }

        void ShowBalanceError(AmountEntryViewModel _)
        {
            if (TextColor == FgError) return;

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                TextColor = FgError;
                AnimateControl();
            });
        }

        void ShowBalanceSuccess(AmountEntryViewModel _)
        {
            if (TextColor == FgSuccess) return;

            RxApp.MainThreadScheduler.Schedule(() => TextColor = FgSuccess);
        }

        void AmountEntry_Changed(object sender, PropertyChangedEventArgs e)
        {
            var entry = (AmountEntry)sender;
            var text = entry.Text;

            if (string.IsNullOrEmpty(text)) return;

            if (text.Equals("."))
                entry.Text = $"{ViewModel.CurrencySymbol}0.";
            else if (text.Equals(ViewModel.CurrencySymbol))
                entry.Text = string.Empty;
            else
                entry.Text = NormalizeText(text);

            AmountChanged?.Invoke(entry, e);
        }

        string NormalizeText(string text)
        {
            var amount = text.Split(ViewModel.CurrencySymbol, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            if (string.IsNullOrEmpty(amount) || !decimal.TryParse(amount, out _)) return text;

            var symbol = ViewModel.CurrencySymbol;

            string finalText;
            if (ViewModel.DisplayCurrencyService.CurrencyType == DisplayCurrencyType.Bitcoin)
            {
                while (GetDecimalPlaces(amount) > 8) amount = amount[..^1];

                finalText = $"{symbol}{amount:0.########}";
            }
            else
            {
                while (GetDecimalPlaces(amount) > 2) amount = amount[..^1];

                finalText = $"{symbol}{amount:0.##}";
            }

            return finalText;
        }

        int GetDecimalPlaces(string amountStr)
        {
            var split = amountStr.Split(".");

            if (split.Length != 2) return 0;

            return split.Last().Length;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(ShowBalanceAnimation):
                    if (ShowBalanceAnimation) ViewModel.TrackBalance();
                    break;
                case nameof(AddressToSend):
                    ViewModel.AddressToSend = AddressToSend;
                    break;
                case nameof(Fee):
                    ViewModel.Fee = Fee;
                    break;
            }
        }

    }
}