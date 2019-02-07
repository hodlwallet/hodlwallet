using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Serilog;

using HodlWallet2.Locale;
using HodlWallet2.ViewModels;

namespace HodlWallet2.Views
{
    public partial class RecoverView : ContentPage
    {
        Wallet _Wallet;
        ILogger _Logger;

        public RecoverView()
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;

            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            Title = LocaleResources.Recover_title;
            Header.Text = LocaleResources.Recover_header;
            Next.Text = LocaleResources.Recover_next;
        }

        private void RecoverNext_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RecoverWalletEntryView(new RecoverWalletEntryViewModel()));
            _Logger.Information("Recover button clicked.");
        }
    }
}
