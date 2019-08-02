using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using ZXing.Mobile;
using HodlWallet2.Core.Services;
using Serilog;
using HodlWallet2.UI;
using HodlWallet2.Core.Interfaces;
using Xamarin.Forms;

[assembly: global::Xamarin.Forms.ResolutionGroupName("AppEffects")]

namespace HodlWallet2.Droid
{
    [Activity(Label = "HodlWallet2", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        IWalletService _WalletService => DependencyService.Get<IWalletService>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::ZXing.Net.Mobile.Forms.Android.Platform.Init();

            global::Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            MobileBarcodeScanner.Initialize(Application);

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

            base.OnCreate(savedInstanceState);

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        internal static MainActivity Instance { get; private set; }
    }
}
