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
using Rg.Plugins.Popup.Services;

namespace HodlWallet.UI.Controls
{
    public partial class ToastView : PopupPage
    {
        const int TIME_DELAY_MILLISECONDS = 2500;

        public CancellationTokenSource Cts { get; private set; }

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

        public ToastView(string text)
        {
            InitializeComponent();

            Cts ??= new CancellationTokenSource();
            ToastText = text;
        }

        public static async Task Show(string message)
        {
            var view = new ToastView(message);
            await PopupNavigation.Instance.PushAsync(view);

            _ = Task.Delay(TIME_DELAY_MILLISECONDS, view.Cts.Token).ContinueWith(async _ => await PopupNavigation.Instance.PopAsync());
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

        async void Toast_Tapped(object sender, EventArgs e)
        {
            Cts.Cancel();

            await PopupNavigation.Instance.PopAsync();
        }
    }
}
