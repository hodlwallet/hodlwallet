//
// AuthenticationService.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
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

using Xamarin.Forms;

using HodlWallet.Core.Interfaces;
using HodlWallet.Core.Services;
using HodlWallet.UI.Views;
using HodlWallet.UI;
using HodlWallet.Core.ViewModels;
using Xamarin.Essentials;

using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System.Diagnostics;

[assembly: Dependency(typeof(AuthenticationService))]
namespace HodlWallet.Core.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
#if DEBUG
        const int EXPIRATION_TIME = 10_000; // 10 seconds in ms
#else
        const int EXPIRATION_TIME = 300_000; // 5 mins in ms
#endif

        public DateTimeOffset LastAuth { get; set; }

        bool isAuthenticated = false;
        public bool IsAuthenticated
        {
            get
            {
                if (!isAuthenticated) return isAuthenticated;

                var now = DateTimeOffset.UtcNow;
                var expiration = LastAuth + TimeSpan.FromMilliseconds(EXPIRATION_TIME);

                if (expiration < now)
                    isAuthenticated = false;

                return isAuthenticated;
            }

            set
            {
                if (value) LastAuth = DateTimeOffset.UtcNow;

                isAuthenticated = value;
            }
        }


        public bool ShowingLoginForm { get; set; }
        bool biometricsAvailable;
        public bool BiometricsAvailable
        {
            get
            {
                return Preferences.Get("biometricsAvailable", false);
            }

            set
            {
                biometricsAvailable = value;
                Preferences.Set("biometricsAvailable", biometricsAvailable);
            }
        }





        public async void ShowLogin(string action = null)
        {
            string lastLogin = Preferences.Get("lastLogin", "pin");
            bool biometricsAllow = Preferences.Get("biometricsAllow", false);
            BiometricsAvailable = await CrossFingerprint.Current.IsAvailableAsync();

            if (biometricsAllow & (lastLogin == "biometric" & BiometricsAvailable))
            {
                var view = new BiometricLoginView(action);

                if (action == "pop")
                {
                    if (Application.Current.MainPage is AppShell appShell)
                        await appShell.Navigation.PushModalAsync(view);

                    return;
                }

                Application.Current.MainPage = view;
            }
            else
            {
                var view = new LoginView(action);

                if (action == "pop")
                {
                    if (Application.Current.MainPage is AppShell appShell)
                        await appShell.Navigation.PushModalAsync(view);

                    return;
                }
                
                Application.Current.MainPage = view;
            }
        }

        public bool Authenticate(string input)
        {
            IsAuthenticated = input == SecureStorageService.GetPin();

            return IsAuthenticated;
        }
    }
}
