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

namespace HodlWallet2.Core.ViewModels
{
    public class ReceiveViewModel : BaseViewModel
    {
        public string ShareButtonText => "Share";
        public ICommand ShowShareIntentCommand { get; }

        string _Address;
        public string Address
        {
            get => _Address;
            set => SetProperty(ref _Address, value);
        }

        public ReceiveViewModel()
        {
            ShowShareIntentCommand = new Command(ShowShareIntent);

            if (_WalletService.IsStarted)
            {
                GetAddressFromWallet();
            }
            else
            {
                _WalletService.OnStarted += _WalletService_OnStarted;
            }
        }

        private void _WalletService_OnStarted(object sender, EventArgs e)
        {
            GetAddressFromWallet();
        }

        void GetAddressFromWallet()
        {
            Address = _WalletService.GetReceiveAddress().Address;

            Debug.WriteLine($"[GetAddressFromWallet] New address: {Address}");
        }

        void ShowShareIntent()
        {
            Debug.WriteLine($"[ShowShareIntent] Sharing address: {Address}");

            _ShareIntent.QRTextShareIntent(Address);
        }
    }
}
