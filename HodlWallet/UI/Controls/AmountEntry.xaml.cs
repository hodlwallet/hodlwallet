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
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HodlWallet.Core.Services;
using HodlWallet.Core.ViewModels;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AmountEntry : Entry
    {
        public AmountEntryViewModel ViewModel => BindingContext as AmountEntryViewModel;

        public event PropertyChangedEventHandler AmountChanged = (e, a) => { };

        public AmountEntry()
        {
            InitializeComponent();
        }

        void AmountEntry_Changed(object sender, PropertyChangedEventArgs e)
        {
            var entry = (AmountEntry)sender;
            var text = entry.Text;

            if (string.IsNullOrEmpty(text)) return;

            entry.Text = NormalizeText(text);

            AmountChanged?.Invoke(sender, e);
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
    }
}