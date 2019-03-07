using System;
using System.Collections.Generic;

using Xamarin.Forms;

using HodlWallet2.ViewModels;
using HodlWallet2.Locale;

namespace HodlWallet2.Views
{
    public partial class SecurityCenterView : ContentPage
    {
        public SecurityCenterViewModel ViewModel { get { return BindingContext as SecurityCenterViewModel; } }

        public SecurityCenterView(SecurityCenterViewModel viewModel)
        {
            InitializeComponent();
            SetLabels();
            BindingContext = viewModel;
        }

        private void SetLabels()
        {
            SecurityCenterTitle.Text = LocaleResources.SecurityCenter_title;
            ShieldHeader.Text = "Header";
        }

        public async void OnCloseTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        public async void OnFaqTapped(object sender, EventArgs e)
        {
            // TODO:
        }
    }
}
