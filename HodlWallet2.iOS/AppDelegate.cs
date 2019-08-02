using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using HodlWallet2.UI;
using HodlWallet2.Core.Services;
using Serilog;

namespace HodlWallet2.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
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
            global::Xamarin.Forms.Forms.Init();
            global::ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            LoadApplication(new App());

#if DEBUG
            WalletService.Instance.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.NSLog()
                .Enrich.WithProperty(Serilog.Core.Constants.SourceContextPropertyName, "HodlWallet2") // Sets the tag fields
                .CreateLogger();
#else
                WalletService.Instance.Logger = new LoggerConfiguration()
                    .WriteTo.NSLog()
                    .Enrich.WithProperty(Serilog.Core.Constants.SourceContextPropertyName, "HodlWallet2") // Sets the tag fields
                    .CreateLogger();
#endif

            return base.FinishedLaunching(app, options);
        }
    }
}
