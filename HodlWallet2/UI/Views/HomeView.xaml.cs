using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.ViewModels;
using NBitcoin;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace HodlWallet2.UI.Views
{
    public partial class HomeView : ContentPage
    {
        HomeViewModel _ViewModel => (HomeViewModel)BindingContext;
        bool _IsBtc = true;
        bool _IsPriceShowing = false;
        decimal _Balance = 0.50341173m;
        object _Lock = new object();

        public HomeView()
        {
            InitializeComponent();

            SubscribeToMessages();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ViewModel.InitializeWalletAndPrecio();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<HomeViewModel, TransactionModel>(this, "NavigateToTransactionDetail", NavigateToTransactionDetail);
        }

        void NavigateToTransactionDetail(HomeViewModel _, TransactionModel txModel)
        {
            var view = new TransactionDetailsView(txModel);
            var nav = new NavigationPage(view);

            Navigation.PushModalAsync(nav);
        }

        void PriceButton_Tapped(object sender, EventArgs e)
        {
            if (_IsPriceShowing)
            {
                // Move to balance
                BalanceAndPriceScrollView.ScrollToAsync(0, BalanceAmount.Y, true);

                PriceButton.Source = "price-tag-3-line.png";
            }
            else
            {
                // Move to price
                BalanceAndPriceScrollView.ScrollToAsync(0, PriceLabel.Y, true);

                PriceButton.Source = "price-tag-3-fill.png";
            }

            _IsPriceShowing = !_IsPriceShowing;
        }

        void Balance_Tapped(object sender, EventArgs e)
        {
            var price = 10_840.00m;

            lock (_Lock)
            {
                if (_IsBtc)
                {
                    // Switch to USD
                    var newAmount = _Balance * price;
                    var newAmountStr = string.Format("≈ {0:C}", newAmount);

                    ChangeBalanceTo(newAmountStr, "usd");
                }
                else
                {
                    // Switch to BTC
                    var newAmount = _Balance;
                    var newAmountStr = newAmount.ToString();

                    ChangeBalanceTo(newAmountStr, "btc");
                }

                _IsBtc = !_IsBtc;
            }
        }

        void ChangeBalanceTo(string amountText, string currency)
        {
            BalanceAmount.Text = amountText;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await BalanceLabel.FadeTo(0.0);
                BalanceLabel.Text = currency;
                await BalanceLabel.FadeTo(1.0);
            });
        }
    }
}
