//
// CustomNavigationRenderer.cs
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
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using HodlWallet.iOS.Renderers;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]
namespace HodlWallet.iOS.Renderers
{
    public class CustomNavigationRenderer : NavigationRenderer
    {
        UIColor _TextPrimary => ((Color)Xamarin.Forms.Application.Current.Resources["TextPrimary"]).ToUIColor();
        string _SansFontName => (OnPlatform<string>)Xamarin.Forms.Application.Current.Resources["Sans-Regular"];
        //string _SansBoldFontName => (OnPlatform<string>)Xamarin.Forms.Application.Current.Resources["Sans-Bold"];

        UIControlState[] _ControlStates =
        {
            UIControlState.Normal,
            UIControlState.Focused,
            UIControlState.Highlighted,
            UIControlState.Selected,
            UIControlState.Disabled
        };

        UIFont _TitleFont => UIFont.FromName(_SansFontName, 20);
        UIStringAttributes _TitleStringAttributes => new UIStringAttributes()
        {
            ForegroundColor = _TextPrimary,
            Font = _TitleFont
        };

        UIFont _ItemFont => UIFont.FromName(_SansFontName, 16);
        UITextAttributes _ItemTextAttributes => new UITextAttributes()
        {
            TextColor = _TextPrimary,
            Font = _ItemFont
        };

        public CustomNavigationRenderer()
        {
            UpdateNavBarStyles();
            UpdateNavBarItemStyles();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UpdateNavBarStyles();
            UpdateNavBarItemStyles();

            UpdateNavBar();
            UpdateNavBarItems();
        }

        void UpdateNavBarStyles()
        {
            UINavigationBar.Appearance.Translucent = false;
            UINavigationBar.Appearance.ShadowImage = new UIImage();
            UINavigationBar.Appearance.SetBackgroundImage(new UIImage(), UIBarPosition.Any, UIBarMetrics.Default);

            UINavigationBar.Appearance.TitleTextAttributes = _TitleStringAttributes;
        }

        void UpdateNavBarItemStyles()
        {
            foreach (var controlState in _ControlStates)
                UIBarButtonItem.Appearance.SetTitleTextAttributes(_ItemTextAttributes, controlState);
        }

        void UpdateNavBar()
        {
            // Removes bottom border of the bar
            NavigationBar.Translucent = false;
            NavigationBar.ShadowImage = new UIImage();
            NavigationBar.SetBackgroundImage(new UIImage(), UIBarPosition.Any, UIBarMetrics.Default);

            NavigationBar.TitleTextAttributes = _TitleStringAttributes;
        }

        void UpdateNavBarItems()
        {
            foreach (var navBarItem in NavigationBar.Items)
            {
                foreach (var buttonItem in new UIBarButtonItem[] { navBarItem.BackBarButtonItem, navBarItem.RightBarButtonItem, navBarItem.LeftBarButtonItem })
                {
                    if (buttonItem is null) continue;

                    foreach (var controlState in _ControlStates)
                        buttonItem.SetTitleTextAttributes(_ItemTextAttributes, controlState);
                }
            }
        }
    }
}
