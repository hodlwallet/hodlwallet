using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using HodlWallet2.iOS.Renderers;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]
namespace HodlWallet2.iOS.Renderers
{
    public class CustomNavigationRenderer : NavigationRenderer
    {
        UIColor _TextPrimary => ((Color)Xamarin.Forms.Application.Current.Resources["TextPrimary"]).ToUIColor();
        string _SansFontName => (OnPlatform<string>)Xamarin.Forms.Application.Current.Resources["Sans-Regular"];
        string _SansBoldFontName => (OnPlatform<string>)Xamarin.Forms.Application.Current.Resources["Sans-Bold"];

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UpdateNavBar();
            UpdateNavBarItems();
        }

        void UpdateNavBar()
        {
            // Removes bottom border of the bar
            NavigationBar.Translucent = false;
            NavigationBar.ShadowImage = new UIImage();
            NavigationBar.SetBackgroundImage(new UIImage(), UIBarPosition.Any, UIBarMetrics.Default);

            // Add bold font
            var font = UIFont.FromName(_SansBoldFontName, 20);
            var attrs = new UIStringAttributes()
            {
                ForegroundColor = _TextPrimary,
                Font = font
            };

            NavigationBar.TitleTextAttributes = attrs;
        }

        void UpdateNavBarItems()
        {
            foreach (var navBarItem in NavigationBar.Items)
            {
                foreach (var backButtonItem in new UIBarButtonItem[] { navBarItem.RightBarButtonItem, navBarItem.LeftBarButtonItem })
                {
                    if (backButtonItem is null) continue;

                    var backButtonFont = UIFont.FromName(_SansFontName, 16);
                    var textAttrs = new UITextAttributes()
                    {
                        TextColor = _TextPrimary,
                        Font = backButtonFont
                    };

                    backButtonItem.SetTitleTextAttributes(textAttrs, UIControlState.Normal);
                    backButtonItem.SetTitleTextAttributes(textAttrs, UIControlState.Focused);
                    backButtonItem.SetTitleTextAttributes(textAttrs, UIControlState.Highlighted);
                    backButtonItem.SetTitleTextAttributes(textAttrs, UIControlState.Selected);
                    backButtonItem.SetTitleTextAttributes(textAttrs, UIControlState.Disabled);
                }
            }
        }
    }
}
