using System.Threading.Tasks;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.Utils;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using Serilog;

namespace HodlWallet2.Core.ViewModels
{
    public class BackupViewModel : BaseViewModel
    {
        //TODO: Add localization strings
        public string BackupTitle => "Backup Recovery Key";

        public string HeaderText =>
            "Your backup recovery key is the only way to restore your wallet if your phone is lost, stolen, broken or upgraded.";

        public string SubheaderText =>
            "We will show you a list of words to write down on a piece of paper and keep safe.";
        public string ButtonText => "Write Down Backup Recovery Key";
        public IMvxAsyncCommand<string> WriteDownWordsCommand { get; }
        
        public BackupViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            WriteDownWordsCommand = new MvxAsyncCommand<string>(NavigateToView);
        }

        private async Task NavigateToView(string arg)
        {
            //TODO: WIP - Add logic for BAckupRecoveryWordViewModel.
            await NavigationService.Navigate<BackupRecoveryWordViewModel>();
        }
    }
}