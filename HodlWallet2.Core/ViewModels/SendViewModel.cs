using System.Threading.Tasks;
using HodlWallet2.Core.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    public class SendViewModel : BaseViewModel
    {
        private readonly IWalletService _walletService;

        private string _addressToSendTo;
        private int _fee;
        private int _amountToSend;

        public string AddressToSendTo
        {
            get => _addressToSendTo;
            set => SetProperty(ref _addressToSendTo, value);
        }

        public int Fee
        {
            get => _fee;
            set => SetProperty(ref _fee, value);
        }

        public int AmountToSend
        {
            get => _amountToSend;
            set => SetProperty(ref _amountToSend, value);
        }
        
        public MvxAsyncCommand ScanCommand { get; private set; }
        public MvxAsyncCommand SendCommand { get; private set; }

        public SendViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) : base(logProvider, navigationService)
        {
            _walletService = walletService;
            ScanCommand = new MvxAsyncCommand(Scan);
            SendCommand = new MvxAsyncCommand(Send);
        }

        private Task Scan()
        {
            //TODO: Implement Scan
        }

        private Task Send()
        {
            //TODO: Implement Send
        }
    }
}