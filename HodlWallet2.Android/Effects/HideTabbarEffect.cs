//
// HideTabbarEffect.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2019 
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
using System.Linq;

using Android.Support.Design.Widget;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

using HodlWallet.Droid.Effects;
using HodlWallet.Droid.Extensions;

[assembly: ExportEffect(typeof(HideTabbarEffect), nameof(HideTabbarEffect))]
namespace HodlWallet.Droid.Effects
{
    public class HideTabbarEffect : PlatformEffect
    {
        TabbedPageRenderer _TabbedPageRenderer => (Control ?? Container) as TabbedPageRenderer;

        protected override void OnAttached()
        {
            ToggleTo(hidden: true);
        }

        protected override void OnDetached()
        {
            ToggleTo(hidden: false);
        }

        void ToggleTo(bool hidden)
        {
            var renderer = (Control ?? Container) as TabbedPageRenderer;

            var children = renderer?.ViewGroup?.RetrieveAllChildViews();
            if (children?.FirstOrDefault(x => x is BottomNavigationView) is BottomNavigationView bottomNav)
            {
                bottomNav.Visibility = hidden
                    ? Android.Views.ViewStates.Invisible
                    : Android.Views.ViewStates.Visible;
            }
        }
    }
}
