using System;
using System.Collections.Generic;

using Xamarin.Forms;
using HodlWallet2.Locale;
using Serilog;

using HodlWallet2.Views;

namespace HodlWallet2.ViewModels
{
    public partial class DashboardPage : ContentPage
    {
        private Wallet _Wallet;
        private ILogger _Logger;

        public DashboardPage()
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;

            InitializeComponent();
            SetTempLabels();
        }

        public async void OnSendTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SendView(new ViewModels.SendViewModel()));
        }

        public async void OnReceiveTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecieveView());
        }

        public void OnMenuTapped(object sender, EventArgs e)
        {
        }

        public void SetTempLabels()
        {
            amountLabel.Text = "20 BTC";
            priceLabel.Text = "1 BTC = $4";
            sendLabel.Text = LocaleResources.Dashboard_send;
            receiveLabel.Text = LocaleResources.Dashboard_receive;
            menuLabel.Text = LocaleResources.Dashboard_menu;
        }
    }
}
