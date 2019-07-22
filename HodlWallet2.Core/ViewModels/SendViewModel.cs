using System;
using System.Linq;
using System.Threading.Tasks;

using NBitcoin;
using NBitcoin.Payment;

using Xamarin.Forms;
using Xamarin.Essentials;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using HodlWallet2.Core.Interactions;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Utils;

using Liviano;
using Liviano.Exceptions;

namespace HodlWallet2.Core.ViewModels
{
    public class SendViewModel : BaseViewModel
    {
        readonly IWalletService _WalletService;
        readonly IPrecioService _PrecioService;
        readonly Serilog.ILogger _Logger;

        MvxInteraction<YesNoQuestion> _QuestionInteraction = new MvxInteraction<YesNoQuestion>();
        public IMvxInteraction<YesNoQuestion> QuestionInteraction => _QuestionInteraction;

        MvxInteraction<BarcodeScannerPrompt> _BarcodeScannerInteraction = new MvxInteraction<BarcodeScannerPrompt>();
        public IMvxInteraction<BarcodeScannerPrompt> BarcodeScannerInteraction => _BarcodeScannerInteraction;

        MvxInteraction<DisplayAlertContent> _DisplayAlertInteraction = new MvxInteraction<DisplayAlertContent>();
        public IMvxInteraction<DisplayAlertContent> DisplayAlertInteraction => _DisplayAlertInteraction;

        string _AddressToSendTo;
        int _Fee;
        decimal _AmountToSend;
        float _Rate;
        string _AmountToSendText;

        const double MAX_SLIDER_VALUE = 100;
        double _SliderValue;

        string _TransactionFeeText;
        string _EstConfirmationText;
        string _ISOLabel;

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

        public string AmountToSendText
        {
            get => _AmountToSendText;
            set => SetProperty(ref _AmountToSendText, value);
        }

        public string ISOLabel
        {
            get => _ISOLabel;
            set => SetProperty(ref _ISOLabel, value);
        }

        public MvxAsyncCommand ScanCommand { get; }
        public MvxAsyncCommand PasteCommand { get; }
        public MvxAsyncCommand SendCommand { get; }
        public MvxAsyncCommand CloseCommand { get; }
        public MvxAsyncCommand ShowFaqCommand { get; }
        public MvxAsyncCommand OnSliderValueChangedCommand { get; }
        public MvxAsyncCommand SwitchCurrencyCommand { get; }

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
            SendCommand = new MvxAsyncCommand(Send);
            CloseCommand = new MvxAsyncCommand(Close);
            ShowFaqCommand = new MvxAsyncCommand(ShowFaq);
            OnSliderValueChangedCommand = new MvxAsyncCommand(SetSliderValue);
            SwitchCurrencyCommand = new MvxAsyncCommand(SwitchCurrency);

            SliderValue = MAX_SLIDER_VALUE * 0.5;

            Task.Run(SetSliderValue);
        }

        public override async void ViewAppearing()
        {
            base.ViewAppearing();

            ISOLabel = "USD($)";
            //TODO: Create constants list for code instead of hardcoding them.
            var currencyEntity = (await _PrecioService.GetRates()).SingleOrDefault(r => r.Code == "USD");
            if (currencyEntity != null)
            {
                _Rate = currencyEntity.Rate;
            }
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

        private Task SwitchCurrency()
        {
            if (ISOLabel == "USD($)") //TODO: Refactor with more user currencies
            {
                AmountToSend = Convert.ToDecimal(AmountToSendText) / (decimal)_Rate;
                AmountToSendText = AmountToSend.ToString();
                ISOLabel = "BTC";
            }
            else
            {
                AmountToSend = Convert.ToDecimal(AmountToSend) * (decimal)_Rate;
                AmountToSendText = $"{AmountToSend:F2}";
                ISOLabel = "USD($)";
            }
            return Task.FromResult(this);
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

            if (!IsCameraAvailable) return;

            string address = "";
            var request = new BarcodeScannerPrompt
            {
                ResultCallback = (ZXing.Result result) =>
                {
                    // If we already scanned this we get out
                    // this is done because ZXing is weird
                    // and keeps sending scans
                    if (result.Text == address) return;

                    address = result.Text;

                    TryProcessAddress(address, Constants.DISPLAY_ALERT_SCAN_MESSAGE);
                }
            };
            _BarcodeScannerInteraction.Raise(request);
        }

        async Task Paste()
        {
            if (!Clipboard.HasText)
            {
                DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_PASTE_MESSAGE);

                return;
            }

            string address = await Clipboard.GetTextAsync();

            TryProcessAddress(address, Constants.DISPLAY_ALERT_PASTE_MESSAGE);
        }

        void TryProcessAddress(string address, string errorMessage)
        {
            if (IsBitcoinAddress(address))
            {
                if (!_WalletService.IsAddressOwn(address))
                {
                    AddressToSendTo = address;

                    return;
                }

                DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_ERROR_SEND_TO_YOURSELF);

                return;
            }

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
            catch (WalletException we)
            {
                _Logger.Information(we.Message);

                DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_ERROR_BIP70);
            }
            catch (Exception ex)
            {
                _Logger.Information(
                    "Unable to extract address from QR code: {address}, {error}",
                    address,
                    ex.Message
                );

                DisplayProcessAddressErrorAlert(errorMessage);
            }
        }

        void DisplayProcessAddressErrorAlert(string errorMessage, string title = null)
        {
            var request = new DisplayAlertContent
            {
                Title = title ?? Constants.DISPLAY_ALERT_ERROR_TITLE,
                Message = errorMessage,
                Buttons = new string[] { Constants.DISPLAY_ALERT_ERROR_BUTTON }
            };

            _DisplayAlertInteraction.Raise(request);
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

        async Task Send()
        {
            string password = "";

            if (AmountToSend <= 0.00m || string.IsNullOrEmpty(AddressToSendTo) || Fee <= 0)
            {
                var validationErrorRequest = new DisplayAlertContent
                {
                    Title = Constants.DISPLAY_ALERT_ERROR_TITLE,
                    Message = "Unable to send, check your amount, address and fee",
                    Buttons = new string[] { Constants.DISPLAY_ALERT_ERROR_BUTTON }
                };

                _DisplayAlertInteraction.Raise(validationErrorRequest);

                return;
            }

            var (Success, Tx, Fees, Error) = _WalletService.CreateTransaction(AmountToSend, AddressToSendTo, Fee, password);

            _Logger.Debug($"Creating a tx: success = {Success}, tx = {Tx.ToString()}, fees = {Fees} and error = {Error}");
            if (Success)
            {
                // TODO Show yes no dialog to broadcast it or not
                await _WalletService.BroadcastManager.BroadcastTransactionAsync(Tx);


                return;
            }
            else
            {
                string errorMsg = string.Format("Error trying to create a transaction.\nAmount to send: {0}, address: {1}, fee: {2}, password: {3}.\nFull Error: {4}",
                    AmountToSend,
                    AddressToSendTo,
                    Fee,
                    password,
                    Error);
                var transactionErrorRequest = new DisplayAlertContent
                {
                    Title = Constants.DISPLAY_ALERT_ERROR_TITLE,
                    Message = errorMsg,
                    Buttons = new string[] { Constants.DISPLAY_ALERT_ERROR_BUTTON }
                };

                _DisplayAlertInteraction.Raise(transactionErrorRequest);

                // TODO show error screen for now just log it.
                _Logger.Error(
                    "Error trying to create a transaction.\nAmount to send: {amount}, address: {address}, fee: {fee}, password: {password}.\nFull Error: {error}",
                    AmountToSend,
                    AddressToSendTo,
                    Fee,
                    password,
                    Error
                );
            }
        }
    }
}
