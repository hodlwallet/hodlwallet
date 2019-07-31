using System;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using HodlWallet2.iOS.Renderers;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace HodlWallet2.iOS.Renderers
{
    public class CustomTabbedPageRenderer : TabbedRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            // Remove color decoloring by iOS
            TabBar.Translucent = false;

            // Removes border on top of the tabbed bar
            TabBar.ShadowImage = null;
            TabBar.ClipsToBounds = true;
            TabBar.Layer.BorderWidth = 0.0f;

            // Since we add item after the tabbed appears, we subscribe to this method
            // this is very important since here the icons get moved a little bit to the top
            Tabbed.Appearing += Tabbed_Appearing;

            base.ViewWillAppear(animated);
        }

        private void Tabbed_Appearing(object sender, EventArgs e)
        {
            if (Element is TabbedPage)
            {
                for (int i = 0, count = TabBar.Items.Length; i < count; i++)
                {
                    UpdateTabBarItem(TabBar.Items[i]);
                }
            }
        }

        private void UpdateTabBarItem(UITabBarItem item)
        {
            if (item == null)
                return;

            // Move icons to the center by moving the view a little down
            // And then, we remove the titles!
            item.ImageInsets = new UIEdgeInsets(5, 0, -5, 0);

            // Remove titles from being visible
            item.Title = "";
        }
    }
}
