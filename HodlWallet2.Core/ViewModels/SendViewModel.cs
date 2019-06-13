using System;
using System.Threading.Tasks;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.Utils;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using ZXing.Mobile;

namespace HodlWallet2.Core.ViewModels
{
    public class SendViewModel : BaseViewModel
    {
        readonly IWalletService _WalletService;
        readonly IPrecioService _PrecioService;

        private string _AddressToSendTo;
        private int _Fee;
        private int _AmountToSend;

        double _SliderValue;
        const double _Maximum = 500;

        string _TransactionFeeText;
        string _EstConfirmationText;

        public string TransactionFeeTitle => "Transaction Fee";
        public string EstConfirmationTitle => "Est. Confirmation";
        public string SlowText => "Economy";
        public string NormalText => "Normal";
        public string FastestText => "High";

        public string TransactionFeeText
        {
            get => _TransactionFeeText;
            set => SetProperty(ref _TransactionFeeText, value);
        }

        public string EstConfirmationText
        {
            get => _EstConfirmationText;
            set => SetProperty(ref _EstConfirmationText, value);
        }

        public double SliderValue
        {
            get => _SliderValue;
            set => SetProperty(ref _SliderValue, value);
        }

        public string AddressToSendTo
        {
            get => _AddressToSendTo;
            set => SetProperty(ref _AddressToSendTo, value);
        }

        public int Fee
        {
            get => _Fee;
            set => SetProperty(ref _Fee, value);
        }

        public int AmountToSend
        {
            get => _AmountToSend;
            set => SetProperty(ref _AmountToSend, value);
        }
        
        public MvxAsyncCommand ScanCommand { get; private set; }
        public MvxAsyncCommand SendCommand { get; private set; }
        public MvxAsyncCommand CloseCommand { get; private set; }
        public MvxAsyncCommand ShowFaqCommand { get; private set; }
        public MvxAsyncCommand OnValueChangedCommand { get; private set; }

        public SendViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService,
            IPrecioService precioService) : base(logProvider, navigationService)
        {
            _WalletService = walletService;
            _PrecioService = precioService;

            ScanCommand = new MvxAsyncCommand(Scan);
            SendCommand = new MvxAsyncCommand(Send);
            CloseCommand = new MvxAsyncCommand(Close);
            ShowFaqCommand = new MvxAsyncCommand(ShowFaq);
            OnValueChangedCommand = new MvxAsyncCommand(SetSliderValue);

            SliderValue = _Maximum * 0.5;
            Task.Run(() => SetSliderValue());
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

        async Task SetSliderValue()
        {
            var currentFees = await _PrecioService.GetFees();

            if (SliderValue <= (_Maximum * 0.25))
            {
                SliderValue = 0;
                Fee = currentFees.SlowSatKB;
                EstConfirmationText = currentFees.SlowTime;
            }
            else if (SliderValue > (_Maximum * 0.25)
                     && SliderValue < (_Maximum * 0.75))
            {
                SliderValue = _Maximum * 0.5;
                Fee = currentFees.NormalSatKB;
                EstConfirmationText = currentFees.NormalTime;
            }
            else
            {
                SliderValue = _Maximum;
                Fee = currentFees.FastestSatKB;
                EstConfirmationText = currentFees.FastestTime;
            }

            TransactionFeeText = string.Format(Constants.SatByteUnit, (Fee / 1000));
        }

        private async Task Send()
        {
            string pin = "";

            if (SecureStorageProvider.HasPin())
            {
                pin = SecureStorageProvider.GetPin();
            }

            var txCreateResult = _WalletService.CreateTransaction(AmountToSend, AddressToSendTo, Fee, pin);

            if (txCreateResult.Success)
            {
                await _WalletService.BroadcastManager.BroadcastTransactionAsync(txCreateResult.Tx);
            }
            else
            {
                // TODO show error screen for now just log it.
                LogProvider.GetLogFor<SendViewModel>().Error(
                    "Error trying to create a transaction.\nAmount to send: {amount}, address: {address}, fee: {fee}, pin: {pin}.\nFull Error: {error}",
                    AmountToSend,
                    AddressToSendTo,
                    Fee,
                    pin,
                    txCreateResult.Error
                );
            }
        }
    }
}