using System;
using System.Collections.Generic;

using Xamarin.Forms;

using HodlWallet2.Locale;

namespace HodlWallet2.Views
{
    public partial class BackupRecoveryWordView : ContentPage
    {
        public BackupRecoveryWordView()
        {
            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            Title.Text = LocaleResources.Backup_title;
            Header.Text = LocaleResources.Backup_wordheader;
            Position.Text = "1 of 12";
            Next.Text = LocaleResources.Seed_next;
        }
    }
}
