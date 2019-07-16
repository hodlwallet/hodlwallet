using System.Threading.Tasks;
using System.Threading.Tasks;

using HodlWallet2.Core.Interfaces;
using Xamarin.Essentials;

using MvvmCross.Commands;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Essentials;

using HodlWallet2.Core.Interactions;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Utils;

namespace HodlWallet2.Core.ViewModels
{
    public class ReceiveViewModel : BaseViewModel
    {
        readonly IWalletService _WalletService;
        string _Address;
        readonly Serilog.ILogger _Logger;

        public string ShareButtonText => "Share";
        public string ReceiveTitle => "Receive";
        public string RequestAmountButtonText => "Request Amount";

        public IMvxCommand ShowFaqCommand { get; }
        public IMvxCommand ShareButtonCommand { get; }
        public IMvxAsyncCommand CopyAddressCommand { get; }

        MvxInteraction<DisplayAlertContent> _CopiedToClipboardInteraction = new MvxInteraction<DisplayAlertContent>();
        public IMvxInteraction<DisplayAlertContent> CopiedToClipboardInteraction => _CopiedToClipboardInteraction;

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
            _Logger = walletService.Logger;
            _WalletService = walletService;

            ShowFaqCommand = new MvxCommand(ShowFaq);
            CopyAddressCommand = new MvxAsyncCommand(ToClipboard);
            ShareButtonCommand = new MvxCommand(ShowShareIntent);
        }

        void ShowFaq()
        {
            //TODO: Implement FAQ
            throw new System.NotImplementedException();
        }

        async Task ToClipboard()
        {
            await Clipboard.SetTextAsync(Address);

            var request = new DisplayAlertContent
            {
                Title = Constants.RECEIVE_ADDRESS_COPIED_TO_CLIPBOARD_TITLE,
                Message = "",
                Buttons = new string[] { Constants.RECEIVE_ADDRESS_COPIED_TO_CLIPBOARD_BUTTON }
            };

            _CopiedToClipboardInteraction.Raise(request);
        }

        void ShowShareIntent()
        {
            var sharer = Xamarin.Forms.DependencyService.Get<IShareIntent>();
            sharer.QRTextShareIntent(Address);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            Address = _WalletService.GetReceiveAddress().Address;

            _Logger.Debug("New Receive Address: {0}", Address);
        }
    }
}