//
// BiometricLoginView.xaml.cs
//
// Copyright (c) 2019 HODL Wallet
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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HodlWallet.Core.ViewModels;

using HodlWallet.Core.Services;
using HodlWallet.UI.Views.Demos;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using HodlWallet.UI.Locale;
using HodlWallet.Core.Utils;

namespace HodlWallet.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BiometricLoginView : ContentPage
    {
        BiometricLoginViewModel ViewModel => (BiometricLoginViewModel)BindingContext;

        public BiometricLoginView(string action = null)
        {
            InitializeComponent();

            ViewModel.Action = action;
            LoginViewModel loginViewModel = new LoginViewModel
            {
                LastLogin = "Fingerprint"
            };

            if (ViewModel.Action == "update")
            {
                LogoFront.IsVisible = false;
                Header.Text = LocaleResources.Pin_updateHeader;
                CancelButton.IsEnabled = true;
                CancelButton.IsVisible = true;
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                BiometricsButton.Text = LocaleResources.Login_useFaceId;
            }
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.LoginFormVisible = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (ViewModel.IsLoading) ViewModel.IsLoading = false;

            ViewModel.LoginFormVisible = false;
        }

        void StartAppShell()
        {
            Debug.WriteLine($"[SubscribeToMessage][StartAppShell]");

            if (ViewModel.Action == "update") // Update PIN
            {
                Navigation.PushAsync(new PinPadView(new PinPadChangeView()));

                return;
            }
            else if (ViewModel.Action == "pop") // Login after logout or timeout
            {
                Navigation.PopModalAsync();

                return;
            }

            // Init app after startup, new wallet or restore
            Application.Current.MainPage = new AppShell();
        }

        void Logo_Tapped(object sender, EventArgs e)
        {
            if (SecureStorageService.HasMnemonic())
                Debug.WriteLine($"[Logo_Tapped] Seed: {SecureStorageService.GetMnemonic()}");

            Application.Current.MainPage = new ControlsDemoView();
        }

        void CancelButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void BiometricsButton_Clicked(object sender, EventArgs e)
        {
            var authResult = await CrossFingerprint.Current.AuthenticateAsync(
                new AuthenticationRequestConfiguration(Constants.HODL_WALLET, "Use biometric authentication"));

            if (authResult.Authenticated)
            {
                Debug.WriteLine("[Fingerprint] Logged in!");
                ViewModel.IsLoading = true;

                // DONE! We navigate to the root view
                await Task.Delay(65);

                StartAppShell();
                return;
            }

        }

        void UsePinButton(object sender, EventArgs e)
        {
            var view = new LoginView(ViewModel.Action);
            Application.Current.MainPage = view;
        }
    }
}