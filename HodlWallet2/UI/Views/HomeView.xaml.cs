using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.ViewModels;
using NBitcoin;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

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

            _ViewModel.InitializeWalletAndPrecio();
            _ViewModel.View_OnAppearing();

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

        void DisplaySearchNotImplementedAlert(HomeViewModel _)
        {
            DisplayAlert("Search Not Implemented", null, "OK");
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
