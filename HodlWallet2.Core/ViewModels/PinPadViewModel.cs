using System.Threading.Tasks;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;

namespace HodlWallet2.Core.ViewModels
{
    public class PinPadViewModel : BaseViewModel
    {
        IWalletService _WalletService;
        public IMvxAsyncCommand<string> SuccessCommand { get; }
        
        //TODO: Localize properties
        public string PinPadTitle => "Enter PIN";
        public string PinPadHeader => "Your PIN will be used to unlock your wallet and send money.";
        public string PinPadWarning => "Remember this PIN. If you forget it, you won't be able to access your bitcoin.";

        public PinPadViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
            _WalletService = WalletService.Instance;
            SuccessCommand = new MvxAsyncCommand<string>(Success_Callback);
        }

        private async Task Success_Callback(string pin)
        {
            SavePin(pin);

            _WalletService.InitializeWallet();

            await NavigationService.Navigate<BackupViewModel>();
        }

        private void SavePin(string pin)
        {
            SecureStorageProvider.SetPin(pin);
        }
    }
}