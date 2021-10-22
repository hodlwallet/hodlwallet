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
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet.Core.Utils;
using HodlWallet.UI.Controls;
using HodlWallet.UI.Locale;
using Liviano.Utilities;
using Rg.Plugins.Popup.Extensions;

namespace HodlWallet.UI.Extensions
{
    public static class ContentPageExtentions
    {
        public static async Task DisplayToast(this ContentPage view, string content)
        {
            var promptTaskSource = new TaskCompletionSource<bool>();

            if (CanAttachToView(view))
            {
                promptTaskSource = CreateToastFor(view, content);
            }
            else
            {
                Debug.WriteLine("[DisplayToast] Cannot attach toast to view, your layout must be a AbsoluteLayout");

                _ = view.DisplayAlert(Constants.HODL_WALLET, content, LocaleResources.Error_ok);

                promptTaskSource.SetResult(true);
            }

            await promptTaskSource.Task;
        }

        public static async Task<bool> DisplayPrompt(this ContentPage view, string title, string message = null, string okButton = null, string cancelButton = null)
        {
            Guard.NotNull(title, nameof(title));
            Guard.NotEmpty(title, nameof(title));

            var promptTaskSource = new TaskCompletionSource<bool>();
            var prompt = new PromptView(title, message, okButton, cancelButton);

            await view.Navigation.PushPopupAsync(prompt);

            prompt.Responded += (object sender, bool res) =>
            {
                promptTaskSource.SetResult(res);
            };

            return await promptTaskSource.Task;
        }

        static bool CanAttachToView(ContentPage view)
        {
            // Supported layouts that we can add a view to.
            if (view.Content.GetType() == typeof(AbsoluteLayout)) return true;

            return false;
        }

        static TaskCompletionSource<bool> CreateToastFor(ContentPage view, string content)
        {
            if (!(GetContentType(view) == "AbsoluteLayout"))
                throw new ArgumentException("Should not be called without an AbsoluteLayout");

            var taskSource = new TaskCompletionSource<bool>();
            var toast = new ToastView { ToastText = content };
            var layout = (AbsoluteLayout)view.Content;

            var prevToast = layout.Children.FirstOrDefault(
                (View child) => child.GetType() == typeof(ToastView)
            );

            if (prevToast != null) layout.Children.Remove(prevToast);

            layout.Children.Add(toast);

            toast.Init();

            toast.OnClosed += (object sender, bool res) =>
            {
                layout.Children.Remove(toast);
            };

            taskSource.SetResult(true);
            return taskSource;
        }

        static string GetContentType(ContentPage view)
        {
            if (view.Content.GetType() == typeof(AbsoluteLayout)) return "AbsoluteLayout";

            throw new ArgumentException("This function should not be called if we don't have a valid layout, right now that means AbsoluteLayout");
        }
    }
}
