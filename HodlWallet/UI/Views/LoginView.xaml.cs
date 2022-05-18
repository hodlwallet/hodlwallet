﻿//
// LoginView.xaml.cs
//
// Copyright (c) 2021 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Plugin.Fingerprint;
using Xamarin.Forms;
using Xamarin.Essentials;

using HodlWallet.Core.ViewModels;
using HodlWallet.Core.Services;
using HodlWallet.UI.Views.Demos;
using HodlWallet.UI.Extensions;
using Plugin.Fingerprint.Abstractions;
using HodlWallet.Core.Utils;
using HodlWallet.UI.Locale;

namespace HodlWallet.UI.Views
{
    public partial class LoginView : ContentPage
    {
        LoginViewModel ViewModel => (LoginViewModel)BindingContext;

        public LoginView(string action = null)
        {
            InitializeComponent();
            ViewModel.Action = action;
            SubscribeToMessages();

            ViewModel.LastLogin = Preferences.Get("lastLogin", "pin");


            if (ViewModel.Action != "update")
            {
                CheckBiometricsAvailabilityAsync();
                NavigationPage.SetHasNavigationBar(this, false);

                if (ViewModel.LastLogin == "biometric")
                {
                    FingerprintButtonClicked(this, new EventArgs());
                }

                return;
            }

            LogoFront.IsVisible = false;
            ContHeader.IsVisible = true;
            Header.IsVisible = false;
            enterlabelspacer.IsVisible = true;

            Title = Locale.LocaleResources.Pin_updateTitle;

            NavigationPage.SetHasNavigationBar(this, true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.LoginFormVisible = true;

            var biometricsAllow = Preferences.Get("biometricsAllow", false);

            if (ViewModel.Action != "update")
            {
                fingerprint.IsVisible = ViewModel.BiometricsAvailable && biometricsAllow;
                fingerprintspacer.IsVisible = !(ViewModel.BiometricsAvailable && biometricsAllow);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (ViewModel.IsLoading) ViewModel.IsLoading = false;

            ViewModel.LoginFormVisible = false;
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<LoginViewModel>(this, "StartAppShell", async (vm) => await StartAppShell(vm));
            MessagingCenter.Subscribe<LoginViewModel>(this, "UpdatePin", UpdatePin);
        }

        void UnsubscribeToMessages()
        {
            MessagingCenter.Unsubscribe<LoginViewModel, int>(this, "StartAppShell");
            MessagingCenter.Unsubscribe<LoginViewModel, int>(this, "UpdatePin");
        }

        async void CheckBiometricsAvailabilityAsync()
        {
            ViewModel.BiometricsAvailable = await CrossFingerprint.Current.IsAvailableAsync();
        }

        async Task StartAppShell(LoginViewModel vm)
        {
            Debug.WriteLine($"[SubscribeToMessage][StartAppShell]");
            
            UnsubscribeToMessages();

            if (vm.Action == "pop") // Login after logout or timeout
            {
                await Shell.Current.GoToAsync("..");

                return;
            }

            // Init app after startup, new wallet or restore
            Application.Current.MainPage = new AppShell();
        }

        async void UpdatePin(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][UpdatePin]");

            await Navigation.PushAsync(new PinPadView("PinPadChangeView"));
            UnsubscribeToMessages();
            return;
        }

        async void Logo_Tapped(object sender, EventArgs e)
        {
#if DEBUG || TESTNET
            if (SecureStorageService.HasMnemonic())
            {
                Debug.WriteLine($"[Logo_Tapped] Seed: {SecureStorageService.GetMnemonic()}");

                await Clipboard.SetTextAsync(SecureStorageService.GetMnemonic());
                await this.DisplayToast("Mnemonic copied to clipboard");
            }

            Application.Current.MainPage = new ControlsDemoView();

            UnsubscribeToMessages();
#endif
        }

        async void FingerprintButtonClicked(object sender, EventArgs e)
        {
            var authResult = await CrossFingerprint.Current.AuthenticateAsync(
                new AuthenticationRequestConfiguration(Constants.HODL_WALLET, LocaleResources.BiometricLogin_askAuth));

            if (authResult.Authenticated)
            {
                ViewModel.IsLoading = true;
                LoginViewModel.SetLastLoginAs("biometric");
                // DONE! We navigate to the root view
                await Task.Delay(65);
                await StartAppShell(ViewModel);
            }
        }

        async void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}