//
// WalletSettingsView.xaml.cs
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
using ReactiveUI;
using System.Reactive.Concurrency;

namespace HodlWallet.UI.Views
{
    public partial class WalletSettingsView : ContentPage
    {
        WalletSettingsViewModel ViewModel => BindingContext as WalletSettingsViewModel;

        public WalletSettingsView()
        {
            InitializeComponent();
        }

        void Resync_Tapped(object sender, EventArgs e)
        {
            RxApp.MainThreadScheduler.Schedule(async () =>
            {
                var answer = await AskThisIsIrreversibleQuestion("resync");

                if (!answer) return;

                await ViewModel.ResyncWallet();
            });
        }

        void Restore_Tapped(object sender, EventArgs e)
        {
            RxApp.MainThreadScheduler.Schedule(async () =>
            {
                var answer = await AskThisIsIrreversibleQuestion("restore");

                if (!answer) return;

                var view = new RecoverInfoView(closeable: true);
                var nav = new NavigationPage(view);

                await Navigation.PushModalAsync(nav);
            });
        }

        void Wipe_Tapped(object sender, EventArgs e)
        {
            RxApp.MainThreadScheduler.Schedule(async () =>
            {
                var answer = await AskThisIsIrreversibleQuestion("wipe");

                if (!answer) return;

                ViewModel.WipeWallet();
                Process.GetCurrentProcess().Kill(); // die
            });
        }

        async Task<bool> AskThisIsIrreversibleQuestion(string key)
        {
            var title = key switch
            {
                "resync" => LocaleResources.Menu_resyncWallet,
                "restore" => LocaleResources.Menu_restoreWallet,
                "wipe" => LocaleResources.Menu_wipeWallet,
                _ => throw new ArgumentException($"Invalid question sent, key: {key}"),
            };

            return await this.DisplayPrompt(
                title,
                LocaleResources.Alert_irreversible,
                LocaleResources.Button_yes,
                LocaleResources.Button_no
            );
        }
    }
}
