using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Serilog;

using HodlWallet2.Locale;
using HodlWallet2.ViewModels;

namespace HodlWallet2.Views
{
    public partial class BackupView : ContentPage
    {
        Wallet _Wallet;
        ILogger _Logger;
        string[] Words = { "zebra", "blame", "wink", "cactus", "hungry", "often", "uniform", "minute", "broom", "weapon", "venture", "creek" }; /* WIP until Wallet.Instance isses are fixed */

        public BackupView()
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;

            if (/*SecureStorageProvider.HasMnemonic() == false*/ true)
            {
                // SecureStorageProvider.SetMnemonic(_Wallet.WalletManager.CreateWallet("hodl", SecureStorageProvider.GetPassword(), null, "english", 12).ToString());
            }
            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            Title.Text = LocaleResources.Backup_title;
            Subheader.Text = LocaleResources.Backup_subheader;
            Button.Text = LocaleResources.Backup_button;
        }

        private void BackupButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BackupRecoveryWordView(1, Words));
            _Logger.Information("Backup button clicked.");
        }
    }
}
