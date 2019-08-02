using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;

using HodlWallet2.Core.Utils;
using HodlWallet2.UI.Locale;
using HodlWallet2.Core.ViewModels;

namespace HodlWallet2.UI.Views
{
    public partial class SettingsView : ContentPage
    {
        SettingsViewModel _ViewModel;

        public SettingsView()
        {
            InitializeComponent();

            SetLabels();

            _ViewModel = (SettingsViewModel)BindingContext;
        }

        void BackupMnemonic_Clicked(object sender, EventArgs e)
        {
            var view = new BackupView(closable: true);
            var nav = new NavigationPage(view);

            Navigation.PushModalAsync(nav);
        }

        void ResyncWallet_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool answer = await AskThisIsIrreversibleQuestion("resync-wallet");

                if (!answer) return;

                _ViewModel.ResyncWallet();
            });
        }

        void RestoreWallet_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool answer = await AskThisIsIrreversibleQuestion("restore-wallet");

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
                bool answer = await AskThisIsIrreversibleQuestion("wipe-wallet");

                if (!answer) return;

                _ViewModel.WipeWallet();
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

            DisplayAlert(
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

            return await DisplayAlert(
                title,
                Constants.ACTION_IRREVERSIBLE,
                Constants.YES_BUTTON,
                Constants.NO_BUTTON
            );
        }
    }
}
