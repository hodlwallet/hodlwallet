//
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
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet2.Core.Services;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.UI.Views;
using HodlWallet2.UI.Locale;

namespace HodlWallet2.UI
{
    public partial class App : Application
    {
        IWalletService _WalletService => DependencyService.Get<IWalletService>();
        IPrecioService _PrecioService => DependencyService.Get<IPrecioService>();
        ILocalize _Localize => DependencyService.Get<ILocalize>();

        public App()
        {
            SetupCultureInfo();

            InitializeComponent();

            RegisterServices();

            if (UserDidSetup())
            {
                MainPage = new NavigationPage(new LoginView());
            }
            else
            {
                MainPage = new NavigationPage(new OnboardView());
            }
        }

        protected override void OnStart()
        {
            // NOTE You might think, why not move this forward?
            // the init code that inserts the logger into
            // WalletService is only run after the custructor
            // and only after all the platforms init
            Task.Run(_WalletService.InitializeWallet);
            Task.Run(_PrecioService.Init);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        void RegisterServices()
        {
            DependencyService.Register<IWalletService>();
            DependencyService.Register<IPrecioService>();
            DependencyService.Register<IShareIntent>();
            DependencyService.Register<IPermissions>();
        }

        void SetupCultureInfo()
        {
            var ci = _Localize.GetCurrentCultureInfo();

            LocaleResources.Culture = ci; // set the RESX for resource localization
            _Localize.SetLocale(ci); // set the Thread for locale-aware methods
        }

        bool UserDidSetup()
        {
            return SecureStorageService.HasPin()
                && SecureStorageService.HasMnemonic()
                && SecureStorageService.HasSeedBirthday()
                && SecureStorageService.HasWalletId()
                && SecureStorageService.HasNetwork();
        }
    }
}
