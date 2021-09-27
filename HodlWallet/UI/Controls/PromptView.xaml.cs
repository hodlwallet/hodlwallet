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
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Essentials;

using HodlWallet.UI.Locale;

namespace HodlWallet.UI.Controls
{
    public partial class PromptView : ContentView
    {
        Color Fg => (Color)Application.Current.Resources["Fg"];
        double QuestionFrameTopMargin => (DeviceDisplay.MainDisplayInfo.Height / 2) - (QuestionFrame.Height * 2);

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
                    Responded.Invoke(this, true);

                if (promptResponse == PromptResponses.Cancel)
                    Responded.Invoke(this, false);
            }
        }

        public event EventHandler<bool> Responded;

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

        public PromptView(string title, string message = null, string okButton = null, string cancelButton = null)
        {
            InitializeComponent();

            QuestionFrame.Margin = new Thickness(0, QuestionFrameTopMargin, 0, 0);

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

        public void Init()
        {
            Show();
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

        async Task ShowPromptAnimated()
        {
            IsVisible = true;

            var animationTaskSource = new TaskCompletionSource<bool>();

            var animation = new Animation(v => { QuestionFrame.Margin = new Thickness(0, v, 0, 0); }, QuestionFrame.Margin.Top, 0);
            animation.Commit(this, "OpenPromptAnimation", 16, 350, Easing.SinOut, (v, c) => QuestionFrame.Margin = new Thickness(0, 0, 0, 0), () =>
            {
                animationTaskSource.SetResult(false);
                return false;
            });

            await Task.WhenAll(
                animationTaskSource.Task,
                TransparentBackgroundBoxView.FadeTo(0.9)
            );

            MessagingCenter.Send(this, "HideTabbar");
        }

        async Task HidePromptAnimated()
        {
            var animationTaskSource = new TaskCompletionSource<bool>();

            MessagingCenter.Send(this, "ShowTabbar");

            var animation = new Animation(v => { QuestionFrame.Margin = new Thickness(0, v, 0, 0); }, QuestionFrame.Margin.Top, QuestionFrameTopMargin);
            animation.Commit(this, "ClosePromptAnimation", 16, 350, Easing.SinOut, (v, c) => QuestionFrame.Margin = new Thickness(0, QuestionFrameTopMargin, 0, 0), () =>
            {
                animationTaskSource.SetResult(false);
                return false;
            });

            await Task.WhenAll(
                animationTaskSource.Task,
                TransparentBackgroundBoxView.FadeTo(0.0)
            );

            IsVisible = false;

            RemoveYourself();
        }

        void Show()
        {
            _ = ShowPromptAnimated();
        }

        void Hide()
        {
            _ = HidePromptAnimated();
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

        void CancelButton_Clicked(object sender, EventArgs e)
        {
            PromptResponse = PromptResponses.Cancel;

            Hide();
        }

        void OkButton_Clicked(object sender, EventArgs e)
        {
            PromptResponse = PromptResponses.Ok;

            Hide();
        }

        void RemoveYourself()
        {
            ((AbsoluteLayout)Parent).Children.Remove(this);
        }
    }
}
