using System;
using System.Collections.Generic;

using Xamarin.Forms;
using HodlWallet2.Locale;
using Serilog;

using HodlWallet2.Views;
using HodlWallet2.ViewModels;

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

        public void OnSendTapped(object sender, EventArgs e)
        {
        }

        public async void OnReceiveTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecieveView(new ReceiveViewModel()));
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
