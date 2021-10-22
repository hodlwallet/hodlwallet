//
// PromptV2DemoView.xaml.cs
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
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

using HodlWallet.UI.Controls;
using System.Diagnostics;

namespace HodlWallet.UI.Views.Demos
{
    public partial class PromptV2DemoView : ContentPage
    {
        public PromptV2DemoView()
        {
            InitializeComponent();
        }

        void PromptButton_Clicked(object sender, EventArgs e)
        {
            var promptView = new PromptView("This title", "this is the message", "okay", "cancel");
            promptView.Responded += PromptView_Responded;
            Navigation.PushPopupAsync(promptView);
        }

        void ToastButton_Clicked(object sender, EventArgs e)
        {
        }

        void PromptView_Responded(object sender, bool e)
        {
            Debug.WriteLine($"Responded with: {e}");
        }
    }
}
