
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using Xamarin.Forms;

using MvvmCross.Forms.Platforms.Android.Views;
using ZXing.Mobile;

[assembly: ResolutionGroupName("AppEffects")]

namespace HodlWallet2.Droid
{
    [Activity(Label = "HodlWallet2", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : MvxFormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::ZXing.Net.Mobile.Forms.Android.Platform.Init();

            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            MobileBarcodeScanner.Initialize(Application);
            FormsControls.Droid.Main.Init(this);

            base.OnCreate(savedInstanceState);
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
