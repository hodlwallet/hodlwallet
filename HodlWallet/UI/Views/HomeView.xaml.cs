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

using Xamarin.Forms;

using HodlWallet.Core.Interfaces;
using HodlWallet.Core.Models;
using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Extensions;
using System.Threading.Tasks;

namespace HodlWallet.UI.Views
{
    public partial class HomeView : ContentPage
    {
        HomeViewModel ViewModel => BindingContext as HomeViewModel;
        IWalletService WalletService => DependencyService.Get<IWalletService>();

        public HomeView()
        {
            InitializeComponent();

            SubscribeToMessages();

            if (WalletService.Syncing)
            {
                SyncToolbarItem.IsVisible = true;
                SearchToolBarItem.IsVisible = false;
            }
            else
            {
                SyncToolbarItem.IsVisible = false;
                SearchToolBarItem.IsVisible = true;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //PriceButton.Source = "price_tag_3_line.png";

            ViewModel.View_OnAppearing();

            ViewModel.InitializeWalletAndPrecio();

            InitializeDisplayedCurrency();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ViewModel.View_OnDisappearing();
        }

        void InitializeDisplayedCurrency()
        {
            if (ViewModel.Currency == "BTC")
            {
                //BalanceScrollView.ScrollToAsync(0, BalanceAmountBTC.Y, true);
            }
            else
            {
                //BalanceScrollView.ScrollToAsync(0, BalanceAmountUSD.Y, true);
            }
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<HomeViewModel, TransactionModel>(this, "NavigateToTransactionDetail", NavigateToTransactionDetail);
            MessagingCenter.Subscribe<HomeViewModel>(this, "SwitchCurrency", SwitchCurrency);
        }

        async void NavigateToTransactionDetail(HomeViewModel _, TransactionModel txModel)
        {
            var view = new TransactionDetailsView(txModel);
            var nav = new NavigationPage(view);

            await Navigation.PushModalAsync(nav);
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

        //void PriceButton_Tapped(object sender, EventArgs e)
        //{
        //    PriceButton.Source = "price_tag_3_fill.png";

        //    Navigation.PushModalAsync(new PriceView());
        //}

        void SwitchCurrency(HomeViewModel _)
        {
            if (ViewModel.IsBtcEnabled)
            {
                //BalanceScrollView.ScrollToAsync(0, BalanceAmountBTC.Y, true);
            }
            else
            {
                //BalanceScrollView.ScrollToAsync(0, BalanceAmountUSD.Y, true);
            }
        }

        async void Search_Clicked(object sender, EventArgs e)
        {
            await DisplaySearchNotImplementedMessage();
        }

        async void Sync_Clicked(object sender, EventArgs e)
        {
            await DisplaySyncingMessage();
        }

        void TransactionsScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (BalanceLabel.Bounds.Bottom < e.ScrollY)
            {
                BalanceNavigationTitleLabel.FadeTo(1, 100);
            }
            else
            {
                BalanceNavigationTitleLabel.FadeTo(0, 50);
            }
        }

        async void Balance_Tapped(object sender, EventArgs args)
        {
            ViewModel.SwitchCurrency();

            await this.DisplayToast($"Balance tapped, currency: {ViewModel.Currency}");
        }
    }
}
