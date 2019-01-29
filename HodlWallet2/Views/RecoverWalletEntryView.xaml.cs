using System;
using System.Collections.Generic;

using Xamarin.Forms;

using HodlWallet2.Locale;

namespace HodlWallet2.Views
{
    public partial class RecoverWalletEntryView : ContentPage
    {
        public RecoverWalletEntryView()
        {
            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            Title.Text = LocaleResources.Recover_title;
            Header.Text = LocaleResources.Recover_entryHeader;
        }
    }
}
