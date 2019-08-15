﻿//
// PinPadViewModel.cs
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
using System.Threading.Tasks;
using System.Windows.Input;
using NBitcoin.RPC;
using Xamarin.Forms;

using HodlWallet2.Core.Services;
using HodlWallet2.UI.Views;

namespace HodlWallet2.Core.ViewModels
{
    public class PinPadViewModel : BaseViewModel
    {
        public string PinPadTitle { get; } = "Enter PIN";
        public string PinPadHeader { get; } = "Your PIN will be used to unlock your wallet and send money.";
        public string PinPadWarning { get; } = "Remember this PIN. If you forget it, you won't be able to access your bitcoin.";

        public ICommand SuccessCommand { get; }

        public PinPadViewModel()
        {
            SuccessCommand = new Command<string>(Success_Callback);
        }

        void Success_Callback(string pin)
        {
            SavePin(pin);

            MessagingCenter.Send(this, "NavigateToBackupViewModel");
        }

        void SavePin(string pin)
        {
            SecureStorageService.SetPin(pin);
        }
    }
}
