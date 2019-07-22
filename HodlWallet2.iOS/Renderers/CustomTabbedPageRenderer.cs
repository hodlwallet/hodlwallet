using System;
using System.Threading.Tasks;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using HodlWallet2.iOS.Renderers;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace HodlWallet2.iOS.Renderers
{
    public class CustomTabbedPageRenderer : TabbedRenderer
    {
        readonly Color GRAY_BACKGROUND = Color.FromHex("#212121");

        public CustomTabbedPageRenderer()
        {
            TabBar.TintColor = UIColor.Red;
            TabBar.BackgroundColor = UIColor.Red;
        }

        public override void ViewWillAppear(bool animated)
        {
            TabBar.BarTintColor = GRAY_BACKGROUND.ToUIColor();

            // Removes border on top of bar
            TabBar.Translucent = false;
            TabBar.Layer.BorderWidth = 0.5f;
            TabBar.Layer.BorderColor = GRAY_BACKGROUND.ToUIColor().CGColor;

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

            item.ImageInsets = new UIEdgeInsets(5, 0, -5, 0);

            // Remove titles
            item.Title = "";
        }
    }
}
