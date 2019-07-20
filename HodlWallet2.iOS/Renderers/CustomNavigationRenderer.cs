using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System;
using HodlWallet2.Renderers;
using HodlWallet2.iOS.Renderers;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]
namespace HodlWallet2.iOS.Renderers
{
    public class CustomNavigationRenderer : NavigationRenderer
    {
        UIColor GRAY_BACKGROUND { get; } = new UIColor(red: 0.13f, green: 0.13f, blue: 0.13f, alpha: 1.0f);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationBar.Translucent = false;

            NavigationBar.BackgroundColor = UIColor.White;
            NavigationBar.ShadowImage = new UIImage();
            NavigationBar.BarTintColor = GRAY_BACKGROUND;
            NavigationBar.TintColor = UIColor.White;

            var font = UIFont.FromName("Electrolize", 22);
            var descriptor =  font.FontDescriptor.CreateWithAttributes(new UIFontAttributes
            {
                Traits = new UIFontTraits() { SymbolicTrait = UIFontDescriptorSymbolicTraits.Bold }
            });
            var boldFont = UIFont.FromDescriptor(descriptor, font.PointSize + 2);

            NavigationBar.TitleTextAttributes = new UIStringAttributes()
            {
                Font = boldFont,
                ForegroundColor = UIColor.White
            };

            //UINavigationBar.Appearance.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            //UINavigationBar.Appearance.ShadowImage = new UIImage();
            //UINavigationBar.Appearance.BackgroundColor = UIColor.Clear;
            //UINavigationBar.Appearance.TintColor = UIColor.White;
            //UINavigationBar.Appearance.BarTintColor = UIColor.Clear;
            //UINavigationBar.Appearance.Translucent = false;
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }
    }
}
