using System;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace HodlWallet2.Core.ViewModels
{
    public class SendTabViewModel : BaseViewModel
    {
        public SendTabViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {

        }
    }
}
