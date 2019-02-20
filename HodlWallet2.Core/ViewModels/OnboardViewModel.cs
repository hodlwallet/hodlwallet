using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    public class OnboardViewModel : BaseViewModel
    {        
        public IMvxAsyncCommand<string> NavigateToViewCommand { get; private set; } 
        
        public OnboardViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
            NavigateToViewCommand = new MvxAsyncCommand<string>(NavigateToView);
        }

        private async Task NavigateToView(string option)
        {
            switch (option)
            {
                case "Create":
                    await NavigationService.Navigate<PinPadViewModel>();
                    break;
                case "Recover":
                    await NavigationService.Navigate<RecoverViewModel>();
                    break;
            }
        }
    }
}