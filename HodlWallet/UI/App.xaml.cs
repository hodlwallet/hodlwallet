﻿//
// App.xaml.cs
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
using System.Reactive.Linq;
using System.Threading;

using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Services;
using HodlWallet.Core.Interfaces;
using HodlWallet.UI.Views;
using HodlWallet.UI.Locale;

namespace HodlWallet.UI
{
    public partial class App : Application
    {
        IWalletService WalletService => DependencyService.Get<IWalletService>();
        IPrecioService PrecioService => DependencyService.Get<IPrecioService>();
        IDisplayCurrencyService DisplayCurrencyService => DependencyService.Get<IDisplayCurrencyService>();
        ILocalize Localize => DependencyService.Get<ILocalize>();
        ILegacySecureKeyService LegacySecureKeyService => DependencyService.Get<ILegacySecureKeyService>();
        IAuthenticationService AuthenticationService => DependencyService.Get<IAuthenticationService>();

        readonly CancellationTokenSource cts = new();

        public App()
        {
            SetupCultureInfo();

            InitializeComponent();

#if WIPE_WALLET
            SecureStorageService.RemoveAll();
#endif

            RegisterServices();

            if (SecureStorageService.UserDidSetup())
            {
                AuthenticationService.ShowLogin();

                return;
            }

            CollectExistingKeys();

            if (SecureStorageService.UserDidSetup())
            {
                AuthenticationService.ShowLogin();

                return;
            }

            MainPage = new NavigationPage(new OnboardView());
        }

        protected override void OnSleep()
        {
            AuthenticationService.LastAuth = DateTimeOffset.UtcNow;
        }

        protected override void OnResume()
        {
            if (SecureStorageService.UserDidSetup() && !AuthenticationService.IsAuthenticated && !AuthenticationService.ShowingLoginForm)
                AuthenticationService.ShowLogin(action: "pop");
        }

        protected override void OnStart()
        {
            base.OnStart();

            Observable
                .Start(PrecioService.Start, RxApp.TaskpoolScheduler)
                .Subscribe(cts.Token);

            DisplayCurrencyService.Load();

            DisplayCurrencyService.FiatCurrencyCode = "USD";
            DisplayCurrencyService.BitcoinCurrencyCode = "BTC";

            DisplayCurrencyService.Save();
        }

        void RegisterServices()
        {
            DependencyService.Register<IWalletService>();
            DependencyService.Register<IPrecioService>();
            DependencyService.Register<IShareIntent>();
            DependencyService.Register<IPermissions>();
            DependencyService.Register<IAuthenticationService>();
        }

        void SetupCultureInfo()
        {
            var ci = Localize.GetCurrentCultureInfo();

            LocaleResources.Culture = ci; // set the RESX for resource localization
            Localize.SetLocale(ci); // set the Thread for locale-aware methods
        }

        void CollectExistingKeys()
        {
            try
            {
                var mnemonic = LegacySecureKeyService.GetMnemonic();
                var pin = LegacySecureKeyService.GetPin();
                var birthday = LegacySecureKeyService.GetWalletCreationTime();

                SecureStorageService.SetMnemonic(mnemonic);
                SecureStorageService.SetPin(pin);

                // TODO initialize a legacy account as in HODL 1.0
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("There was an error while collecting deprecated keys: {0}", ex.Message));
            }
        }
    }
}
