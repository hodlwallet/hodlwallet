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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

using NBitcoin;
using NBitcoin.Payment;

using Xamarin.Forms;
using Xamarin.Essentials;

using HodlWallet.Core.Utils;
using HodlWallet.UI.Views;
using HodlWallet.UI.Locale;

using Liviano.Exceptions;
using Liviano.Extensions;

namespace HodlWallet.Core.ViewModels
{
    public class SendViewModel : BaseViewModel
    {
        string addressToSendTo;
        string fee = string.Empty;
        decimal amountToSend;
        //float rate => PrecioService.Rate.Rate;
        string amountToSendText;
        Transaction transactionToBroadcast;

        const double MAX_SLIDER_VALUE = 100;
        double sliderValue;

        string transactionFeeText;
        string estConfirmationText;
        string isoLabel = "BTC";

        public string TransactionFeeText
        {
            get => transactionFeeText;
            set => SetProperty(ref transactionFeeText, value);
        }

        public string EstConfirmationText
        {
            get => estConfirmationText;
            set => SetProperty(ref estConfirmationText, value);
        }

        public double SliderValue
        {
            get => sliderValue;
            set => SetProperty(ref sliderValue, value);
        }

        public string AddressToSendTo
        {
            get => addressToSendTo;
            set => SetProperty(ref addressToSendTo, value);
        }

        public string Fee
        {
            get => fee;
            set => SetProperty(ref fee, value);
        }

        public decimal AmountToSend
        {
            get => amountToSend;
            set => SetProperty(ref amountToSend, value);
        }

        public string AmountToSendText
        {
            get => amountToSendText;
            set => SetProperty(ref amountToSendText, value);
        }

        public string ISOLabel
        {
            get => isoLabel;
            set => SetProperty(ref isoLabel, value);
        }

        public ICommand ScanCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand ClearAdressCommand { get; }
        public ICommand SendCommand { get; }
        public ICommand SwitchCurrencyCommand { get; }
        public ICommand ClearAmountCommand { get; }

        public SendViewModel()
        {
            ScanCommand = new Command(Scan);
            PasteCommand = new Command(() => _ = Paste());
            ClearAdressCommand = new Command(ClearAdress);
            SendCommand = new Command(Send);
            SwitchCurrencyCommand = new Command(SwitchCurrency);
            ClearAmountCommand = new Command(ClearAmount);

            SliderValue = MAX_SLIDER_VALUE * 0.5;

            Task.Run(SetSliderValue);

            SubscribeToMessages();
        }

        public async Task SetSliderValue()
        {
            var currentFees = await PrecioHttpService.GetFeeEstimator();

            if (SliderValue <= (MAX_SLIDER_VALUE * 0.25))
            {
                SliderValue = 0;
                Fee = (currentFees.SlowSatKB / 1000).ToString();
                EstConfirmationText = currentFees.SlowTime;
            }
            else if (SliderValue > (MAX_SLIDER_VALUE * 0.25)
                     && SliderValue < (MAX_SLIDER_VALUE * 0.75))
            {
                SliderValue = MAX_SLIDER_VALUE * 0.5;
                Fee = (currentFees.NormalSatKB / 1000).ToString();
                EstConfirmationText = currentFees.NormalTime;
            }
            else
            {
                SliderValue = MAX_SLIDER_VALUE;
                Fee = (currentFees.FastestSatKB / 1000).ToString();
                EstConfirmationText = currentFees.FastestTime;
            }

            if (string.Equals("0", Fee))
                Fee = string.Empty;

            TransactionFeeText = string.Format(Constants.SAT_PER_BYTE_UNIT_LABEL, Fee);
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<SendView>(this, "BroadcastTransaction", BroadcastTransaction);
        }

        async void BroadcastTransaction(SendView _)
        {
            if (transactionToBroadcast is null) return;

            var (sent, error) = await WalletService.SendTransaction(transactionToBroadcast);

            if (sent == true)
            {
                await Shell.Current.GoToAsync("../send");
                AddressToSendTo = "";
                AmountToSend = 0;
                SliderValue = MAX_SLIDER_VALUE * 0.5;
                DisplayProcessAddressErrorAlert(LocaleResources.Send_transactionSentMessage);
            }
            else
            {
                DisplayProcessAddressErrorAlert($"{Constants.DISPLAY_ALERT_TRANSACTION_MESSAGE}\n{error}");
            }
        }

        void SwitchCurrency()
        {
            if (ISOLabel == "USD($)") //TODO: Refactor with more user currencies
            {
                //AmountToSend = Convert.ToDecimal(AmountToSendText) / (decimal)rate;
                AmountToSendText = AmountToSend.ToString();
                ISOLabel = "BTC";
            }
            else
            {
                //AmountToSend = Convert.ToDecimal(AmountToSend) * (decimal)rate;
                AmountToSendText = $"{AmountToSend:F2}";
                ISOLabel = "USD($)";
            }
        }

        void Scan()
        {
            var IsCameraAvailable = PermissionsService.HasCameraPermission();

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

            if (WalletService.IsStarted)
            {
                string address = await Clipboard.GetTextAsync();

                TryProcessAddress(address, Constants.DISPLAY_ALERT_PASTE_MESSAGE);

                return;
            }

            WalletService.OnStarted += _WalletService_OnStarted_PasteAddress;
        }

        void ClearAdress()
        {
            AddressToSendTo = "";
        }

        void ClearAmount()
        {
            AmountToSend = 0;
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
                if (!WalletService.IsAddressOwn(address))
                {
                    AddressToSendTo = address;

                    return;
                }

                DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_ERROR_SEND_TO_YOURSELF);

                return;
            }

            try
            {
                var bitcoinUrl = new BitcoinUrlBuilder(address, WalletService.GetNetwork());

                if (bitcoinUrl.Address is BitcoinAddress addr)
                    AddressToSendTo = addr.ToString();

                if (bitcoinUrl.Amount is Money amount)
                {
                    // TODO This has to decide depending on the currency,
                    // if the user is in USD we should switch them to BTC
                    // once currency flip is available then we can do this
                    AmountToSend = amount.ToDecimal(MoneyUnit.BTC);
                }
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
            return content.IsBitcoinAddress(WalletService.GetNetwork());
        }

        bool IsBitcoinAddressReused(string address)
        {
            return WalletService.IsAddressOwn(address);
        }

        void Send()
        {
            string password = "";

            if (AmountToSend <= 0.00m || string.IsNullOrEmpty(AddressToSendTo) || long.Parse(Fee) <= 0)
            {
                DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_AMOUNT_MESSAGE);
                return;
            }

            var (Success, Tx, Fees, Error) = WalletService.CreateTransaction(AmountToSend, AddressToSendTo, long.Parse(Fee), password);

            Debug.WriteLine($"Creating a tx: success = {Success}, tx = {Tx.ToString()}, fees = {Fees} and error = {Error}");

            if (Success)
            {
                var totalOut = Tx.TotalOut.ToDecimal(MoneyUnit.BTC);
                transactionToBroadcast = Tx;

                MessagingCenter.Send(this, "AskToBroadcastTransaction", (AmountToSend, Fees));
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
