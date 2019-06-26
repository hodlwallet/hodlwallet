using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using HodlWallet2.iOS.Renderers;
using HodlWallet2.Core.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(ShareIntent))]
namespace HodlWallet2.iOS.Renderers
{
    public class ShareIntent : IShareIntent
    {
        public void TextShareIntent(string text)
        {
            var activityController = new UIActivityViewController(new NSObject[] { UIActivity.FromObject(text) }, null);
            var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            while (topController.PresentedViewController != null)
            {
                topController = topController.PresentedViewController;
            }

            topController.PresentViewController(activityController, true, null);
        }

        public void QRTextShareIntent(string address)
        {
            var barcodeWriter = new ZXing.Mobile.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 300
                }
            };

            var bitmap = barcodeWriter.Write(address);

            var addr = NSObject.FromObject(address);
            var img = NSObject.FromObject(bitmap);

            var activityItems = new[] { addr, img };
            var activityController = new UIActivityViewController(activityItems, null);

            var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            while(topController.PresentedViewController != null)
            {
                topController = topController.PresentedViewController;
            }

            topController.PresentViewController(activityController, true, null);
        }
    }
}