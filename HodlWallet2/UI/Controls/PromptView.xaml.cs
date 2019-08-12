//
// DialogView.xaml.cs
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
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace HodlWallet2.UI.Controls
{
    public partial class PromptView : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.CreateAttached(
            nameof(Title),
            typeof(string),
            typeof(PromptView),
            default(string)
        );

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty MessageProperty = BindableProperty.CreateAttached(
            nameof(Message),
            typeof(string),
            typeof(PromptView),
            default(string)
        );

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly BindableProperty CancelTextProperty = BindableProperty.CreateAttached(
            nameof(CancelText),
            typeof(string),
            typeof(PromptView),
            default(string)
        );

        public string CancelText
        {
            get => (string)GetValue(CancelTextProperty);
            set => SetValue(CancelTextProperty, value);
        }

        public static readonly BindableProperty OkTextProperty = BindableProperty.CreateAttached(
            nameof(OkText),
            typeof(string),
            typeof(PromptView),
            default(string)
        );

        public string OkText
        {
            get => (string)GetValue(OkTextProperty);
            set => SetValue(OkTextProperty, value);
        }

        public PromptView()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Title))
            {
                ChangeTitleTo(Title);
            }

            if (propertyName == nameof(Message))
            {
                ChangeMessageTo(Message);
            }

            if (propertyName == nameof(CancelText))
            {
                ChangeCancelTextTo(CancelText);
            }

            if (propertyName == nameof(OkText))
            {
                ChangeOkTextTo(OkText);
            }
        }

        void ChangeTitleTo(string title)
        {
            TitleLabel.Text = title;
        }

        void ChangeMessageTo(string message)
        {
            MessageLabel.Text = message;
        }

        void ChangeCancelTextTo(string cancelText)
        {
            CancelButton.Text = cancelText;
        }

        void ChangeOkTextTo(string okText)
        {
            OkButton.Text = okText;
        }
    }
}
