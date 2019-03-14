using System;
using System.Collections.Generic;

using Xamarin.Forms;

using HodlWallet2.ViewModels;
using HodlWallet2.Locale;
using HodlWallet2.Utils;

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

            FingerprintCheckHeader.Text = Device.RuntimePlatform == Device.iOS?
                                          LocaleResources.SecurityCenter_fingerprintHeaderIOS : LocaleResources.SecurityCenter_fingerprintHeaderAndroid;

            FingerprintCheckDetail.Text = LocaleResources.SecurityCenter_fingerprintDetail;
            MnemonicCheckHeader.Text = LocaleResources.SecurityCenter_mnemonicHeader;
            MnemonicCheckDetail.Text = LocaleResources.SecurityCenter_mnemonicDetail;
        }

        public async void OnCloseTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        public async void OnFaqTapped(object sender, EventArgs e)
        {
            // TODO:
        }

        public async void OnFingerprintTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FingerprintAuthView());
        }

        public async void OnMnemonicTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
            await App.Current.MainPage.Navigation.PushAsync(new BackupView());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.InitializeChecks();
        }
    }
}
