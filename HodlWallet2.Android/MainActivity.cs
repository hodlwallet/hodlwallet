using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using Xamarin.Forms;

using ZXing.Mobile;
using HodlWallet2.Core.Services;
using Serilog;
using HodlWallet2.UI;

[assembly: ResolutionGroupName("AppEffects")]

namespace HodlWallet2.Droid
{
    [Activity(Label = "HodlWallet2", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::ZXing.Net.Mobile.Forms.Android.Platform.Init();

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            MobileBarcodeScanner.Initialize(Application);

#if DEBUG
            WalletService.Instance.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.AndroidLog()
                .Enrich.WithProperty(Serilog.Core.Constants.SourceContextPropertyName, "HodlWallet2") // Sets the Tag field.
                .CreateLogger();
#else
                WalletService.Instance.Logger = new LoggerConfiguration()
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
