//
// CustomShellRenderer.cs
//
// Copyright (c) 2021 HODL Wallet
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
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using HodlWallet.iOS.Renderers;
using HodlWallet.UI;
using UIKit;

[assembly: ExportRenderer(typeof(AppShell), typeof(CustomShellRenderer))]
namespace HodlWallet.iOS.Renderers
{
    public class CustomShellRenderer : ShellRenderer
    {
        protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
        {
            var renderer = base.CreateShellSectionRenderer(shellSection) as ShellSectionRenderer;

            renderer.NavigationBar.Translucent = false;
            renderer.NavigationBar.ShadowImage = null;
            //renderer.NavigationController.NavigationBar.ShadowImage = null;

            return renderer;
        }

        protected override IShellItemRenderer CreateShellItemRenderer(ShellItem item)
        {
            var renderer = base.CreateShellItemRenderer(item) as ShellItemRenderer;

            renderer.TabBar.Translucent = false;
            renderer.TabBar.ShadowImage = null;
            renderer.TabBar.ClipsToBounds = true;
            renderer.TabBar.Layer.BorderWidth = 0.0f;

            foreach (var tabItem in renderer.TabBar.Items)
                tabItem.ImageInsets = new UIEdgeInsets(5, 0, -5, 0);

            return renderer;
        }
    }
}
