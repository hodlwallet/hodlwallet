using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    public class BackupViewModel : BaseViewModel
    {
        public IMvxAsyncCommand<string> NavigateToViewCommand { get; private set; }
        
        public BackupViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            NavigateToViewCommand = new MvxAsyncCommand<string>(NavigateToView);
        }

        private async Task NavigateToView(string arg)
        {
            await NavigationService.Navigate<BackupRecoveryWordViewModel>();
        }
    }
}