using System.Threading.Tasks;
using HodlWallet2.Core.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using Xamarin.Essentials;

namespace HodlWallet2.Core.ViewModels
{
    public class ReceiveViewModel : BaseViewModel
    {
        private readonly IWalletService _walletService;
        private string _address;

        public IMvxCommand ShowFaqCommand { get; private set; }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public ReceiveViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) : base(logProvider, navigationService)
        {
            _walletService = walletService;
            ShowFaqCommand = new MvxCommand(ShowFaq);
        }

        private void ShowFaq()
        {
            //TODO: Implement FAQ
            throw new System.NotImplementedException();
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            Address = _walletService.GetReceiveAddress().Address;
            LogProvider.GetLogFor<ReceiveViewModel>().Info($"New Receive Address: {Address}");
        }

        public Task ToClipboard()
        {
            return Clipboard.SetTextAsync(Address);
        }
    }
}