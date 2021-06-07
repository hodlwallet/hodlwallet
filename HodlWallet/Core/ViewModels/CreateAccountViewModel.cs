//
// CreateAccountViewModel.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

using Xamarin.Forms;


namespace HodlWallet.Core.ViewModels
{
    class CreateAccountViewModel : BaseViewModel
    {
        string[] accountTypesList = { "bip84", "bip141" };
        public IList<string> AcountTypes { get => accountTypesList; }
        public ICommand CreateAccountCommand { get; }

        string accountName;

        public string AccountName
        {
            get => accountName;
            set => SetProperty(ref accountName, value);
        }

        string accountType;

        public string AccountType
        {
            get => accountType;
            set => SetProperty(ref accountType, value);
        }
        public CreateAccountViewModel()
        {
            CreateAccountCommand = new Command(CreateAccount);
        }
        private async void CreateAccount()
        {
            var (Success, Error) = await WalletService.AddAccount(AccountType ?? accountTypesList[0], AccountName);
            await Shell.Current.GoToAsync("..");
        }
    }
}
