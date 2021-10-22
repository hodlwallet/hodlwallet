//
// ToastView.xaml.cs
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
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Exceptions;
using Rg.Plugins.Popup.Services;

namespace HodlWallet.UI.Controls
{
    public partial class ToastView : PopupPage
    {
        const int TIME_DELAY_MILLISECONDS = 2500;

        public CancellationTokenSource Cts { get; private set; }

        public ContentPage View { get; private set; }

        public static readonly BindableProperty ToastTextProperty = BindableProperty.CreateAttached(
                nameof(ToastText),
                typeof(string),
                typeof(ToastView),
                default(string)
        );

        public string ToastText
        {
            get => (string)GetValue(ToastTextProperty);
            set => SetValue(ToastTextProperty, value);
        }

        public ToastView(ContentPage view, string text)
        {
            InitializeComponent();

            Cts ??= new CancellationTokenSource();
            View ??= view;

            ToastText = text;
        }

        public static async Task Show(ContentPage view, string message)
        {
            var toast = new ToastView(view, message);

            await view.Navigation.PushPopupAsync(toast);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TIME_DELAY_MILLISECONDS, Cts.Token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                if (Cts.IsCancellationRequested) return;

                await TryRemovePopup();
            });
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(ToastText))
            {
                ChangeToastText(ToastText);
            }
        }

        void ChangeToastText(string toastText)
        {
            Debug.WriteLine($"[ChangeToastText] Changing toast text to: {toastText}");

            ToastContent.Text = toastText;
        }

        async Task TryRemovePopup()
        {
            try
            {
                await View.Navigation.PopPopupAsync();
            }
            catch (RGPopupStackInvalidException e)
            {
                Debug.WriteLine($"[OnAppearing] Failed to remove the popup, I hope is not there anymore: {e.Message}");

                try
                {
                    await PopupNavigation.Instance.PopAllAsync();
                }
                catch (RGPopupStackInvalidException e2)
                {
                    Debug.WriteLine($"[OnAppearing] Extra error: {e2.Message}");
                }
            }
        }

        async void Toast_Tapped(object sender, EventArgs e)
        {
            await TryRemovePopup();
        }
    }
}
