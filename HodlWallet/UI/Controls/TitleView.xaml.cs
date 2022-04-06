//
// TitleView.xaml.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2022 HODL Wallet
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
// THE SOFTWARE.using Xamarin.Forms;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TitleView : StackLayout
    {
        public TitleView()
        {
            InitializeComponent();
        }

        void AddNeededMarginToCenterInIOS()
        {
            var view = Parent as ContentPage;

            view.Appearing += ParentContentPage_Appearing;
        }

        private void ParentContentPage_Appearing(object sender, EventArgs e)
        {
            var view = sender as ContentPage;
            var hasNavigationPage = NavigationPage.GetHasNavigationBar(view);
            var hasNavigationBackButton = NavigationPage.GetHasBackButton(view);

            // If we do not have a navigation then we don't need to adjust anything
            if (!hasNavigationPage) return;

            var items = view.ToolbarItems.Where(item =>
            {
                if (item is HideableToolbarItem)
                    return (item as HideableToolbarItem).IsVisible;

                return item.IsEnabled;
            }).ToArray();
            var itemLen = items.Length;

            // If it's a shell item, it's on a shell, and will have a menu
            var hasMenu = view.Parent is BaseShellItem;

            // has menu and on the other side an item
            // center is enough
            if (hasMenu && itemLen == 1) return;

            var left = 0;
            var right = -(40 * itemLen);
            Thickness thickness;
            if (hasMenu || hasNavigationBackButton)
            {
                left = -40;
            }
            else
            {
                left = 0;
            }
            thickness = new Thickness(left, 0, right, 0);

            titleLabel.Margin = thickness;
        }

        protected override void OnParentSet()
        {
            // TODO This could make them center on iOS like I explain
            // in xaml
            //if (Device.RuntimePlatform == Device.iOS)
            //    AddNeededMarginToCenterInIOS();
        }
    }
}