//
// CustomTabbedPageRenderer.cs
//
// Copyright (c) 2019 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
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
            // Add these next time we dynamically add items to the tabbar
            //Tabbed.Appearing += Tabbed_Appearing;

            UpdateAllTabBarItems();

            base.ViewWillAppear(animated);
        }

        void Tabbed_Appearing(object sender, EventArgs e)
        {
            UpdateAllTabBarItems();
        }

        void UpdateAllTabBarItems()
        {
            if (Element is TabbedPage)
            {
                for (int i = 0, count = TabBar.Items.Length; i < count; i++)
                {
                    UpdateTabBarItem(TabBar.Items[i]);
                }
            }
        }

        void UpdateTabBarItem(UITabBarItem item)
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
