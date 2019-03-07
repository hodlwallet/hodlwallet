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

        const string grayCheck = "Assets.circle_check_gray.svg", yellowCheck = "Assets.circle_check_yellow.svg";

        public SecurityCenterView(SecurityCenterViewModel viewModel)
        {
            InitializeComponent();
            SetLabels();
            BindingContext = viewModel;
        }

        private void SetLabels()
        {
            SecurityCenterTitle.Text = LocaleResources.SecurityCenter_title;
            ShieldHeader.Text = LocaleResources.SecurityCenter_header;
            PinCheckHeader.Text = LocaleResources.SecurityCenter_pinHeader;
            PinCheckDetail.Text = LocaleResources.SecurityCenter_pinDetail;
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
