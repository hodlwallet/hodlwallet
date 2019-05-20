using MvvmCross.Commands;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    [MvxModalPresentation]
    public class MenuViewModel : BaseViewModel
    {
        public MvxCommand CloseCommand { get; private set; }
        public MvxCommand SecurityCommand { get; private set; }
        
        public MenuViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
            CloseCommand = new MvxCommand(Close);
            SecurityCommand = new MvxCommand(SecurityTapped);
        }

        private async void SecurityTapped()
        {
            await NavigationService.Navigate<SecurityCenterViewModel>();
        }

        private async void Close()
        {
            await NavigationService.Close(this);
        }
    }
}