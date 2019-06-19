using System.Diagnostics;
using System.Threading.Tasks;
using HodlWallet2.Core.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

using HodlWallet2.Core.Utils;

namespace HodlWallet2.Core.ViewModels
{
    public class PinPadViewModel : BaseViewModel
    {
        IWalletService _WalletService;
        ILogger _Logger;
        public IMvxAsyncCommand<string> SuccessCommand { get; }
        
        //TODO: Localize properties
        public string PinPadTitle => "Enter PIN";
        public string PinPadHeader => "Your PIN will be used to unlock your wallet and send money.";
        public string PinPadWarning => "Remember this PIN. If you forget it, you won't be able to access your bitcoin.";

        public PinPadViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) 
            : base(logProvider, navigationService)
        {
            _WalletService = walletService;
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