//
// SendViewModel.cs
//
// Copyright (c) 2019 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Threading.Tasks;
using System.Windows.Input;

using NBitcoin;
using NBitcoin.Payment;

using Xamarin.Forms;
using Xamarin.Essentials;

using HodlWallet2.Core.Utils;

using Liviano;
using Liviano.Exceptions;
using Liviano.Extensions;
using HodlWallet2.UI.Views;
using System.Diagnostics;

namespace HodlWallet2.Core.ViewModels
{
    public class SendViewModel : BaseViewModel
    {
        string _AddressToSendTo;
        int _Fee;
        decimal _AmountToSend;
        float _Rate => _PrecioService.Rate.Rate;
        string _AmountToSendText;
        Transaction _TransactionToBroadcast;

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
            ScanCommand = new Command(Scan);
            PasteCommand = new Command(() => _ = Paste());
            SendCommand = new Command(Send);
            OnSliderValueChangedCommand = new Command(() => _ = SetSliderValue());
            SwitchCurrencyCommand = new Command(SwitchCurrency);

            SliderValue = MAX_SLIDER_VALUE * 0.5;

            Task.Run(SetSliderValue);

            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<SendView>(this, "BroadcastTransaction", BroadcastTransaction);
        }

        async void BroadcastTransaction(SendView _)
        {
            if (_TransactionToBroadcast is null) return;

            var result = await _WalletService.SendTransaction(_TransactionToBroadcast);

            if (result.Sent == true)
            {
                MessagingCenter.Send(this, "ChangeCurrentPageTo", RootView.Tabs.Home);
            }
            else
            {
                DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_TRANSACTION_MESSAGE);
            }
        }

        void SwitchCurrency()
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
        }

        async Task SetSliderValue()
        {
            var currentFees = await _PrecioHttpService.GetFeeEstimator();

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

        void Scan()
        {
            var IsCameraAvailable = _PermissionsService.HasCameraPermission();

            if (!IsCameraAvailable) return;

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
                DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_PASTE_MESSAGE);

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

        void _WalletService_OnStarted_PasteAddress(object sender, EventArgs e)
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

#pragma warning disable CS0618 // Type or member is obsolete
                if (bitcoinUrl.PaymentRequestUrl is Uri paymentRequestUrl)
#pragma warning restore CS0618 // Type or member is obsolete
                    throw new WalletException($"HODL Wallet does not support BIP70");
            }
            catch (WalletException we)
            {
                Debug.WriteLine(we.Message);

                DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_ERROR_BIP70);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to extract address from QR code: {address}, {ex.Message}");

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

        void Send()
        {
            string password = "";

            if (AmountToSend <= 0.00m || string.IsNullOrEmpty(AddressToSendTo) || Fee <= 0)
            {
                DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_AMOUNT_MESSAGE);
                return;
            }

            var (Success, Tx, Fees, Error) = _WalletService.CreateTransaction(AmountToSend, AddressToSendTo, Fee, password);

            Debug.WriteLine($"Creating a tx: success = {Success}, tx = {Tx.ToString()}, fees = {Fees} and error = {Error}");

            if (Success)
            {
                var totalOut = Tx.TotalOut.ToDecimal(MoneyUnit.BTC);
                _TransactionToBroadcast = Tx;

                MessagingCenter.Send(this, "AskToBroadcastTransaction", (totalOut, Fees));
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
                Debug.WriteLine($"Error trying to create a transaction.\nAmount to send: {AmountToSend}, address: {AddressToSendTo}, fee: {Fee}, password: {password}.\nFull Error: {Error}");
            }
        }
    }
}
