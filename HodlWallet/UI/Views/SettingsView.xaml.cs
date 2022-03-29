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
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;

using HodlWallet.Core.Utils;
using HodlWallet.UI.Locale;
using HodlWallet.UI.Extensions;
using HodlWallet.Core.ViewModels;

namespace HodlWallet.UI.Views
{
    public partial class SettingsView : ContentPage
    {
        SettingsViewModel ViewModel => BindingContext as SettingsViewModel;

        public SettingsView()
        {
            InitializeComponent();

            SetBuiltOnDate();
        }

        void BackupMnemonic_Clicked(object sender, EventArgs e)
        {
            var view = new BackupView(action: "close");
            var nav = new NavigationPage(view);

            Navigation.PushModalAsync(nav);
        }

        void RestoreWallet_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var answer = await AskThisIsIrreversibleQuestion("restore-wallet");

                if (!answer) return;

                var view = new RecoverInfoView(closeable: true);
                var nav = new NavigationPage(view);

                await Navigation.PushModalAsync(nav);
            });
        }

        void WipeWallet_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var answer = await AskThisIsIrreversibleQuestion("wipe-wallet");

                if (!answer) return;

                ViewModel.WipeWallet();
                Process.GetCurrentProcess().Kill(); // die
            });
        }

        async void BuildDate_Tapped(object sender, EventArgs e)
        {
            string buildInfo = string.Format(
                Constants.BUILD_INFO_CONTENT,
                BuildInfo.GitBranch,
                BuildInfo.GitHead
            );

            Debug.WriteLine(buildInfo);

            await Clipboard.SetTextAsync(buildInfo);

            string msg = $"{buildInfo}\n\n{Constants.BUILD_INFO_COPIED_TO_CLIPBOARD}";

            await this.DisplayPrompt(
                Constants.BUILD_INFO_MESSAGE_TITLE,
                msg,
                LocaleResources.Error_ok
            );
        }

        void SetBuiltOnDate()
        {
#if DEBUG || TESTNET
            BuildDate.Text = $"Built on: {BuildInfo.BuildDateText}";
#endif
        }

        async Task<bool> AskThisIsIrreversibleQuestion(string key)
        {
            var title = key switch
            {
                "wipe-wallet" => LocaleResources.Menu_wipeWallet,
                "resync-wallet" => LocaleResources.Menu_resyncWallet,
                "restore-wallet" => LocaleResources.Menu_restoreWallet,
                _ => throw new ArgumentException($"Invalid question sent, key: {key}"),
            };

            return await this.DisplayPrompt(
                title,
                LocaleResources.Alert_irreversible,
                LocaleResources.Button_yes,
                LocaleResources.Button_no
            );
        }

        async void Security_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SecuritySettingsView());
        }

        async void AccountSettings_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccountSettingsView());
        }

        async void Currency_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DisplayCurrencyView());
        }
    }
}
