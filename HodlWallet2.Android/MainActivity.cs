using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using Serilog;

using HodlWallet2.UI;
using HodlWallet2.Core.Interfaces;

[assembly: global::Xamarin.Forms.ResolutionGroupName("AppEffects")]

namespace HodlWallet2.Droid
{
    [Activity(Label = "HodlWallet2", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        IWalletService _WalletService => global::Xamarin.Forms.DependencyService.Get<IWalletService>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::ZXing.Net.Mobile.Forms.Android.Platform.Init();

            global::Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            global::ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);

            LoadApplication(new App());

#if DEBUG
            _WalletService.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.AndroidLog()
                .Enrich.WithProperty(Serilog.Core.Constants.SourceContextPropertyName, "HodlWallet2") // Sets the Tag field.
                .CreateLogger();
#else
            _WalletService.Logger = new LoggerConfiguration()
                .WriteTo.AndroidLog()
                .Enrich.WithProperty(Serilog.Core.Constants.SourceContextPropertyName, "HodlWallet2") // Sets the Tag field.
                .CreateLogger();
#endif
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            global::Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        internal static MainActivity Instance { get; private set; }
    }
}
