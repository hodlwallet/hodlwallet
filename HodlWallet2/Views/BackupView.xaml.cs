using System;
using System.Collections.Generic;

using HodlWallet2.Locale;

using Xamarin.Forms;

namespace HodlWallet2.Views
{
    public partial class BackupView : ContentPage
    {
        public BackupView()
        {
            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            Title.Text = LocaleResources.Backup_title;
            Subheader.Text = LocaleResources.Backup_subheader;
            Button.Text = LocaleResources.Backup_button;
        }
    }
}
