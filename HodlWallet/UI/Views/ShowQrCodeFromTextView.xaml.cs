//
// ShowQrCodeFromTextView.xaml.cs
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
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Essentials;

using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Extensions;
using HodlWallet.UI.Locale;

namespace HodlWallet.UI.Views
{
    public partial class ShowQrCodeFromTextView : ContentPage
    {
        ShowQrCodeFromTextViewModel ViewModel => BindingContext as ShowQrCodeFromTextViewModel;

        public ShowQrCodeFromTextView(string text)
        {
            InitializeComponent();

            ViewModel.Text = text;
        }

        async void QrCode_Tapped(object sender, EventArgs e)
        {
            await CopyText();
        }

        async void Text_Tapped(object sender, EventArgs e)
        {
            await CopyText();
        }

        async Task CopyText()
        {
            await Clipboard.SetTextAsync(ViewModel.Text);
            await this.DisplayToast(LocaleResources.SecretContentView_textCopied);
        }

        async void Close_Tapped(object sender, EventArgs e)
        {
            await Close();
        }

        async void Close_Swiped(object sender, SwipedEventArgs e)
        {
            await Close();
        }

        async Task Close()
        {
            await Navigation.PopModalAsync();
        }
    }
}
