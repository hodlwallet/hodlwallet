//
// SecretContentView.xaml.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2021 HODL Wallet
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

using Xamarin.Essentials;
using Xamarin.Forms;

using HodlWallet.UI.Extensions;
using HodlWallet.UI.Locale;

namespace HodlWallet.UI.Controls
{
    public partial class SecretContentView : ContentView
    {
        public SecretContentView()
        {
            InitializeComponent();
            HiddenLayout.SizeChanged += HiddenLayout_SizeChanged;
        }

        void HiddenLayout_SizeChanged(object sender, EventArgs e)
        {
            CensoredContentBoxView.HeightRequest = HiddenLayout.Height;
            HiddenLayout.IsVisible = false;
        }

        public View Secret
        {
            get => (View)GetValue(SecretProperty);
            set
            {
                SetValue(SecretProperty, value);
            }
        }

        public static readonly BindableProperty SecretProperty = BindableProperty.Create(
            nameof(Secret),
            typeof(View),
            typeof(SecretContentView),
            default(View),
            propertyChanged: OnSecretChanged
        );

        public static void OnSecretChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var secretContentView = bindable as SecretContentView;

            secretContentView.HiddenLayout.Children.Clear();
            secretContentView.HiddenLayout.Children.Add(newValue as View);
        }

        public bool IsHidden
        {
            get => (bool)GetValue(IsHiddenProperty);
            set
            {
                SetValue(IsHiddenProperty, value);
            }
        }

        public static readonly BindableProperty IsHiddenProperty = BindableProperty.Create(
            nameof(IsHidden),
            typeof(bool),
            typeof(SecretContentView),
            true,
            propertyChanged: OnIsHiddenChanged
        );

        public static void OnIsHiddenChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var secretContentView = bindable as SecretContentView;
            var value = (bool)newValue;

            secretContentView.ToggleHideButton.Source = value ? "hidden" : "visible";
            secretContentView.HiddenLayout.IsVisible = !value;
            secretContentView.QrCodeButton.IsVisible = !value;
            secretContentView.HiddenLayout.Opacity = value ? 0.00 : 1.00;
            secretContentView.CensoredContentBoxView.IsVisible = value;
        }

        void ToggleHideButton_Clicked(object sender, EventArgs e)
        {
            IsHidden = !IsHidden;
        }

        void QrCodeButton_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Not implemented yet");
        }

        async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (IsHidden) return;
            if (HiddenLayout.Children.Count == 0) return;

            var onlyChild = HiddenLayout.Children[0];
            if (onlyChild is Label) await Clipboard.SetTextAsync((onlyChild as Label).Text);
            if (onlyChild is Entry) await Clipboard.SetTextAsync((onlyChild as Entry).Text);
            if (onlyChild is Editor) await Clipboard.SetTextAsync((onlyChild as Editor).Text);

            await (Navigation.NavigationStack[1] as ContentPage).DisplayToast(LocaleResources.SecretContentView_textCopied);
        }
    }
}
