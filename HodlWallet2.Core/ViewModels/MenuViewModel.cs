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
        
        public MenuViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
            CloseCommand = new MvxCommand(Close);
            SecurityCommand = new MvxCommand(SecurityTapped);
            SettingsCommand = new MvxCommand(SettingsTapped);
        }

        private async void SecurityTapped()
        {
            await NavigationService.Navigate<SecurityCenterViewModel>();
        }

        private async void SettingsTapped()
        {
            await NavigationService.Navigate<SettingsViewModel>();
        }

        private async void Close()
        {
            await NavigationService.Close(this);
        }
    }
}