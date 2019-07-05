using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Essentials;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

using NBitcoin;

using Serilog.Core;

using ZXing.Mobile;

using Liviano;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.Interactions;
using Constants = HodlWallet2.Core.Utils.Constants;

using MvvmCross.ViewModels;
using MvvmCross.Base;
using MvvmCross;

namespace HodlWallet2.Core.ViewModels
{
    public class SendViewModel : BaseViewModel
    {
        readonly IWalletService _WalletService;
        readonly IPrecioService _PrecioService;

        MvxInteraction<YesNoQuestion> _QuestionInteraction = new MvxInteraction<YesNoQuestion>();
        public IMvxInteraction<YesNoQuestion> QuestionInteraction => _QuestionInteraction;

        string _AddressToSendTo;
        int _Fee;
        int _AmountToSend;

        const double MAX_SLIDER_VALUE = 500;
        double _SliderValue;

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

        public MvxAsyncCommand ScanCommand { get; }
        public MvxAsyncCommand<string> SendCommand { get; }
        public MvxAsyncCommand CloseCommand { get; }
        public MvxAsyncCommand ShowFaqCommand { get; }
        public MvxAsyncCommand OnValueChangedCommand { get; }

        public SendViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService,
            IPrecioService precioService) : base(logProvider, navigationService)
        {
            _WalletService = walletService;
            _PrecioService = precioService;

            ScanCommand = new MvxAsyncCommand(Scan);
            SendCommand = new MvxAsyncCommand<string>(Send);
            CloseCommand = new MvxAsyncCommand(Close);
            ShowFaqCommand = new MvxAsyncCommand(ShowFaq);
            OnValueChangedCommand = new MvxAsyncCommand(SetSliderValue);

            SliderValue = MAX_SLIDER_VALUE * 0.5;
            
            Task.Run(SetSliderValue);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
        }

        public async Task ProcessAddressOnClipboardToPaste()
        {
            string content = await Clipboard.GetTextAsync();

            if (!IsBitcoinAddress(content) || IsBitcoinAddressReused(content)) return;

            var request = new YesNoQuestion
            {
                QuestionKey = "use-address-on-clipboard",
                AnswerCallback = (yes) =>
                {
                    if (yes) AddressToSendTo = content;
                }
            };
            _QuestionInteraction.Raise(request);
        }

        Task ShowFaq()
        {
            //TODO: Implement FAQ
            return Task.FromResult(this);
        }

        async Task Close()    
        {
            await NavigationService.Close(this);
        }

        async Task Scan()
        {
            //TODO: Implement Scan
            var scanner = new MobileBarcodeScanner();

            var result = await scanner.Scan();

            AddressToSendTo = result.Text;
        }

        async Task SetSliderValue()
        {
            var currentFees = await _PrecioService.GetFees();

            if (SliderValue <= (MAX_SLIDER_VALUE * 0.25))
            {
                SliderValue = 0;
                Fee = currentFees.SlowSatKB;
                EstConfirmationText = currentFees.SlowTime;
            }
            else if (SliderValue > (MAX_SLIDER_VALUE * 0.25)
                     && SliderValue < (MAX_SLIDER_VALUE * 0.75))
            {
                SliderValue = MAX_SLIDER_VALUE * 0.5;
                Fee = currentFees.NormalSatKB;
                EstConfirmationText = currentFees.NormalTime;
            }
            else
            {
                SliderValue = MAX_SLIDER_VALUE;
                Fee = currentFees.FastestSatKB;
                EstConfirmationText = currentFees.FastestTime;
            }

            TransactionFeeText = string.Format(Constants.SAT_PER_BYTE_UNIT_LABEL, (Fee / 1000));
        }

        bool IsBitcoinAddress(string content)
        {
            return content.IsBitcoinAddress(_WalletService.WalletManager.Network);
        }

        bool IsBitcoinAddressReused(string address)
        {
            return _WalletService.IsAddressReused(address);
        }

        async Task Send(string password = "")
        {
            var txCreateResult = _WalletService.CreateTransaction(AmountToSend, AddressToSendTo, Fee, password);

            if (txCreateResult.Success)
            {
                await _WalletService.BroadcastManager.BroadcastTransactionAsync(txCreateResult.Tx);
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
