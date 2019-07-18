using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using MvvmCross.Forms.Views;
using MvvmCross.Forms.Platforms.Ios.Views;

using HodlWallet2.iOS.Renderers;
using System;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace HodlWallet2.iOS.Renderers
{
    public class CustomTabbedPageRenderer : TabbedRenderer
    {
        UIColor GRAY_BACKGROUND { get; } = new UIColor(red: 0.13f, green: 0.13f, blue: 0.13f, alpha: 1.0f);

        public override void ViewWillAppear(bool animated)
        {
            TabBar.BarTintColor = GRAY_BACKGROUND;

            TabBar.Translucent = false;
            TabBar.Layer.BorderWidth = 0.5f;
            TabBar.Layer.BorderColor = UIColor.Clear.CGColor;

            TabBar.ClipsToBounds = true;

            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            if (TabBar?.Items == null)
                return;

            var tabs = Element as TabbedPage;
            if (tabs != null)
            {
                for (int i = 0; i < TabBar.Items.Length; i++)
                {
                    UpdateTabBarItem(TabBar.Items[i], tabs.Children[i].IconImageSource);
                }
            }

            base.ViewDidAppear(animated);
        }

        private void UpdateTabBarItem(UITabBarItem item, ImageSource icon)
        {
            if (item == null || icon == null)
                return;

            // Set the font for the title.
            item.SetTitleTextAttributes(new UITextAttributes() {
                Font = UIFont.FromName("Electrolize", 12),
                TextColor = Color.FromHex("#757575").ToUIColor()
            }, UIControlState.Normal);

            item.SetTitleTextAttributes(new UITextAttributes() {
                Font = UIFont.FromName("Electrolize", 12),
                TextColor = Color.FromHex("#3C9BDF").ToUIColor()
            }, UIControlState.Selected);
        }

        protected override Task<Tuple<UIImage, UIImage>> GetIcon(Page page)
        {
            return base.GetIcon(page);
        }
    }
}