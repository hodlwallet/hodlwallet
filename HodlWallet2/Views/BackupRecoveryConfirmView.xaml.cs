using System;
using System.Collections.Generic;

using HodlWallet2.Locale;

using Xamarin.Forms;

namespace HodlWallet2.Views
{
    public partial class BackupRecoveryConfirmView : ContentPage
    {
        public BackupRecoveryConfirmView()
        {
            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            Title.Text = LocaleResources.Backup_title;
            Header.Text = LocaleResources.ConfirmBackup_header;
        }
    }
}
