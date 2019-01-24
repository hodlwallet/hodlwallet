using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Serilog;

using HodlWallet2.Locale;

namespace HodlWallet2.Views
{
    public partial class RecoverView : ContentPage
    {
        Wallet _Wallet;
        ILogger _Logger;

        public RecoverView()
        {
            InitializeComponent();
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;
            SetLabels();
        }

        private void RecoverWallet_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RecoverSeedEntryView());
            _Logger.Information("Recover button clicked.");
        }

        private void SetLabels()
        {
            Title.Text = LocaleResources.Recover_title;
            Header.Text = LocaleResources.Recover_header;
            Button.Text = LocaleResources.Recover_next;
        }
    }
}
