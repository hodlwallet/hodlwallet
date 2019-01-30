using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Serilog;
using Serilog.Core;
using ZXing.Mobile;

namespace HodlWallet2.Droid
{
    [Activity(Label = "HodlWallet2", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            MobileBarcodeScanner.Initialize(Application);
            global::ZXing.Net.Mobile.Forms.Android.Platform.Init();

            FormsControls.Droid.Main.Init(this);

            Wallet.Instance.Logger = new LoggerConfiguration()
                .WriteTo.AndroidLog()
                .Enrich.WithProperty(Constants.SourceContextPropertyName, "HodlWallet2") // Sets the Tag field.
                .CreateLogger();

            LoadApplication(new App());
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

            Wallet.InitializeWallet();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
