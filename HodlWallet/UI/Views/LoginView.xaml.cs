//
// LoginView.xaml.cs
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

using HodlWallet.Core.ViewModels;
using HodlWallet.Core.Services;
using HodlWallet.UI.Views.Demos;

namespace HodlWallet.UI.Views
{
    public partial class LoginView : ContentPage
    {
        readonly uint incorrectPinAnimationTimeout = 50;

        Color DigitOnColor => (Color)Application.Current.Resources["InputPinOn"];
        Color DigitOffColor => (Color)Application.Current.Resources["InputPinOff"];

        LoginViewModel ViewModel => (LoginViewModel)BindingContext;

        public LoginView(string action = null)
        {
            InitializeComponent();
            SubscribeToMessages();

            ViewModel.Action = action;

            if (ViewModel.Action == "update")
            {
                LogoFront.IsVisible = false;
                Header.Text = Locale.LocaleResources.Pin_updateHeader;
                CancelButton.IsEnabled = true;
                CancelButton.IsVisible = true;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (ViewModel.IsLoading) ViewModel.IsLoading = false;
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<LoginViewModel, int>(this, "DigitAdded", DigitAdded);
            MessagingCenter.Subscribe<LoginViewModel, int>(this, "DigitRemoved", DigitRemoved);
            MessagingCenter.Subscribe<LoginViewModel>(this, "IncorrectPinAnimation", IncorrectPinAnimation);
            MessagingCenter.Subscribe<LoginViewModel>(this, "StartAppShell", StartAppShell);
            MessagingCenter.Subscribe<LoginViewModel>(this, "ResetPin", ResetPin);
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

        void StartAppShell(LoginViewModel _)
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

        void ResetPin(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][ResetPin]");

            ColorDigitTo(Pin1, DigitOffColor);
            ColorDigitTo(Pin2, DigitOffColor);
            ColorDigitTo(Pin3, DigitOffColor);
            ColorDigitTo(Pin4, DigitOffColor);
            ColorDigitTo(Pin5, DigitOffColor);
            ColorDigitTo(Pin6, DigitOffColor);
        }

        void ColorDigitTo(int index, Color color)
        {
            var pin = (BoxView)FindByName($"Pin{index}");

            ColorDigitTo(pin, color);
        }

        void ColorDigitTo(BoxView pin, Color color)
        {
            pin.Color = color;
        }

        void Logo_Tapped(object sender, EventArgs e)
        {
            if (SecureStorageService.HasMnemonic())
                Debug.WriteLine($"[Logo_Tapped] Seed: {SecureStorageService.GetMnemonic()}");

            Application.Current.MainPage = new ControlsDemoView();
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
                Navigation.PopModalAsync();
        }
    }
}
