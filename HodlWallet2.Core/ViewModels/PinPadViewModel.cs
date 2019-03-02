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
        public string PinPadTitle => "This Title is coming from the ViewModel";

        public string PinPadHeader => "This Header is coming from the ViewModel";

        public string PinPadWarning => "This Warning is coming from the ViewModel";

        public PinPadViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
            //TODO: Change Action.
            SuccessCommand = new MvxCommand<string>(pin => Debug.WriteLine($"PIN Saved: {pin}"));
        }
    }
}