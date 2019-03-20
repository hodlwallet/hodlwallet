using System.Diagnostics;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    public class PinPadViewModel : BaseViewModel
    {
        public IMvxCommand<string> SuccessCommand { get; private set; }
        
        //TODO: Localize properties
        public string PinPadTitle => "Enter PIN";

        public string PinPadHeader => "Your PIN will be used to unlock your wallet and send money.";

        public string PinPadWarning => "Remember this PIN. If you forget it, you won't be able to access your bitcoin.";

        public PinPadViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
            //TODO: Change Action.
            SuccessCommand = new MvxCommand<string>(pin =>
            {
                Debug.WriteLine($"PIN Saved: {pin}");
                
            });
        }
    }
}