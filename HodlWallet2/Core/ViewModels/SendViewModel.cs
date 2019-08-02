using System;
using System.Threading.Tasks;
using System.Windows.Input;

using NBitcoin;
using NBitcoin.Payment;

using Xamarin.Forms;
using Xamarin.Essentials;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Utils;

using Liviano;
using Liviano.Exceptions;
using HodlWallet2.UI.Views;
using NBitcoin.Protocol;

namespace HodlWallet2.Core.ViewModels
{
    public class SendViewModel : BaseViewModel
    {
        Serilog.ILogger _Logger;
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

        public ICommand ScanCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand SendCommand { get; }
        public ICommand OnSliderValueChangedCommand { get; }
        public ICommand SwitchCurrencyCommand { get; }

        public SendViewModel()
        {
            ScanCommand = new Command(async () => await Scan());
            PasteCommand = new Command(async () => await Paste());
            SendCommand = new Command(async () => await Send());
            OnSliderValueChangedCommand = new Command(async () => await SetSliderValue());
            SwitchCurrencyCommand = new Command(async () => await SwitchCurrency());

            SliderValue = MAX_SLIDER_VALUE * 0.5;

            _Logger = _WalletService.Logger;

            Task.Run(SetSliderValue);
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

        async Task Scan()
        {
            var IsCameraAvailable = _PermissionsService.HasCameraPermission();

            if (!IsCameraAvailable) return;

            string address = "";

            MessagingCenter.Send(this, "OpenBarcodeScanner");

            MessagingCenter.Subscribe<SendView, string>(this, "BarcodeScannerResult", (v, result) =>
            {
                if (string.IsNullOrEmpty(result)) return;

                TryProcessAddress(result, Constants.DISPLAY_ALERT_SCAN_MESSAGE);
            });
        }

        async Task Paste()
        {
            if (!Clipboard.HasText)
            {
                //DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_PASTE_MESSAGE);

                return;
            }

            if (_WalletService.IsStarted)
            {
                string address = await Clipboard.GetTextAsync();

                TryProcessAddress(address, Constants.DISPLAY_ALERT_PASTE_MESSAGE);

                return;
            }

            _WalletService.OnStarted += _WalletService_OnStarted_PasteAddress;
        }

        private void _WalletService_OnStarted_PasteAddress(object sender, EventArgs e)
        {
            Device.InvokeOnMainThreadAsync(async () =>
            {
                string address = await Clipboard.GetTextAsync();

                TryProcessAddress(address, Constants.DISPLAY_ALERT_PASTE_MESSAGE);
            });
        }

        void DisplayProcessAddressErrorAlert(string errorMessage, string title = null)
        {
            MessagingCenter.Send(this, "DisplayProcessAlertError", new string[] { errorMessage, title });
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
                string message = "Unable to send, check your amount, address and fee";

                DisplayProcessAddressErrorAlert(message, Constants.DISPLAY_ALERT_ERROR_TITLE);
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

                DisplayProcessAddressErrorAlert(errorMsg, Constants.DISPLAY_ALERT_ERROR_TITLE);

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
