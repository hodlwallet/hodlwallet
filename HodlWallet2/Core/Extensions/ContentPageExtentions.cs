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
using System.Collections.ObjectModel;

namespace HodlWallet2.Core.Extensions
{
    public static class ContentPageExtentions
    {
        public static async Task DisplayToast(this ContentPage view, string title)
        {
            if (HasChildren(view))
            {
                var children = GetChildren(view);
                var label = new Label
                {
                    Text = "Toast Clicked!",
                    TextColor = Color.Red
                };

                Device.BeginInvokeOnMainThread(() =>
                {
                    children.Add(label);
                    //view.DisplayAlert(title, "", Constants.DISPLAY_ALERT_ERROR_BUTTON);
                });
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

        static bool HasChildren(ContentPage view)
        {
            var didHaveChildren = view.Content.GetType().GetProperty("Children") != null;

            return didHaveChildren;
        }

        static ObservableCollection<Element> GetChildren(ContentPage view)
        {
            return (ObservableCollection<Element>)view.Content.GetType().GetProperty("Children").GetValue(view.Content);
        }
    }
}
