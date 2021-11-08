//
// BiometricsView.xaml.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using HodlWallet.UI.Locale;
using HodlWallet.UI.Extensions;

namespace HodlWallet.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BiometricsView : ContentPage
    {
        public BiometricsView()
        {
            bool value = Preferences.Get("biometricsAllow", true);
            InitializeComponent();
            BiometricSwitch.IsToggled = value;

            if (Device.RuntimePlatform == Device.iOS)
            {
                Header.Title= LocaleResources.BiometricsIOS_title;
                BiometricHeader.Text = LocaleResources.FingerprintAuth_headerIOS;
                BiometricEnable.Text = LocaleResources.FingerprintAuth_switchIOS;

                // IT IS NECESSARY TO SET THE CORRECT SVG IMAGE OF FACE RECOGNITION INSTEAD OF FINGERPRINT!!
                BiometricIcon.ResourceId = "HodlWallet.UI.Assets.fingerprint_large.svg";
            }
        }

        private async void Switch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (BiometricSwitch.IsToggled)
            {
                bool availability = await CrossFingerprint.Current.IsAvailableAsync();
                if (!availability)
                {
                    await this.DisplayToast(LocaleResources.Biometrics_notAvailableWarning);
                    return;
                }
            }
            Preferences.Set("biometricsAllow", BiometricSwitch.IsToggled);
        }
    }
}