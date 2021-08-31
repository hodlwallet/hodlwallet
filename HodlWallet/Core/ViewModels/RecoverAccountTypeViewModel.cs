//
// RecoverAccountTypeViewModel.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2021 HODL Wallet
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace HodlWallet.Core.ViewModels
{
    class RecoverAccountTypeViewModel : BaseViewModel
    {
        string accountType = null;
        public string AccountType
        {
            get => accountType;
            set => SetProperty(ref accountType, value);
        }

        public bool IsNotSelected => string.IsNullOrEmpty(AccountType);

        public ICommand AccountTypeSelectedCommand { get; }
        CancellationTokenSource Cts { get; set; }

        public RecoverAccountTypeViewModel()
        {
            Cts ??= new CancellationTokenSource();
            AccountTypeSelectedCommand = new Command<string>(AccountTypeSelected);
        }

        public void InitializeWallet(string accountType)
        {
            IsLoading = true;

            Task.Factory.StartNew(
                () => WalletService.InitializeWallet(accountType),
                Cts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );

            WalletService.OnStarted += WalletService_OnStarted;
        }

        public void InitializePrice()
        {
            Task.Factory.StartNew(
                () => PrecioService.Init(),
                Cts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );
        }

        private void WalletService_OnStarted(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "InitAppShell");

            IsLoading = false;
        }

        void AccountTypeSelected(string typeSelected)
        {
            Debug.WriteLine($"[AccountTypeSelected] Account type selected: {typeSelected}");

            if (AccountType is null)
            {
                MessagingCenter.Send(this, "HideEmptyState");
            }

            AccountType = typeSelected;

            MessagingCenter.Send(this, "AnimateSelected");
        }
    }
}
