using System;
using System.Threading.Tasks;
using HodlWallet2.Core.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using ZXing.Mobile;

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
        public MvxAsyncCommand CloseCommand { get; private set; }
        public MvxAsyncCommand ShowFaqCommand { get; private set; }

        public SendViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) : base(logProvider, navigationService)
        {
            _walletService = walletService;
            ScanCommand = new MvxAsyncCommand(Scan);
            SendCommand = new MvxAsyncCommand(Send);
            CloseCommand = new MvxAsyncCommand(Close);
            ShowFaqCommand = new MvxAsyncCommand(ShowFaq);
        }

        private Task ShowFaq()
        {
            //TODO: Implement FAQ
            return Task.FromResult(this);
        }

        private async Task Close()    
        {
            await NavigationService.Close(this);
        }

        private async Task Scan()
        {
            //TODO: Implement Scan
            var scanner = new MobileBarcodeScanner();

            var result = await scanner.Scan();

            AddressToSendTo = result.Text;
        }

        private Task Send()
        {
            //TODO: Implement Send
            return Task.FromResult(this);
        }
    }
}