//
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

namespace HodlWallet.UI.Views
{
    public partial class LoginView : ContentPage
    {
        readonly uint incorrectPinAnimationTimeout = 50;

        Color DigitOnColor => (Color)Application.Current.Resources["FgSuccess"];
        
        Color DigitOffColor => (Color)Application.Current.Resources["Fg5"];
        
        LoginViewModel ViewModel => (LoginViewModel)BindingContext;

        public LoginView(string action = null)
        {
            InitializeComponent();
            
            ViewModel.Action = action;

	        CheckBiometricsAvailabilityAsync();
            SubscribeToMessages();

            ViewModel.LastLogin = "pin";

            if (ViewModel.Action != "update")
            {
                NavigationPage.SetHasNavigationBar(this, false);

                return;
            }

            LogoFront.IsVisible = false;

            Header.Text = Locale.LocaleResources.Pin_updateHeader;
            Title = Locale.LocaleResources.Pin_updateTitle;

            NavigationPage.SetHasNavigationBar(this, true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.LoginFormVisible = true;

            var biometricsAllow = Preferences.Get("biometricsAllow", false);

            FingerprintButton.IsVisible = ViewModel.BiometricsAvailable && biometricsAllow;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (ViewModel.IsLoading) ViewModel.IsLoading = false;

            ViewModel.LoginFormVisible = false;
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<LoginViewModel, int>(this, "DigitAdded", DigitAdded);
            MessagingCenter.Subscribe<LoginViewModel, int>(this, "DigitRemoved", DigitRemoved);
            MessagingCenter.Subscribe<LoginViewModel>(this, "IncorrectPinAnimation", IncorrectPinAnimation);
            MessagingCenter.Subscribe<LoginViewModel>(this, "StartAppShell", async (vm) => await StartAppShell(vm));
            MessagingCenter.Subscribe<LoginViewModel>(this, "UpdatePin", UpdatePin);
            MessagingCenter.Subscribe<LoginViewModel>(this, "ResetPin", ResetPin);
        }


        void UnsubscribeToMessages()
        {
            MessagingCenter.Unsubscribe<LoginViewModel, int>(this, "DigitAdded");
            MessagingCenter.Unsubscribe<LoginViewModel, int>(this, "DigitRemoved");
            MessagingCenter.Unsubscribe<LoginViewModel, int>(this, "IncorrectPinAnimation");
            MessagingCenter.Unsubscribe<LoginViewModel, int>(this, "ResetPin");
            MessagingCenter.Unsubscribe<LoginViewModel, int>(this, "StartAppShell");
            MessagingCenter.Unsubscribe<LoginViewModel, int>(this, "UpdatePin");
        }

        async void CheckBiometricsAvailabilityAsync()
        {
            ViewModel.BiometricsAvailable = await CrossFingerprint.Current.IsAvailableAsync();
        }

        void DigitAdded(LoginViewModel _, int index)
        {
            Debug.WriteLine($"[SubscribeToMessage][DigitAdded] Add digit: {index}");

            ColorDigitTo(index, DigitOnColor);
        }

        void DigitRemoved(LoginViewModel _, int index)
        {
            Debug.WriteLine($"[SubscribeToMessage][DigitRemoved] Remove digit: {index}");

            ColorDigitTo(index, DigitOffColor);
        }

        async void IncorrectPinAnimation(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][IncorrectPinAnimation]");

            // Shake ContentView Re-Enter PIN
            await InputGrid.TranslateTo(-15, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(15, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(-10, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(10, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(-5, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(5, 0, incorrectPinAnimationTimeout);

            InputGrid.TranslationX = 0;

            await Task.Delay(500);
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

        void ResetPin(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][ResetPin]");

            Pin1.Fill = DigitOffColor;
            Pin2.Fill = DigitOffColor;
            Pin3.Fill = DigitOffColor;
            Pin4.Fill = DigitOffColor;
            Pin5.Fill = DigitOffColor;
            Pin6.Fill = DigitOffColor;
        }

        void ColorDigitTo(int index, Color color)
        {
            switch (index)
            {
                case 1:
                    Pin1.Fill = color;
                    break;
                case 2:
                    Pin2.Fill = color;
                    break;
                case 3:
                    Pin3.Fill = color;
                    break;
                case 4:
                    Pin4.Fill = color;
                    break;
                case 5:
                    Pin5.Fill = color;
                    break;
                case 6:
                    Pin6.Fill = color;
                    break;
            }
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
            UnsubscribeToMessages();
            var view = new BiometricLoginView(ViewModel.Action);
            if (ViewModel.Action == "update")
            {
                await Navigation.PushAsync(view);
            }
            else
            {
                var nav = new NavigationPage(view);
                await Navigation.PushModalAsync(nav);
            }
        }

        async void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
