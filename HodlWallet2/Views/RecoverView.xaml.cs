using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;
using Serilog;

using HodlWallet2.Locale;
using MvvmCross.Forms.Views;
using HodlWallet2.Core.Services;

namespace HodlWallet2.Views
{
    public partial class RecoverView : MvxContentPage<RecoverViewModel>
    {
        WalletService _Wallet;
        ILogger _Logger;

        public RecoverView()
        {
            _Wallet = WalletService.Instance;
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
            //Navigation.PushAsync(new RecoverWalletEntryView(new RecoverWalletEntryViewModel()));
            _Logger.Information("Recover button clicked.");
        }
    }
}
