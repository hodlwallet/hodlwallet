﻿using System;
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

        public async void OnSendTapped(object sender, EventArgs e)
        {
            _Logger.Information("On Send Tapped.");

            await Navigation.PushAsync(new RecieveView(new ReceiveViewModel(_Wallet.GetReceiveAddress().ToString())));
        }

        public void OnReceiveTapped(object sender, EventArgs e)
        {
            //TODO add code here
        }

        public void OnMenuTapped(object sender, EventArgs e)
        {
            //TODO add code here
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
