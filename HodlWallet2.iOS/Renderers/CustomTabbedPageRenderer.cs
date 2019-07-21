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
        string GRAY_HEX = "#212121";
        UIColor GRAY_BACKGROUND { get; } = new UIColor(red: 0.13f, green: 0.13f, blue: 0.13f, alpha: 1.0f);
        string ORANGE_HEX = "#c89e26";
        UIColor ORANGE { get; } = new UIColor(red:0.85f, green:0.67f, blue:0.16f, alpha:1.0f);

        public CustomTabbedPageRenderer()
        {
            TabBar.TintColor = UIColor.Red;
            TabBar.BackgroundColor = UIColor.Red;
        }

        public override void ViewWillAppear(bool animated)
        {
            // Sets tabbar background color
            // This is set on XAML I think is better because Android will pick it up.
            //Tabbed.SelectedTabColor = Color.FromHex(ORANGE_HEX);
            //Tabbed.BackgroundColor = Color.FromHex(GRAY_HEX);

            TabBar.BarTintColor = GRAY_BACKGROUND;

            // Removes border on top of bar
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

            // Remove titles
            item.Title = "";

            //icon.

            // Set the font for the title.
            //item.SetTitleTextAttributes(new UITextAttributes()
            //{
            //    Font = UIFont.FromName("Electrolize", 12),
            //    TextColor = Color.FromHex("#757575").ToUIColor()
            //}, UIControlState.Normal);

            //item.SetTitleTextAttributes(new UITextAttributes()
            //{
            //    Font = UIFont.FromName("Electrolize", 12),
            //    TextColor = ORANGE
            //}, UIControlState.Selected);
        }

        //protected override Task<Tuple<UIImage, UIImage>> GetIcon(Page page)
        //{
        //    UIImage icon = new UIImage();
        //    UIImage iconSelected = new UIImage();

        //    switch (page.GetType().Name)
        //    {
        //        case "SendTabView":

        //            icon = UIImage.FromBundle("send_tab.png");
        //            iconSelected = UIImage.FromBundle("send_tab_selected.png");

        //            break;
        //        case "ReceiveTabView":
        //            return base.GetIcon(page);
        //        case "HomeTabView":
        //            return base.GetIcon(page);
        //        case "SettingsTabView":
        //            return base.GetIcon(page);
        //        default:
        //            throw new ArgumentException(
        //                $"Did you create a new tab of type {page.GetType().Name} Add its Icon to the iOS CustomTabbedPageRenderer?"
        //            );
        //    }

        //    //icon.get

        //    return Task.FromResult(new Tuple<UIImage, UIImage>(icon, iconSelected));
        //}
    }
}
