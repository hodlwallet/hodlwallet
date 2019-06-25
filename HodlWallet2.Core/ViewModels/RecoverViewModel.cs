using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    public class RecoverViewModel : BaseViewModel
    {
        public IMvxAsyncCommand RecoverNextCommand { get; }
        
        public RecoverViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
            RecoverNextCommand = new MvxAsyncCommand(RecoverNext);
        }
        
        private async Task RecoverNext()
        {
            await NavigationService.Navigate<RecoverWalletEntryViewModel>();
        }
    }
}