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
using System.Linq;
using System.Windows.Input;

using Xamarin.Forms;
using NBitcoin;
using Xamarin.Essentials;

namespace HodlWallet.Core.ViewModels
{
    public class ReceiveViewModel : BaseViewModel
    {
        public ICommand ShareIntentCommand { get; }
        public ICommand AddAmountCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CopyAddressCommand { get; }

        string addressFormatted;
        public string AddressFormatted
        {
            get => addressFormatted;
            set => SetProperty(ref addressFormatted, value);
        }

        public BitcoinAddress Address { get; set; }

        string amount;
        public string Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        bool amountFrameIsVisible = false;
        public bool AmountFrameIsVisible
        {
            get => amountFrameIsVisible;
            set => SetProperty(ref amountFrameIsVisible, value);
        }

        bool clearIsVisible = false;
        public bool ClearIsVisible
        {
            get => clearIsVisible;
            set => SetProperty(ref clearIsVisible, value);
        }

        bool addAmountIsVisible = true;
        public bool AddAmountIsVisible
        {
            get => addAmountIsVisible;
            set => SetProperty(ref addAmountIsVisible, value);
        }

        public ReceiveViewModel()
        {
            ShareIntentCommand = new Command(ShareIntent);
            AddAmountCommand = new Command(AddAmount);
            ClearCommand = new Command(Clear);
            CopyAddressCommand = new Command(CopyAddress);

            RefreshAddressFromWalletService();
        }

        async void CopyAddress(object obj)
        {
            await Clipboard.SetTextAsync(AddressFormatted);

            MessagingCenter.Send(this, "CopiedAddressToast");
        }

        public void RefreshAddressFromWalletService()
        {
            if (WalletService.IsStarted) SetAddressFromWalletService();
            else WalletService.OnStarted += WalletService_OnStarted;
        }

        internal void UpdateAddressFormatted()
        {
            if (string.IsNullOrEmpty(Amount)) return;
            if (Amount.StartsWith('.')) Amount = $"0.{Amount[1..]}";
            if (!decimal.TryParse(Amount, out var amount)) return;

            var split = Amount.Split('.');
            if (split.Length == 2)
            {
                var decimals = split[1];
                if (decimals.Length > 8 && decimals.All(digit => digit == '0'))
                {
                    Amount = "0.00000000";
                }
            }

            if (amount <= 0)
            {
                AddressFormatted = Address.ToString();

                return;
            }

            if (HasLessEightDecimalPlaces(Amount)) AddressFormatted = $"bitcoin:{Address}?amount={Amount}";
            else Amount = Amount[..(Amount.Length - 1)];
        }

        bool HasLessEightDecimalPlaces(string decimalStr)
        {
            decimal value = Convert.ToDecimal(decimalStr);
            decimal step = (decimal)Math.Pow(10, 8);

            return (value * step) - Math.Truncate(value * step) == 0;
        }

        void AddAmount()
        {
            AmountFrameIsVisible = true;
            ClearIsVisible = true;
            AddAmountIsVisible = false;

            UpdateAddressFormatted();
        }

        void Clear()
        {
            AmountFrameIsVisible = false;
            ClearIsVisible = false;
            AddAmountIsVisible = true;

            AddressFormatted = Address.ToString();
        }

        void ShareIntent()
        {
            Debug.WriteLine($"[ShareIntent] Sharing address: {AddressFormatted}");

            if (Address is null)
                Debug.WriteLine("[ShareIntent] Sharing address is empty");
            else
                ShareIntentService.QRTextShareIntent(AddressFormatted);
        }

        void WalletService_OnStarted(object sender, EventArgs e)
        {
            SetAddressFromWalletService();
        }

        void SetAddressFromWalletService()
        {
            Address = WalletService.GetReceiveAddress();
            AddressFormatted = Address.ToString();

            Debug.WriteLine($"[SetAddressFromWalletService] New address: {AddressFormatted}");
        }
    }
}
