//
// SettingsView.xaml.cs
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

using Xamarin.Essentials;
using Xamarin.Forms;

using HodlWallet.Core.Utils;
using HodlWallet.UI.Locale;
using HodlWallet.UI.Extensions;

namespace HodlWallet.UI.Views
{
    public partial class SettingsView : ContentPage
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        async void Security_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SecuritySettingsView());
        }

        async void Wallet_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WalletSettingsView());
        }

        async void Appearance_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AppearanceSettingsView());
        }

        async void BuildDate_Tapped(object sender, EventArgs e)
        {
            var buildInfo = string.Format(
                Constants.BUILD_INFO_CONTENT,
                BuildInfo.GitBranch,
                BuildInfo.GitHead
            );

            Debug.WriteLine($"[BuildDate_Tapped] {buildInfo}");

            await Clipboard.SetTextAsync(buildInfo);

            var msg = $"{buildInfo}\n\n{Constants.BUILD_INFO_COPIED_TO_CLIPBOARD}";

            await this.DisplayPrompt(
                Constants.BUILD_INFO_MESSAGE_TITLE,
                msg,
                LocaleResources.Error_ok
            );
        }
    }
}
