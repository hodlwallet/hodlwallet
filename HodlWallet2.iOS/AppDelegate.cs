using Foundation;
using HodlWallet2.Core.Interfaces;
using MvvmCross;
using MvvmCross.Forms.Platforms.Ios.Core;
using UIKit;

using Serilog;
using Serilog.Core;

namespace HodlWallet2.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxFormsApplicationDelegate<Setup, Core.App, App>
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            Rg.Plugins.Popup.Popup.Init();
            FormsControls.Touch.Main.Init();

            //UINavigationBar.Appearance.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            //UINavigationBar.Appearance.ShadowImage = new UIImage();
            //UINavigationBar.Appearance.BackgroundColor = UIColor.Clear;
            //UINavigationBar.Appearance.TintColor = UIColor.White;
            //UINavigationBar.Appearance.BarTintColor = UIColor.Clear;
            //UINavigationBar.Appearance.Translucent = true;

            return base.FinishedLaunching(app, options);
        }
    }
}
