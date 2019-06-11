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
        private double _SliderValue;

        enum FeeType { slow, normal, fastest };
        FeeType _CurrentFee;

        public double SliderValue
        {
            get => _SliderValue;
            set => SetProperty(ref _SliderValue, value);
        }

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
        public MvxCommand OnValueChangedCommand { get; private set; }

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
            OnValueChangedCommand = new MvxCommand(SetSliderValue);
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

        void SetSliderValue()
        {
            if (SliderValue <= .25)
            {
                SliderValue = 0;
            }
            else if (SliderValue > .25 
                        && SliderValue < .75)
            {
                SliderValue = .5;
            }
            else
            {
                SliderValue = 1;
            }
        }

        private async Task Send()
        {
            string password = "123456";
            var txCreateResult = _walletService.CreateTransaction(AmountToSend, AddressToSendTo, Fee, password);

            if (txCreateResult.Success)
            {
                await _walletService.BroadcastManager.BroadcastTransactionAsync(txCreateResult.Tx);
            }
            else
            {
                // TODO show error screen for now just log it.
                LogProvider.GetLogFor<SendViewModel>().Error(
                    "Error trying to create a transaction.\nAmount to send: {amount}, address: {address}, fee: {fee}, password: {password}.\nFull Error: {error}",
                    AmountToSend,
                    AddressToSendTo,
                    Fee,
                    password,
                    txCreateResult.Error
                );
            }
        }
    }
}