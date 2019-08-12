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

        public static async Task DisplayPrompt(this ContentPage view, string title, string message = null, string okButton = null, string cancelButton = null, Action<bool> action = null)
        {
            Guard.NotNull(title, nameof(title));
            Guard.NotEmpty(title, nameof(title));

            if (CanAttachToView(view))
            {
                var prompt = CreatePromptFor(view, title, message, okButton, cancelButton);

                if (action is null) return;

                //prompt.Responded += (object s, bool res) =>
                //{
                //    action.Invoke(res);
                //};

                // FIXME This could should work better... like above!
                while (true)
                {
                    if (prompt.PromptResponse == PromptView.PromptResponses.Ok)
                    {
                        action.Invoke(true);
                        return;
                    }
                    if (prompt.PromptResponse == PromptView.PromptResponses.Cancel)
                    {
                        action.Invoke(false);
                        return;
                    }

                    await Task.Delay(250);
                }
            }
            else
            {
                Debug.WriteLine("[DisplayToast] Cannot attach toast to view, your layout must be a AbsoluteLayout");

                bool invoked = false;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await view.DisplayAlert(title, message, okButton, cancelButton);

                    if (action != null) action.Invoke(result);

                    invoked = true;
                });

                // FIXME Loop to make the function wait... again, this shouldn't be like this...
                while(true)
                {
                    if (invoked) return;

                    await Task.Delay(250);
                }
            }
        }

        static bool CanAttachToView(ContentPage view)
        {
            // Supported layouts that we can add a view to.
            if (view.Content.GetType() == typeof(AbsoluteLayout)) return true;

            return false;
        }

        static PromptView CreatePromptFor(ContentPage view, string title, string message = null, string okButton = null, string cancelButton = null)
        {
            if (!(GetContentType(view) == "AbsoluteLayout"))
                throw new ArgumentException("Should not be called without an AbsoluteLayout");

            var prompt = new PromptView(title, message, okButton, cancelButton);
            var layout = (AbsoluteLayout)view.Content;

            Device.BeginInvokeOnMainThread(() =>
            {
                layout.Children.Add(prompt);

                prompt.Init();
            });

            return prompt;
        }

        static void CreateToastFor(ContentPage view, string content)
        {
            if (!(GetContentType(view) == "AbsoluteLayout"))
                throw new ArgumentException("Should not be called without an AbsoluteLayout");

            var toast = new ToastView { ToastText = content };
            var layout = (AbsoluteLayout)view.Content;

            Device.BeginInvokeOnMainThread(() =>
            {
                layout.Children.Add(toast);

                toast.Init();
            });
        }

        static string GetContentType(ContentPage view)
        {
            if (view.Content.GetType() == typeof(AbsoluteLayout)) return "AbsoluteLayout";

            throw new ArgumentException("This function should not be called if we don't have a valid layout, right now that means AbsoluteLayout");
        }
    }
}
