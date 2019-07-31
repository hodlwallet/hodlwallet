using UIKit;
using Foundation;

using HodlWallet2.iOS.Renderers;
using HodlWallet2.Core.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(ShareIntent))]
namespace HodlWallet2.iOS.Renderers
{
    public class ShareIntent : IShareIntent
    {
        const int QR_CODE_SIZE = 300;

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
                    Width = QR_CODE_SIZE,
                    Height = QR_CODE_SIZE
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