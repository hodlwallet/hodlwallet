using System.Threading.Tasks;

using MvvmCross.Commands;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using Serilog;

using HodlWallet2.Core.Services;
using HodlWallet2.Core.Interactions;

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

        MvxInteraction<YesNoQuestion> _QuestionInteraction = new MvxInteraction<YesNoQuestion>();
        public IMvxInteraction<YesNoQuestion> QuestionInteraction => _QuestionInteraction;

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
                    // TODO
                    //if (yes) _WalletService.DestroyWallet(true);

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
                    // TODO
                    //if (yes) _WalletService.DestroyWallet(true);

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
                    if (yes) _WalletService.DestroyWallet(true);

                    await NavigationService.Close(this);
                }
            };

             _QuestionInteraction.Raise(request);
        }
    }
}
