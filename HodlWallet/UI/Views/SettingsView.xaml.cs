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
        SettingsViewModel _ViewModel => (SettingsViewModel)BindingContext;

        public SettingsView()
        {
            InitializeComponent();

            SetLabels();
        }

        void BackupMnemonic_Clicked(object sender, EventArgs e)
        {
            var view = new BackupView(action: "close");
            var nav = new NavigationPage(view);

            Navigation.PushModalAsync(nav);
        }

        async void ResyncWallet_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var answer = await AskThisIsIrreversibleQuestion("resync-wallet");

                if (!answer) return;
            });

            await _ViewModel.ResyncWallet();
        }

        void RestoreWallet_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var answer = await AskThisIsIrreversibleQuestion("restore-wallet");

                if (!answer) return;

                var view = new RecoverView(closeable: true);
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

                _ViewModel.WipeWallet();
                Process.GetCurrentProcess().Kill(); // die
            });
        }

        void BuildDate_Tapped(object sender, EventArgs e)
        {
            string buildInfo = string.Format(
                Constants.BUILD_INFO_CONTENT,
                BuildInfo.GitBranch,
                BuildInfo.GitHead
            );

            Debug.WriteLine(buildInfo);

            Clipboard.SetTextAsync(buildInfo);

            string msg = $"{buildInfo}\n\n{Constants.BUILD_INFO_COPIED_TO_CLIPBOARD}";

            _ = this.DisplayPrompt(
                Constants.BUILD_INFO_MESSAGE_TITLE,
                msg,
                Constants.DISPLAY_ALERT_ERROR_BUTTON
            );
        }

        void SetLabels()
        {
            ResyncWallet.Text = LocaleResources.Menu_resyncWallet;
            RestoreWallet.Text = LocaleResources.Menu_restoreWallet;
            WipeWallet.Text = LocaleResources.Menu_wipeWallet;
            BackupMnemonic.Text = LocaleResources.Backup_title;

#if DEBUG
            BuildDate.Text = $"Built on: {BuildInfo.BuildDateText}";
#endif
        }

        async Task<bool> AskThisIsIrreversibleQuestion(string key)
        {
            string title;
            switch (key)
            {
                case "wipe-wallet":
                    title = LocaleResources.Menu_wipeWallet;
                    break;
                case "resync-wallet":
                    title = LocaleResources.Menu_resyncWallet;
                    break;
                case "restore-wallet":
                    title = LocaleResources.Menu_restoreWallet;
                    break;
                default:
                    throw new ArgumentException($"Invalid question sent, key: {key}");
            }

            return await this.DisplayPrompt(
                title,
                Constants.ACTION_IRREVERSIBLE,
                Constants.YES_BUTTON,
                Constants.NO_BUTTON
            );
        }
    }
}
