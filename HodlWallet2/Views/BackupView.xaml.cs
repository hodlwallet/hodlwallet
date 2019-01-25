using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Serilog;

using HodlWallet2.Locale;
using HodlWallet2.ViewModels;
using HodlWallet2.Utils;

namespace HodlWallet2.Views
{
    public partial class BackupView : ContentPage
    {
        Wallet _Wallet;
        ILogger _Logger;
        string[] mnemonic;

        public BackupView()
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;

            if (SecureStorageProvider.HasMnemonic() == false)
            {
                mnemonic = Wallet.GetNewMnemonic("english", 12).Split(' ');
                SecureStorageProvider.SetMnemonic(string.Join(" ", mnemonic));
            }
            else
            {
                mnemonic = SecureStorageProvider.GetMnemonic().Split(' ');
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
            Navigation.PushAsync(new BackupRecoveryWordView(1, mnemonic));
            _Logger.Information("Backup button clicked.");
        }
    }
}
