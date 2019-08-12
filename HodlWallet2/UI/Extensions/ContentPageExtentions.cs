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
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet2.Core.Utils;
using HodlWallet2.UI.Controls;
using Liviano.Utilities;

namespace HodlWallet2.UI.Extensions
{
    public static class ContentPageExtentions
    {
        public static async Task DisplayToast(this ContentPage view, string content)
        {
            if (CanAttachToView(view))
            {
                CreateToastFor(view, content);
            }
            else
            {
                Debug.WriteLine("[DisplayToast] Cannot attach toast to view, your layout must be a AbsoluteLayout");

                Device.BeginInvokeOnMainThread(() =>
                {
                    view.DisplayAlert(content, null, Constants.DISPLAY_ALERT_ERROR_BUTTON);
                });
            }

            await Task.FromResult(true);
        }

        public static async Task<bool> DisplayPrompt(this ContentPage view, string title, string message = null, string okButton = null, string cancelButton = null)
        {
            Guard.NotNull(title, nameof(title));
            Guard.NotEmpty(title, nameof(title));

            // TODO display dialog

            return true;
        }

        static bool CanAttachToView(ContentPage view)
        {
            // Supported layouts that we can add a view to.
            if (view.Content.GetType() == typeof(AbsoluteLayout)) return true;

            return false;
        }

        static void CreateToastFor(ContentPage view, string content)
        {
            var toast = new ToastView
            {
                ToastText = content,
                IsVisible = false
            };

            switch (GetContentType(view))
            {
                case "AbsoluteLayout":
                    var flexLayout = (AbsoluteLayout)view.Content;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        flexLayout.Children.Add(toast);

                        toast.IsVisible = true;
                    });

                    break;
                default:
                    throw new ArgumentException("Should not be called without an AbsoluteLayout");
            }
        }

        static string GetContentType(ContentPage view)
        {
            if (view.Content.GetType() == typeof(AbsoluteLayout)) return "AbsoluteLayout";

            throw new ArgumentException("This function should not be called if we don't have a valid layout, right now that means AbsoluteLayout");
        }
    }
}
