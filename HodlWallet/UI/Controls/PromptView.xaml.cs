//
// PromptView.xaml.cs
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
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using Rg.Plugins.Popup.Pages;

using HodlWallet.UI.Locale;
using Rg.Plugins.Popup.Services;

namespace HodlWallet.UI.Controls
{
    public partial class PromptView : PopupPage
    {
        Color Fg => (Color)Application.Current.Resources["Fg"];

        public enum PromptResponses
        {
            NoReponse,
            Ok,
            Cancel
        }

        PromptResponses promptResponse = PromptResponses.NoReponse;
        public PromptResponses PromptResponse
        {
            get => promptResponse;
            set
            {
                promptResponse = value;

                if (promptResponse == PromptResponses.Ok)
                    Responded?.Invoke(this, true);

                if (promptResponse == PromptResponses.Cancel)
                    Responded?.Invoke(this, false);
            }
        }

        public event EventHandler<bool> Responded;

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

        public PromptView(string title, string message = null, string okButton = null, string cancelButton = null)
        {
            InitializeComponent();

            Title = title;
            Message = message ?? string.Empty;

            if (string.IsNullOrEmpty(okButton)) OkText = LocaleResources.Error_ok;
            else OkText = okButton;

            if (string.IsNullOrEmpty(cancelButton))
            {
                OkButton.TextColor = Fg;
                CancelButton.IsVisible = false;
            }
            else
            {
                CancelText = cancelButton;
                CancelButton.IsVisible = true;
            }
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

        async void CancelButton_Clicked(object sender, EventArgs e)
        {
            PromptResponse = PromptResponses.Cancel;

            await PopupNavigation.Instance.PopAsync();
        }

        async void OkButton_Clicked(object sender, EventArgs e)
        {
            PromptResponse = PromptResponses.Ok;

            await PopupNavigation.Instance.PopAsync();
        }
    }
}
