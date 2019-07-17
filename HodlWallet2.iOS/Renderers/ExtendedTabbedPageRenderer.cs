using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using MvvmCross.Forms.Views;
using MvvmCross.Forms.Platforms.Ios.Views;

using HodlWallet2.iOS.Renderers;
using System;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(ExtendedTabbedPageRenderer))]
namespace HodlWallet2.iOS.Renderers
{
    public class ExtendedTabbedPageRenderer : TabbedRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            TabBar.BackgroundColor = UIColor.FromName("GrayBackground");

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
            item.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName("Electrolize", 12), TextColor = Color.FromHex("#757575").ToUIColor() }, UIControlState.Normal);
            item.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.FromName("Electrolize", 12), TextColor = Color.FromHex("#3C9BDF").ToUIColor() }, UIControlState.Selected);
        }

        protected override Task<Tuple<UIImage, UIImage>> GetIcon(Page page)
        {
            return base.GetIcon(page);
        }
    }
}