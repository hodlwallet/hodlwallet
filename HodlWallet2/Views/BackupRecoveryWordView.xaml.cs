using System;
using System.Collections.Generic;

using Xamarin.Forms;

using HodlWallet2.Locale;

namespace HodlWallet2.Views
{
    public partial class BackupRecoveryWordView : ContentPage
    {
        public BackupRecoveryWordView(int position, string word)
        {
            InitializeComponent();
            SetLabels(position, word);
        }

        private void SetLabels(int position, string word)
        {
            Title.Text = LocaleResources.Backup_title;
            Header.Text = LocaleResources.Backup_wordheader;
            Position.Text = position + " of 12";
            Word.Text = word;
            Next.Text = LocaleResources.Seed_next;
        }
    }
}
