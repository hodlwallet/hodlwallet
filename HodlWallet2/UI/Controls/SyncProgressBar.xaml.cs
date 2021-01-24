//
// SyncProgressBar.xaml.cs
//
// Copyright (c) 2019 HODL Wallet
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
using System.Collections.Generic;
using Xamarin.Forms;

namespace HodlWallet.UI.Controls
{
    public partial class SyncProgressBar : ContentView
    {
        public SyncProgressBar()
        {
            InitializeComponent();
            IsVisible = false;

            SetBinding(IsVisibleProperty, new Binding(nameof(SyncVisible), source: this));
            TitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(SyncTitle), source: this));
            Sync_ProgressBar.SetBinding(ProgressBar.ProgressProperty, new Binding(nameof(SyncProgress), source: this));
            DateLabel.SetBinding(Label.TextProperty, new Binding(nameof(SyncDate), source: this));
        }

        public static readonly BindableProperty SyncVisibleProperty =
            BindableProperty.Create(
                "SyncVisible",
                typeof(bool),
                typeof(SyncProgressBar)
            );

        public bool SyncVisible
        {
            get => (bool) GetValue (SyncVisibleProperty);
            set
            {
                SetValue(SyncVisibleProperty, value);
                IsVisible = value;
            }
        }

        public static readonly BindableProperty SyncTitleProperty = 
            BindableProperty.Create(
                "SyncTitle",
                typeof(string),
                typeof(SyncProgressBar)
            );

        public string SyncTitle
        {
            get => (string) GetValue (SyncTitleProperty);
            set
            {
                SetValue(SyncTitleProperty, value);
                TitleLabel.Text = value;
            }
        }

        public static readonly BindableProperty SyncProgressProperty =
            BindableProperty.Create(
                "SyncProgress",
                typeof(double),
                typeof(SyncProgressBar)
            );

        public double SyncProgress
        {
            get => (double)GetValue(SyncProgressProperty);
            set
            {
                SetValue(SyncTitleProperty, value);
                Sync_ProgressBar.Progress = value;
            }
        }

        public static readonly BindableProperty SyncDateProperty =
            BindableProperty.Create(
                "SyncDate",
                typeof(string),
                typeof(SyncProgressBar)
            );

        public string SyncDate
        {
            get => (string)GetValue(SyncDateProperty);
            set
            {
                SetValue(SyncDateProperty, value);
                DateLabel.Text = value;
            }
        }
    }
}
