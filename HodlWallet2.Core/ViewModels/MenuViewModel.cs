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

namespace HodlWallet2.Core.ViewModels
{
    [MvxModalPresentation]
    public class MenuViewModel : BaseViewModel
    {
        public MvxAsyncCommand CloseCommand { get; }
        public MvxAsyncCommand SecurityCommand { get; }
        public MvxAsyncCommand SettingsCommand { get; }

        public MvxAsyncCommand ResyncWalletCommand { get; }
        public MvxAsyncCommand RestoreWalletCommand { get; }
        public MvxAsyncCommand WipeWalletCommand { get; }

        public MvxAsyncCommand ShowBuildInfoCommand { get; }

        MvxInteraction<YesNoQuestion> _QuestionInteraction = new MvxInteraction<YesNoQuestion>();
        public IMvxInteraction<YesNoQuestion> QuestionInteraction => _QuestionInteraction;

        public MvxInteraction<DisplayAlertContent> DisplayAlertInteraction { get; } = new MvxInteraction<DisplayAlertContent>();

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
            ResyncWalletCommand = new MvxAsyncCommand(ResyncWallet);
            RestoreWalletCommand = new MvxAsyncCommand(RestoreWallet);
            WipeWalletCommand = new MvxAsyncCommand(WipeWallet);
            ShowBuildInfoCommand = new MvxAsyncCommand(ShowBuildInfo);
        }

        async Task ShowBuildInfo()
        {
            string buildInfo = "";

            buildInfo += GitInfo.GetOrigHead();

            _Logger.Debug(buildInfo);

            var request = new DisplayAlertContent
            {
                Title = "Build Info",
                Message = buildInfo,
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
                    if (yes) _WalletService.ReScan();

                    await NavigationService.Close(this);
                }
            };

            _QuestionInteraction.Raise(request);
        }

        async Task RestoreWallet()
        {
            var request = new YesNoQuestion
            {
                QuestionKey = "restore-wallet",
                AnswerCallback = async (yes) =>
                {
                    if (yes) await NavigationService.Navigate<RecoverViewModel>();

                    await NavigationService.Close(this);
                }
            };

            _QuestionInteraction.Raise(request);
        }

        async Task WipeWallet()
        {
            var request = new YesNoQuestion
            {
                QuestionKey = "wipe-wallet",
                AnswerCallback = async (yes) =>
                {
                    if (yes)
                    {
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

                        return; // ... App is dad anyways, so we just do this of courtesy and correctness
                    }

                    await NavigationService.Close(this);
                }
            };

             _QuestionInteraction.Raise(request);
        }
    }
}
