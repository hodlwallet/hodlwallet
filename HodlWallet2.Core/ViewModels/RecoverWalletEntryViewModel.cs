using System.Threading.Tasks;
using HodlWallet2.Core.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    public class RecoverWalletEntryViewModel : BaseViewModel
    {
        private IWalletService _walletService;
        
        public RecoverWalletEntryViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService)
            : base(logProvider, navigationService)
        {
            _walletService = walletService;
        }

        public string RecoverTitle
        {
            //TODO: Localize string
            get => "Enter the backup recovery key for the wallet you want to recover.";
        }

        public string RecoverHeader
        {
            //TODO: Localize string
            get => "Enter Backup Recovery Key";
        }
        
    }
}