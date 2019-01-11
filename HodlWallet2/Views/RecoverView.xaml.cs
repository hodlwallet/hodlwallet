using System;
using System.Collections.Generic;

using HodlWallet2.Locale;

using Xamarin.Forms;

namespace HodlWallet2.Views
{
    public partial class RecoverView : ContentPage
    {
        public RecoverView()
        {
            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            Title.Text = LocaleResources.Recover_title;
            Subheader.Text = LocaleResources.Recover_subheader;
            Button.Text = LocaleResources.Seed_next;
        }
    }
}
