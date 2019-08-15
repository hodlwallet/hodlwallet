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
using System.Collections.Generic;
using System.Threading.Tasks;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.ViewModels;
using NBitcoin;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

using HodlWallet2.UI.Extensions;

namespace HodlWallet2.UI.Views
{
    public partial class HomeView : ContentPage
    {
        HomeViewModel _ViewModel => (HomeViewModel)BindingContext;

        public HomeView()
        {
            InitializeComponent();

            SubscribeToMessages();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            PriceButton.Source = "price-tag-3-line.png";

            _ViewModel.View_OnAppearing();

            _ViewModel.InitializeWalletAndPrecio();

            InitializeDisplayedCurrency();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _ViewModel.View_OnDisappearing();
        }

        void InitializeDisplayedCurrency()
        {
            var currency = Preferences.Get("currency", "BTC");

            if (currency == "BTC")
            {
                BalanceScrollView.ScrollToAsync(0, BalanceAmountBTC.Y, true);
            }
            else
            {
                BalanceScrollView.ScrollToAsync(0, BalanceAmountUSD.Y, true);
            }
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<HomeViewModel, TransactionModel>(this, "NavigateToTransactionDetail", NavigateToTransactionDetail);

            MessagingCenter.Subscribe<HomeViewModel>(this, "DisplaySearchNotImplementedAlert", DisplaySearchNotImplementedAlert);

            MessagingCenter.Subscribe<HomeViewModel>(this, "SwitchCurrency", SwitchCurrency);
        }

        void NavigateToTransactionDetail(HomeViewModel _, TransactionModel txModel)
        {
            var view = new TransactionDetailsView(txModel);
            var nav = new NavigationPage(view);

            Navigation.PushModalAsync(nav);
        }

        void DisplaySearchNotImplementedAlert(HomeViewModel vm)
        {
            _ = this.DisplayToast("Search Not Implemented");
        }

        void PriceButton_Tapped(object sender, EventArgs e)
        {
            PriceButton.Source = "price-tag-3-fill.png";

            Navigation.PushModalAsync(new PriceView());
        }

        void SwitchCurrency(HomeViewModel _)
        {
            if (_ViewModel.IsBtcEnabled)
            {
                BalanceScrollView.ScrollToAsync(0, BalanceAmountBTC.Y, true);
            }
            else
            {
                BalanceScrollView.ScrollToAsync(0, BalanceAmountUSD.Y, true);
            }
        }
    }
}
