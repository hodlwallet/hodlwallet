//
// ReceiveView.xaml.cs
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

using Xamarin.Essentials;
using Xamarin.Forms;

using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Extensions;
using HodlWallet.UI.Locale;

namespace HodlWallet.UI.Views
{
    public partial class ReceiveView : ContentPage
    {
        ReceiveViewModel ViewModel => (ReceiveViewModel)BindingContext;

        public ReceiveView()
        {
            InitializeComponent();
        }

        void Address_Tapped(object sender, EventArgs e)
        {
            Clipboard.SetTextAsync(ViewModel.Address);

            _ = this.DisplayToast(LocaleResources.SecretContentView_textCopied);
        }

        protected override void OnAppearing()
        {
            Debug.WriteLine($"[ReceiveView][OnAppearing] Showing Address: {ViewModel.Address}");

            base.OnAppearing();
        }

        private void AddAmountButton_Clicked(object sender, EventArgs e)
        {
            ViewModel.Amount = "";
            ViewModel.AmountIsVisible = true;
            ViewModel.AmountButtonIsVisible = false;
        }

        bool HasLessEightDecimalPlaces(string stringValue)
        {
            int places = 8;
            decimal value = Convert.ToDecimal(stringValue);
            decimal step = (decimal)Math.Pow(10, places);
            if (value * step - Math.Truncate(value * step) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        string RemoveLastCharacter(string value)
        {
            return value.Remove(value.Length - 1, 1);
        }

        private void AmountEntry_Changed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.Amount))
            {
                if (Convert.ToDecimal(ViewModel.Amount) > 0 && ViewModel.AmountIsVisible)
                {
                    if (HasLessEightDecimalPlaces(ViewModel.Amount))
                    {
                        ViewModel.Address = "bitcoin:" + ViewModel.BasicAddress + "?amount=" + ViewModel.Amount;
                    }
                    else
                    {
                        ViewModel.Amount = RemoveLastCharacter(ViewModel.Amount);

                    }
                }
            }
        }
    }
}
