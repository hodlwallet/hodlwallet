using HodlWallet2.Core.Interfaces;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    public class SetPinViewModel : BaseViewModel
    {
        public SetPinViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
        }
        
        
    }
}