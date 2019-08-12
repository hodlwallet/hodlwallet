//
// DemoView.xaml.cs
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class DemoView : ContentPage
    {
        enum PromptAnswer
        {
            Ok,
            Cancel
        };

        PromptAnswer _PromptAnswer;

        public DemoView()
        {
            InitializeComponent();
        }

        void ShowPromptButton_Clicked(object sender, EventArgs e)
        {
            _ = ShowPromptAnimated();
        }

        void OkButton_Clicked(object sender, EventArgs e)
        {
            _PromptAnswer = PromptAnswer.Ok;

            _ = HidePromptAnimated();
        }

        void CancelButton_Clicked(object sender, EventArgs e)
        {
            _PromptAnswer = PromptAnswer.Cancel;

            _ = HidePromptAnimated();
        }

        async Task ShowPromptAnimated()
        {
            PromptControl.IsVisible = true;

            await Task.WhenAll(
                TransparentBackgroundBoxView.FadeTo(0.9, 500),
                QuestionFrame.FadeTo(1.0, 150)
            );
        }

        async Task HidePromptAnimated()
        {
            await Task.Delay(10);

            await Task.WhenAll(
                TransparentBackgroundBoxView.FadeTo(0.0, 150),
                QuestionFrame.FadeTo(0.0, 100)
            );

            PromptControl.IsVisible = false;
        }
    }
}
