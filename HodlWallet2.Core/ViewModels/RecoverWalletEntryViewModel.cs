using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    public class RecoverWalletEntryViewModel : BaseViewModel
    {
        public RecoverWalletEntryViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        {
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