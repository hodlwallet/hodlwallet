//
// HomeViewModel.cs
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
using System.Diagnostics;
using System.Windows.Input;

using Xamarin.Forms;

using Liviano.Interfaces;

namespace HodlWallet.Core.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        string accountName;
        public string AccountName
        {
            get => accountName;
            set => SetProperty(ref accountName, value);
        }

        bool syncIsVisible;
        public bool SyncIsVisible
        {
            get => syncIsVisible;
            set => SetProperty(ref syncIsVisible, value);
        }

        string[] colors = new string[] { "Red", "Blue", "Green" };
        public string[] Colors
        {
            get => colors;
            set => SetProperty(ref colors, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand NavigateToTransactionDetailsCommand { get; }

        public HomeViewModel()
        {
            SearchCommand = new Command(StartSearch);
        }

        void StartSearch()
        {
            Debug.WriteLine("[StartSearch] Search is not implemented yet!");

            MessagingCenter.Send(this, "DisplaySearchNotImplementedAlert");
        }

        public void SwitchAccount(IAccount account)
        {
            Debug.WriteLine($"[SwitchAccount] AccountID: {account.Id}");

            var wallet = WalletService.Wallet;
            for (int i = 0; i < wallet.Accounts.Count; i++)
            {
                if (wallet.Accounts[i].Id != account.Id) continue;

                WalletService.Wallet.CurrentAccount = WalletService.Wallet.Accounts[i];

                break;
            }

            WalletService.Wallet.Storage.Save();
        }
    }
}
