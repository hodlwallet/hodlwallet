//
// ReceiveViewModel.cs
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
using System.Windows.Input;

using Xamarin.Forms;

namespace HodlWallet.Core.ViewModels
{
    public class ReceiveViewModel : BaseViewModel
    {
        public ICommand ShowShareIntentCommand { get; }
        public ICommand CloseCommand { get; }

        string address;
        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }

        string basicAddress;
        public string BasicAddress
        {
            get => basicAddress;
            set => SetProperty(ref basicAddress, value);
        }

        string amount;
        public string Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        bool amountIsVisible = false;
        public bool AmountIsVisible
        {
            get => amountIsVisible;
            set => SetProperty(ref amountIsVisible, value);
        }

        bool amountButtonIsVisible = true;
        public bool AmountButtonIsVisible
        {
            get => amountButtonIsVisible;
            set => SetProperty(ref amountButtonIsVisible, value);
        }

        public ReceiveViewModel()
        {
            ShowShareIntentCommand = new Command(ShowShareIntent);
            CloseCommand = new Command(Close);

            if (WalletService.IsStarted)
            {
                GetAddressFromWallet();
            }
            else
            {
                WalletService.OnStarted += WalletService_OnStarted;
            }
        }

        void Close()
        {
            AmountIsVisible = false;
            AmountButtonIsVisible = true;
            Address = BasicAddress;
        }

        private void WalletService_OnStarted(object sender, EventArgs e)
        {
            GetAddressFromWallet();
        }

        void GetAddressFromWallet()
        {
            BasicAddress = WalletService.GetReceiveAddress().ToString();
            Address = BasicAddress;

            Debug.WriteLine($"[GetAddressFromWallet] New address: {Address}");
        }

        void ShowShareIntent()
        {
            Debug.WriteLine($"[ShowShareIntent] Sharing address: {Address}");

            if (string.IsNullOrEmpty(Address))
                Debug.WriteLine("[ShowShareIntent] Sharing address is empty");
            else
                ShareIntent.QRTextShareIntent(Address);
        }
    }
}
