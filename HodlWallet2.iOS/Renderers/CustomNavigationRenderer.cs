using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using HodlWallet2.iOS.Renderers;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]
namespace HodlWallet2.iOS.Renderers
{
    public class CustomNavigationRenderer : NavigationRenderer
    {
        UIColor _TextPrimary = Color.FromHex("#F5F7FA").ToUIColor();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Removes top border
            NavigationBar.Translucent = false;
            NavigationBar.ShadowImage = null;

            // Add bold font
            var font = UIFont.FromName("Sans-Bold", 20);
            var attrs = new UIStringAttributes()
            {
                ForegroundColor = _TextPrimary,
                Font = font
            };

            NavigationBar.TitleTextAttributes = attrs;

            if (Toolbar.Items is null) return;

            // TODO this doesn't work
            var attrsItem = new UITextAttributes()
            {
                TextColor = _TextPrimary,
                Font = font
            };

            foreach (var item in Toolbar.Items)
                item.SetTitleTextAttributes(attrsItem, UIControlState.Normal);
        }
    }
}
