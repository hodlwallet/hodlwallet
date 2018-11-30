using System;
using System.Collections.Generic;

using Xamarin.Forms;
using HodlWallet2.Locale;

namespace HodlWallet2.ViewModels
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
            SetTempLabels();


        }

        public void OnSendTapped(object sender, EventArgs e)
        {
            //TODO add code here
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
