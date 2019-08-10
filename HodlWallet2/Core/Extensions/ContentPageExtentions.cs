//
// ContentPageExtentions.cs
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
using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet2.Core.Utils;

namespace HodlWallet2.Core.Extensions
{
    public static class ContentPageExtentions
    {
        public static async Task DisplayToast(this ContentPage view, string title)
        {
            if (CanAttachToView(view))
            {
                CreateToastFor(view);
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    view.DisplayAlert(title, "", Constants.DISPLAY_ALERT_ERROR_BUTTON);
                });
            }

            await Task.FromResult(true);
        }

        static bool CanAttachToView(ContentPage view)
        {
            // Supported layouts that we can add a view to.
            if (view.Content.GetType() == typeof(FlexLayout)) return true;
            if (view.Content.GetType() == typeof(StackLayout)) return true;
            if (view.Content.GetType() == typeof(AbsoluteLayout)) return true;
            if (view.Content.GetType() == typeof(RelativeLayout)) return true;
            if (view.Content.GetType() == typeof(Grid)) return true;

            return false;
        }

        static void CreateToastFor(ContentPage view)
        {
            var label = new Label
            {
                Text = "Toast Clicked!",
                TextColor = Color.Red
            };

            switch(GetContentType(view))
            {
                case "FlexLayout":
                    var flexLayout = (FlexLayout)view.Content;

                    flexLayout.Children.Add(label);
                    break;
                default:
                    throw new ArgumentException("Should not be called without knowing it can be used");
            }
        }

        static string GetContentType(ContentPage view)
        {
            if (view.Content.GetType() == typeof(FlexLayout)) return "FlexLayout";
            if (view.Content.GetType() == typeof(StackLayout)) return "StackLayout";
            if (view.Content.GetType() == typeof(AbsoluteLayout)) return "AbsoluteLayout";
            if (view.Content.GetType() == typeof(RelativeLayout)) return "RelativeLayout";
            if (view.Content.GetType() == typeof(Grid)) return "Grid";

            throw new ArgumentException("This function should not be called if we don't have a valid layout");
        }
    }
}
