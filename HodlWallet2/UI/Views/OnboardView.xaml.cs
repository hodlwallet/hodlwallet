﻿//
// OnboardView.xaml.cs
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
using System.Diagnostics;

using Xamarin.Forms;

using HodlWallet.UI.Locale;

namespace HodlWallet.UI.Views
{
    public partial class OnboardView : ContentPage
    {
        public OnboardView()
        {
            InitializeComponent();

            SetLabels();
        }

        void SetLabels()
        {
            CreateButton.Text = LocaleResources.Onboard_create;
            RecoverButton.Text = LocaleResources.Onboard_recover;
        }

        void CreateButton_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("[CreateButton_Clicked]");

            Navigation.PushAsync(new PinPadView());
        }

        void RecoverButton_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("[RecoverButton_Clicked]");

            Navigation.PushAsync(new RecoverView());
        }
    }
}
