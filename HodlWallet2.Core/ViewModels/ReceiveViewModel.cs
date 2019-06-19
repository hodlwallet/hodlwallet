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
        readonly IWalletService _WalletService;
        string _Address;

        public IMvxCommand ShowFaqCommand { get; }
        public IMvxAsyncCommand CopyAddressCommand { get; }

        public string Address
        {
            get => _Address;
            set => SetProperty(ref _Address, value);
        }

        public ReceiveViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) : base(logProvider, navigationService)
        {
            _WalletService = walletService;
            ShowFaqCommand = new MvxCommand(ShowFaq);
            CopyAddressCommand = new MvxAsyncCommand(ToClipboard);
        }

        private void ShowFaq()
        {
            //TODO: Implement FAQ
            throw new System.NotImplementedException();
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            Address = _WalletService.GetReceiveAddress().Address;
            LogProvider.GetLogFor<ReceiveViewModel>().Info($"New Receive Address: {Address}");
        }

        public Task ToClipboard()
        {
            return Clipboard.SetTextAsync(Address);
        }
    }
}