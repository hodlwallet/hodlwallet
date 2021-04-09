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

namespace HodlWallet.UI.Controls
{
    public partial class ToastView : Frame
    {
        const int TIME_DELAY_MILLISECONDS = 2500;

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

        bool closed;
        public bool Closed
        {
            get => closed;
            set
            {
                closed = value;

                if (closed) OnClosed.Invoke(this, true);
            }
        }

        public event EventHandler<bool> OnClosed;

        CancellationTokenSource cts = new();

        public ToastView()
        {
            InitializeComponent();
        }

        public void Init()
        {
            IsVisible = true;

            this.FadeTo(0.65);
        }

        public void UpdateContent(string content)
        {
            ToastText = content;

            StartTimerToClose(retry: true);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(ToastText))
            {
                ChangeToastText(ToastText);
            }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent != null) StartTimerToClose();
        }

        void StartTimerToClose(bool retry = false)
        {
            if (retry) cts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                await Task.Delay(TIME_DELAY_MILLISECONDS);

                if (!cts.IsCancellationRequested)
                {
                    cts.Cancel();

                    Close();
                }
            }, cts.Token);
        }

        void ChangeToastText(string toastText)
        {
            Debug.WriteLine($"[ChangeToastText] Changing toast text to: {toastText}");

            ToastContent.Text = toastText;
        }

        void Toast_Tapped(object sender, EventArgs e)
        {
            cts.Cancel();

            Close();
        }

        void Close(uint animationTime = 250)
        {
            this.FadeTo(0.0, animationTime);

            IsVisible = false;
            Closed = true;
        }
    }
}
