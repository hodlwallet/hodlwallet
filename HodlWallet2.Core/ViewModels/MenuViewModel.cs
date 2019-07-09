using System;
using System.Diagnostics;
using System.Threading.Tasks;

using MvvmCross.Commands;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using Serilog;

using HodlWallet2.Core.Interactions;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.Utils;
using Xamarin.Essentials;

namespace HodlWallet2.Core.ViewModels
{
    [MvxModalPresentation]
    public class MenuViewModel : BaseViewModel
    {
        public MvxAsyncCommand CloseCommand { get; }
        public MvxAsyncCommand SecurityCommand { get; }
        public MvxAsyncCommand SettingsCommand { get; }

        public MvxAsyncCommand BackupMnemonicCommand { get; }
        public MvxAsyncCommand ResyncWalletCommand { get; }
        public MvxAsyncCommand RestoreWalletCommand { get; }
        public MvxAsyncCommand WipeWalletCommand { get; }

        public MvxAsyncCommand ShowBuildInfoCommand { get; }

        public MvxInteraction<YesNoQuestion> QuestionInteraction => new MvxInteraction<YesNoQuestion>();
        public MvxInteraction<DisplayAlertContent> DisplayAlertInteraction => new MvxInteraction<DisplayAlertContent>();

        readonly WalletService _WalletService;
        readonly ILogger _Logger;

        public MenuViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
            _WalletService = WalletService.Instance;
            _Logger = _WalletService.Logger;

            CloseCommand = new MvxAsyncCommand(Close);
            SecurityCommand = new MvxAsyncCommand(SecurityTapped);
            SettingsCommand = new MvxAsyncCommand(SettingsTapped);

            BackupMnemonicCommand = new MvxAsyncCommand(BackupMnemonic);
            ResyncWalletCommand = new MvxAsyncCommand(ResyncWallet);
            RestoreWalletCommand = new MvxAsyncCommand(RestoreWallet);
            WipeWalletCommand = new MvxAsyncCommand(WipeWallet);
            ShowBuildInfoCommand = new MvxAsyncCommand(ShowBuildInfo);
        }

        async Task BackupMnemonic()
        {
            await NavigationService.Navigate<BackupViewModel>();
        }

        async Task ShowBuildInfo()
        {
            string buildInfo = string.Format(
                Constants.BUILD_INFO_CONTENT,
                GitInfo.Branch,
                GitInfo.Head
            );

            _Logger.Debug(buildInfo);

            await Clipboard.SetTextAsync(buildInfo);

            string msg = $"{buildInfo}\n\n{Constants.BUILD_INFO_COPIED_TO_CLIPBOARD}";
            var request = new DisplayAlertContent
            {
                Title = Constants.BUILD_INFO_MESSAGE_TITLE,
                Message = msg,
                Buttons = new string[] { Constants.DISPLAY_ALERT_ERROR_BUTTON }
            };
            DisplayAlertInteraction.Raise(request);
        }

        async Task SecurityTapped()
        {
            await NavigationService.Navigate<SecurityCenterViewModel>();
        }

        async Task SettingsTapped()
        {
            await NavigationService.Navigate<SettingsViewModel>();
        }

        async Task Close()
        {
            await NavigationService.Close(this);
        }

        async Task ResyncWallet()
        {
            var request = new YesNoQuestion
            {
                QuestionKey = "resync-wallet",
                AnswerCallback = async (yes) =>
                {
                    if (!yes) return;

                    _WalletService.ReScan();
                    await NavigationService.Close(this);
                }
            };

            QuestionInteraction.Raise(request);
        }

        async Task RestoreWallet()
        {
            var request = new YesNoQuestion
            {
                QuestionKey = "restore-wallet",
                AnswerCallback = async (yes) =>
                {
                    if (!yes) return;

                    await NavigationService.Navigate<RecoverViewModel>();
                    await NavigationService.Close(this);
                }
            };

            QuestionInteraction.Raise(request);
        }

        async Task WipeWallet()
        {
            var request = new YesNoQuestion
            {
                QuestionKey = "wipe-wallet",
                AnswerCallback = async (yes) =>
                {
                    if (!yes) return;

                    try
                    {
                        _WalletService.DestroyWallet(dryRun: false);
                    }
                    catch (Exception e)
                    {
                        // This exception should never happen, in case it does though here it is
                        _Logger.Information("Error trying to destroy wallet: {0}", e.ToString());
                    }

                    // After destroy we kill
                    Process.GetCurrentProcess().Kill();
                }
            };

             QuestionInteraction.Raise(request);
        }
    }
}
