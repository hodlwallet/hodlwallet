//
// SecuritySettingsView.xaml.cs
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
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SecuritySettingsView : ContentPage
    {
        public SecuritySettingsView()
        {
            InitializeComponent();
        }

        async void BackupMnemonic_Clicked(object sender, EventArgs e)
        {
            var view = new BackupView(action: "close");
            var nav = new NavigationPage(view);

            await Navigation.PushModalAsync(nav);
        }

        async void PinButton_Clicked(object sender, EventArgs e)
        {
            var lastLogin = Preferences.Get("lastLogin", "pin");
            var biometricsAllow = Preferences.Get("biometricsAllow", false);
            var availability = Preferences.Get("biometricsAvailable", false);

            ContentPage view;
            if (biometricsAllow && (lastLogin == "biometric" && availability))
                view = new BiometricLoginView("update");
            else
                view = new LoginView("update");

            await Navigation.PushModalAsync(new NavigationPage(view));
        }

        async void Biometrics_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BiometricsSettingsView());
        }
    }
}