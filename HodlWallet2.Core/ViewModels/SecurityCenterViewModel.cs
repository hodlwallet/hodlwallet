using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    [MvxModalPresentation]
    public class SecurityCenterViewModel : BaseViewModel
    {
        protected SecurityCenterViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
        }
    }
}