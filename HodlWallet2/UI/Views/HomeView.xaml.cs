using System;
using System.Collections.Generic;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.ViewModels;
using NBitcoin;
using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class HomeView : ContentPage
    {
        public HomeView()
        {
            InitializeComponent();

            SubscribeToMessages();
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
    }
}
