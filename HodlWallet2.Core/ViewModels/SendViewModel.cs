using System;
using System.Threading.Tasks;
using Liviano;
using NBitcoin.Payment;
using Xamarin.Forms;
using Xamarin.Essentials;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

using ZXing.Mobile;

using MvvmCross.ViewModels;

using HodlWallet2.Core.Interactions;
using HodlWallet2.Core.Interfaces;
using Constants = HodlWallet2.Core.Utils.Constants;
using Liviano.Exceptions;
using NBitcoin;

namespace HodlWallet2.Core.ViewModels
{
    public class SendViewModel : BaseViewModel
    {
        readonly IWalletService _WalletService;
        readonly IPrecioService _PrecioService;
        readonly Serilog.ILogger _Logger;

        MvxInteraction<YesNoQuestion> _QuestionInteraction = new MvxInteraction<YesNoQuestion>();
        public IMvxInteraction<YesNoQuestion> QuestionInteraction => _QuestionInteraction;

        public MvxInteraction<BarcodeScannerPrompt> BarcodeScannerInteraction { get; } = new MvxInteraction<BarcodeScannerPrompt>();
        public MvxInteraction<DisplayAlertContent> DisplayAlertInteraction { get; } = new MvxInteraction<DisplayAlertContent>();

        string _AddressToSendTo;
        int _Fee;
        decimal _AmountToSend;

        const double MAX_SLIDER_VALUE = 100;
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

        public decimal AmountToSend
        {
            get => _AmountToSend;
            set => SetProperty(ref _AmountToSend, value);
        }

        public MvxAsyncCommand ScanCommand { get; }
        public MvxAsyncCommand PasteCommand { get; }
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
            _Logger = _WalletService.Logger;
            _PrecioService = precioService;

            ScanCommand = new MvxAsyncCommand(Scan);
            PasteCommand = new MvxAsyncCommand(Paste);
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
            var IsCameraAvailable = DependencyService.Get<IPermissions>().HasCameraPermission();

            if (IsCameraAvailable)
            {
                int n = 0;
                string address = "";
                var request = new BarcodeScannerPrompt
                {
                    ResultCallback = async (ZXing.Result result) =>
                    {
                        _Logger.Debug($"Running this for {++n} time");

                        // If we already scanned this we get out
                        // this is done because ZXing is weird
                        // and keeps sending scans
                        if (result.Text == address) return;

                        address = result.Text;

                        if (IsBitcoinAddress(address))
                        {
                            AddressToSendTo = address;
                        }
                        else
                        {
                            try
                            {
                                var bitcoinUrl = new BitcoinUrlBuilder(address);

                                if (bitcoinUrl.Address is BitcoinAddress addr)
                                    AddressToSendTo = addr.ToString();

                                if (bitcoinUrl.Amount is Money amount)
                                {
                                    // TODO This has to decide depending on the currency,
                                    // if the user is in USD we should switch them to BTC
                                    // once currency flip is available then we can do this
                                    AmountToSend = amount.ToDecimal(MoneyUnit.BTC);
                                }

                                if (bitcoinUrl.PaymentRequestUrl is Uri paymentRequestUrl)
                                    throw new WalletException($"HODL Wallet does not support BIP70");
                            }
                            catch (Exception ex)
                            {
                                _Logger.Debug(
                                    "Unable to extract address from QR code: {address}, {error}",
                                    address,
                                    ex.Message
                                );

                                var req = new DisplayAlertContent
                                {
                                    Title = Constants.DISPLAY_ALERT_ERROR_TITLE,
                                    Message = Constants.DISPLAY_ALERT_SCAN_MESSAGE,
                                    Buttons = new string[] { Constants.DISPLAY_ALERT_ERROR_BUTTON }
                                };

                                DisplayAlertInteraction.Raise(req);
                            }
                        }
                    }
                };
                BarcodeScannerInteraction.Raise(request);
            }
        }

        private async Task Paste()
        {
            if (Clipboard.HasText)
            {
                var content = await Clipboard.GetTextAsync();

                if (content.IsBitcoinAddress(_WalletService.GetNetwork()))
                {
                    AddressToSendTo = content;
                }
                else
                {
                    try
                    {
                        var urlBuilder = new BitcoinUrlBuilder(content);
                        AddressToSendTo = urlBuilder.Address.ToString();
                    }
                    catch (Exception ex)
                    {
                        LogProvider.GetLogFor<SendViewModel>().Error(
                            "Unable to extract address from clipboard: {address}, {message}",
                            content,
                            ex.Message
                        );

                        var request = new DisplayAlertContent
                        {
                            Title = Constants.DISPLAY_ALERT_ERROR_TITLE,
                            Message = Constants.DISPLAY_ALERT_PASTE_MESSAGE,
                            Buttons = new string[] { Constants.DISPLAY_ALERT_ERROR_BUTTON }
                        };

                        DisplayAlertInteraction.Raise(request);
                    }
                }
            }
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
            return content.IsBitcoinAddress(_WalletService.GetNetwork());
        }

        bool IsBitcoinAddressReused(string address)
        {
            return _WalletService.IsAddressOwn(address);
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
