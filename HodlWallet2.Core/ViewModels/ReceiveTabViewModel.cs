using System.Threading.Tasks;

using Xamarin.Essentials;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using HodlWallet2.Core.Interactions;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Utils;
using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class ReceiveTabViewModel : BaseViewModel
    {
        readonly IWalletService _WalletService;
        string _Address;
        readonly Serilog.ILogger _Logger;

        object _Lock = new object();

        public string ShareButtonText => "Share";
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

        public ReceiveTabViewModel(
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
            if (string.IsNullOrEmpty(Address)) return;

            var sharer = Xamarin.Forms.DependencyService.Get<IShareIntent>();
            sharer.QRTextShareIntent(Address);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            if (!_WalletService.IsStarted)
            {
                _WalletService.OnStarted += _WalletService_OnStarted;

                return;
            }

            Address = _WalletService.GetReceiveAddress().Address;
            _Logger.Debug("New Receive Address: {0}", Address);            
        }

        private void _WalletService_OnStarted(object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (_Lock)
                {
                    Address = _WalletService.GetReceiveAddress().Address;
                    _Logger.Debug("New Receive Address: {0}", Address);
                }
            });

            _WalletService.OnStarted -= _WalletService_OnStarted;
        }
    }
}