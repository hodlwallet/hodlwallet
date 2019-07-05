using System.Threading.Tasks;
using HodlWallet2.Core.Services;
using MvvmCross.Commands;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    [MvxModalPresentation]
    public class MenuViewModel : BaseViewModel
    {
        public MvxCommand CloseCommand { get; }
        public MvxCommand SecurityCommand { get; }
        public MvxCommand SettingsCommand { get; }

        public MvxCommand ResyncWalletCommand { get; }
        public MvxCommand RestoreWalletCommand { get; }
        public MvxCommand WipeWalletCommand { get; }

        Serilog.ILogger _Logger;

        public MenuViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
            _Logger = WalletService.Instance.Logger;

            CloseCommand = new MvxCommand(Close);
            SecurityCommand = new MvxCommand(SecurityTapped);
            SettingsCommand = new MvxCommand(SettingsTapped);
            ResyncWalletCommand = new MvxCommand(ResyncWallet);
            RestoreWalletCommand = new MvxCommand(RestoreWallet);
            WipeWalletCommand = new MvxCommand(WipeWallet);
        }

        // FIXME These methods should not be async...
        async void SecurityTapped()
        {
            await NavigationService.Navigate<SecurityCenterViewModel>();
        }

        async void SettingsTapped()
        {
            await NavigationService.Navigate<SettingsViewModel>();
        }

        async void Close()
        {
            await NavigationService.Close(this);
        }

        void ResyncWallet()
        {
            _Logger.Information("Doing Resync...");
        }

        void RestoreWallet()
        {
            _Logger.Information("Doing Restore...");
        }

        void WipeWallet()
        {
            _Logger.Information("Doing Wipe...");
        }
    }
}