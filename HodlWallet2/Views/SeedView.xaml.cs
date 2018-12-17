using System;
using System.Collections.Generic;

using HodlWallet2.Locale;

using Xamarin.Forms;

namespace HodlWallet2.Views
{
    public partial class SeedView : ContentPage
    {
        public SeedView()
        {
            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            Title.Text = LocaleResources.Seed_title.ToUpper();
            Subheader.Text = LocaleResources.Seed_subheader;
            NextButton.Text = LocaleResources.Seed_next.ToUpper();
        }
    }
}
