using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;

using HodlWallet2.Core.Utils;
using HodlWallet2.UI.Locale;
using HodlWallet2.UI.Extensions;
using HodlWallet2.Core.ViewModels;

namespace HodlWallet2.UI.Views
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

        void ResyncWallet_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await AskThisIsIrreversibleQuestion("resync-wallet", (bool answer) =>
                {
                    if (!answer) return;

                    _ViewModel.ResyncWallet();
                });
            });
        }

        void RestoreWallet_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await AskThisIsIrreversibleQuestion("restore-wallet", async (bool answer) =>
                {
                    if (!answer) return;

                    var view = new RecoverView(closeable: true);
                    var nav = new NavigationPage(view);

                    await Navigation.PushModalAsync(nav);
                });
            });
        }

        void WipeWallet_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await AskThisIsIrreversibleQuestion("wipe-wallet", (bool answer) =>
                {
                    if (!answer) return;

                    _ViewModel.WipeWallet();
                    Process.GetCurrentProcess().Kill(); // die
                });
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

        async Task AskThisIsIrreversibleQuestion(string key, Action<bool> action)
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

            await this.DisplayPrompt(
                title,
                Constants.ACTION_IRREVERSIBLE,
                Constants.YES_BUTTON,
                Constants.NO_BUTTON,
                action
            );
        }
    }
}
