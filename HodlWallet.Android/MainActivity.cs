//
// MainActivity.cs
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
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Views;

using Xamarin.Forms.Platform.Android;

using Serilog;
using Rg.Plugins.Popup;

using HodlWallet.UI;
using HodlWallet.Core.Interfaces;
using Plugin.Fingerprint;

[assembly: global::Xamarin.Forms.ResolutionGroupName("HodlWallet")]
namespace HodlWallet.Droid
{
    [Activity(Label = "HodlWallet", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        IWalletService WalletService => global::Xamarin.Forms.DependencyService.Get<IWalletService>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Popup.Init(this);

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::ZXing.Net.Mobile.Forms.Android.Platform.Init();

            global::Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            CrossFingerprint.SetCurrentActivityResolver(() =>
                Xamarin.Essentials.Platform.CurrentActivity);

            global::ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);

            LoadApplication(new App());

            Xamarin.Forms.Application.Current.PageAppearing += Current_PageAppearing;

            SetupLogging();
        }

        protected override void OnPause()
        {
            Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);

            base.OnPause();
        }

        protected override void OnPostResume()
        {
            Window.ClearFlags(WindowManagerFlags.Secure);

            base.OnPostResume();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            global::Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            Popup.SendBackPressed(base.OnBackPressed);
        }

        void SetupLogging()
        {
            // Please call after LoadApplication
#if DEBUG
            WalletService.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.AndroidLog()
                .Enrich.WithProperty(Serilog.Core.Constants.SourceContextPropertyName, "HodlWallet") // Sets the Tag field.
                .CreateLogger();
#else
            WalletService.Logger = new LoggerConfiguration()
                .WriteTo.AndroidLog()
                .Enrich.WithProperty(Serilog.Core.Constants.SourceContextPropertyName, "HodlWallet") // Sets the Tag field.
                .CreateLogger();
#endif
        }

        void Current_PageAppearing(object sender, Xamarin.Forms.Page e)
        {
            if (e is Xamarin.Forms.NavigationPage) return;

            var bg = e.BackgroundColor.ToAndroid();

            if (Window.StatusBarColor != bg) Window.SetStatusBarColor(bg);
        }

        internal static MainActivity Instance { get; private set; }
    }
}
