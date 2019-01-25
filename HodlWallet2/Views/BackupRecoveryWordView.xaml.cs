using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Serilog;

using HodlWallet2.Locale;
using HodlWallet2.Utils;

namespace HodlWallet2.Views
{
    public partial class BackupRecoveryWordView : ContentPage
    {
        int Position;
        string[] Mnemonic;
        Wallet _Wallet;
        ILogger _Logger;

        public BackupRecoveryWordView(int position, string[] mnemonic)
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;
            InitializeComponent();
            Position = position;
            Mnemonic = mnemonic;
            SetLabels();
        }

        private void SetLabels()
        {
            Title.Text = LocaleResources.Backup_title;
            Header.Text = LocaleResources.Backup_wordheader;
            Index.Text = Position + " of 12";
            Word.Text = Mnemonic[Position - 1];
            Next.Text = LocaleResources.Seed_next;
        }

        private void BackupWord_Clicked(object sender, EventArgs e)
        {
            if (Position < 12)
            {
                Position++;
                Navigation.PushAsync(new BackupRecoveryWordView(Position, Mnemonic));
                _Logger.Information("Backup button clicked.");
            }
            else
            {
                Navigation.PushAsync(new BackupRecoveryConfirmView());
                _Logger.Information("Backup recovery confirmation initiated.");
            }
        }
    }
}
