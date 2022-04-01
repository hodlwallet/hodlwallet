//
// HomeView.xaml.cs
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
using System.Threading.Tasks;

using Liviano.Interfaces;
using Xamarin.Forms;

using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Extensions;

namespace HodlWallet.UI.Views
{
    public partial class HomeView : ContentPage
    {
        HomeViewModel ViewModel => BindingContext as HomeViewModel;

        public HomeView()
        {
            InitializeComponent();
        }

        async Task DisplaySearchNotImplementedMessage()
        {
            await this.DisplayToast("Search Not Implemented");
        }

        async Task DisplaySyncingMessage()
        {
            // TODO Display better information
            await this.DisplayToast("Syncing...");
        }

        public async Task SwitchAccount(IAccount account)
        {
            Debug.WriteLine($"[SwitchAccount] AccountID: {account.Id}");

            // TODO Make these ones locale
            var prompt = this.DisplayPrompt(
                "Switch Account",
                $"Switch account to \"{account.Name}\"",
                "OK",
                "CANCEL"
            );

            if (!(await prompt)) return;

            ViewModel.SwitchAccount(account);
            // Hide the flyout menu
            Shell.Current.FlyoutIsPresented = false;
            await this.DisplayToast($"Switched account to {account.Id}");
        }

        async void Search_Clicked(object sender, EventArgs e)
        {
            await DisplaySearchNotImplementedMessage();
        }

        async void Sync_Clicked(object sender, EventArgs e)
        {
            await DisplaySyncingMessage();
        }

        void TopView_DoubleTapped(object sender, EventArgs e)
        {
            transactionsView.ScrollToTop();
        }
    }
}
