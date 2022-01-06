//
// BiometricLoginView.xaml.cs
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
            SubscribeToMessages();

            LoginViewModel.SetLastLoginAs("biometric");

            if (ViewModel.Action == "update")
            {
                LogoFront.IsVisible = false;
                NavigationPage.SetHasNavigationBar(this, true);
            }
            else
            {
                NavigationPage.SetHasNavigationBar(this, false);
            }

            BiometricsButtonClicked(this, new EventArgs());
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

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<BiometricLoginViewModel>(this, "StartAppShell", StartAppShell);
            MessagingCenter.Subscribe<BiometricLoginViewModel>(this, "UpdatePin", UpdatePin);
        }

        void UnsubscribeToMessages()
        {
            MessagingCenter.Unsubscribe<LoginViewModel, int>(this, "StartAppShell");
            MessagingCenter.Unsubscribe<LoginViewModel, int>(this, "UpdatePin");
        }

        void UpdatePin(BiometricLoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][UpdatePin]");

            Navigation.PushAsync(new PinPadView(new PinPadChangeView()));
            UnsubscribeToMessages();
            return;
        }


        void StartAppShell(BiometricLoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][StartAppShell]");

            if (ViewModel.Action == "pop") // Login after logout or timeout
            {
                Navigation.PopModalAsync();
                Navigation.PopModalAsync();

                return;
            }
            UnsubscribeToMessages();
            // Init app after startup, new wallet or restore
            Application.Current.MainPage = new AppShell();
        }

        void Logo_Tapped(object sender, EventArgs e)
        {
            if (SecureStorageService.HasMnemonic())
                Debug.WriteLine($"[Logo_Tapped] Seed: {SecureStorageService.GetMnemonic()}");

            Application.Current.MainPage = new ControlsDemoView();
        }

        private async void BiometricsButtonClicked(object sender, EventArgs e)
        {
            var authResult = await CrossFingerprint.Current.AuthenticateAsync(
                new AuthenticationRequestConfiguration(Constants.HODL_WALLET, LocaleResources.BiometricLogin_askAuth));

            if (authResult.Authenticated)
            {
                if (ViewModel.Action == "update")
                {
                    Debug.WriteLine("[Biometrics] Authenticated!");
                    ViewModel.UpdatePin();
                }
                else
                {
                    Debug.WriteLine("[Biometrics] Logged in!");
                    ViewModel.IsLoading = true;

                    // DONE! We navigate to the root view
                    await Task.Delay(65);
                    ViewModel.StartAppShell();
                }
                return;
            }

        }

        async void UsePinButtonClicked(object sender, EventArgs e)
        {
            UnsubscribeToMessages();
            var view = new LoginView(ViewModel.Action);
            var nav = new NavigationPage(view);
            await Navigation.PushModalAsync(nav);
        }

        async void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("../../pinSettings");
        }
    }
}