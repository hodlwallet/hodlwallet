using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Serilog;
using Serilog.Core;

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
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FormsControls.Droid.Main.Init(this);

            Wallet.Instance.Logger = new LoggerConfiguration()
                .WriteTo.AndroidLog()
                .Enrich.WithProperty(Constants.SourceContextPropertyName, "HodlWallet2") // Sets the Tag field.
                .CreateLogger();

            LoadApplication(new App());
        }
    }
}