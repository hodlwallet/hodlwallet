using System.Diagnostics;
using HodlWallet2.Core.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    public class PinPadViewModel : BaseViewModel
    {
        private IWalletService _walletService;
        
        public IMvxCommand<string> SuccessCommand { get; private set; }
        
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
            _walletService = walletService;
            //TODO: Change Action.
            SuccessCommand = new MvxCommand<string>(pin =>
            {
                Debug.WriteLine($"PIN Saved: {pin}");
                _walletService.InitializeWallet();
            });
        }
    }
}