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
        //float rate => PrecioService.Rate.Rate;
        
        Transaction transactionToBroadcast;

        const double MAX_SLIDER_VALUE = 100;

        double sliderValue;
        public double SliderValue
        {
            get => sliderValue;
            set => SetProperty(ref sliderValue, value);
        }

        string addressToSendTo;
        public string AddressToSendTo
        {
            get => addressToSendTo;
            set => SetProperty(ref addressToSendTo, value);
        }

        string fee = string.Empty;
        public string Fee
        {
            get => fee;
            set => SetProperty(ref fee, value);
        }

        Money amount;
        public Money Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        string balance = null;
        public string Balance
        {
            get => balance;
            set => SetProperty(ref balance, value);
        }

        string totalFee = null;
        public string TotalFee
        {
            get => totalFee;
            set => SetProperty(ref totalFee, value);
        }

        string total = null;
        public string Total
        {
            get => total;
            set => SetProperty(ref total, value);
        }

        public ICommand ScanCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand ClearAdressCommand { get; }
        public ICommand SendCommand { get; }
        public ICommand SwitchCurrencyCommand { get; }

        public SendViewModel()
        {
            ScanCommand = new Command(Scan);
            PasteCommand = new Command(async () => await Paste());
            SendCommand = new Command(Send);

            SliderValue = MAX_SLIDER_VALUE * 0.5;

            //Task.Run(SetSliderValue);

            if (WalletService.IsStarted) Setup();
            else WalletService.OnStarted += (_, _) => Setup();
        }

        public async void BroadcastTransaction()
        {
            if (transactionToBroadcast is null) return;

            var (sent, error) = await WalletService.SendTransaction(transactionToBroadcast);

            if (sent)
            {
                await Shell.Current.GoToAsync("../send");
                SliderValue = MAX_SLIDER_VALUE * 0.5;
                DisplayProcessAddressErrorAlert(LocaleResources.Send_transactionSentMessage);
            }
            else
            {
                DisplayProcessAddressErrorAlert($"{Constants.DISPLAY_ALERT_TRANSACTION_MESSAGE}\n{error}");
            }
        }

        public void ProcessBarcodeScannerResult(string result)
        {
            if (string.IsNullOrEmpty(result)) return;

            TryProcessAddress(result, Constants.DISPLAY_ALERT_SCAN_MESSAGE);
        }

        void Setup()
        {
            Balance = DisplayCurrencyService.BitcoinAmountFormatted(
                WalletService.GetCurrentAccountBalanceInBTC(true).ToDecimal(MoneyUnit.BTC)
            );
        }

        //public async Task SetSliderValue()
        //{
        //var currentFees = await PrecioHttpService.GetFeeEstimator();

        //if (SliderValue <= (MAX_SLIDER_VALUE * 0.25))
        //{
        //    SliderValue = 0;
        //    Fee = (currentFees.SlowSatKB / 1000).ToString();
        //    EstConfirmationText = currentFees.SlowTime;
        //}
        //else if (SliderValue > (MAX_SLIDER_VALUE * 0.25)
        //         && SliderValue < (MAX_SLIDER_VALUE * 0.75))
        //{
        //    SliderValue = MAX_SLIDER_VALUE * 0.5;
        //    Fee = (currentFees.NormalSatKB / 1000).ToString();
        //    EstConfirmationText = currentFees.NormalTime;
        //}
        //else
        //{
        //    SliderValue = MAX_SLIDER_VALUE;
        //    Fee = (currentFees.FastestSatKB / 1000).ToString();
        //    EstConfirmationText = currentFees.FastestTime;
        //}

        //if (string.Equals("0", Fee))
        //    Fee = string.Empty;

        //TransactionFeeText = string.Format(Constants.SAT_PER_BYTE_UNIT_LABEL, Fee);
        //}

        internal void CalculateTotals()
        {
            if (Amount is null || Amount == Money.Zero) return;
            if (string.IsNullOrEmpty(Fee)) return;
            if (string.IsNullOrEmpty(AddressToSendTo)) return;

            var (success, tx, fees, _) = WalletService.CreateTransaction(
                Amount.ToDecimal(MoneyUnit.BTC), AddressToSendTo, decimal.Parse(Fee), string.Empty
            );

            if (!success) return;

            TotalFee = DisplayCurrencyService.BitcoinAmountFormatted(new Money((long)fees).ToDecimal(MoneyUnit.BTC));
            Total = DisplayCurrencyService.BitcoinAmountFormatted(tx.TotalOut.ToDecimal(MoneyUnit.BTC));
        }

        void Scan()
        {
            var IsCameraAvailable = PermissionsService.HasCameraPermission();

            if (!IsCameraAvailable) return;

            MessagingCenter.Send(this, "OpenBarcodeScanner");
        }

        async Task Paste()
        {
            if (!Clipboard.HasText)
            {
                DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_PASTE_MESSAGE);

                return;
            }

            var address = await Clipboard.GetTextAsync();

            TryProcessAddress(address, Constants.DISPLAY_ALERT_PASTE_MESSAGE);
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
                    Amount = amount;
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
            string password = string.Empty;

            var amount = Amount.ToDecimal(MoneyUnit.BTC);

            if (amount <= 0.00m || string.IsNullOrEmpty(AddressToSendTo) || long.Parse(Fee) <= 0)
            {
                DisplayProcessAddressErrorAlert(Constants.DISPLAY_ALERT_AMOUNT_MESSAGE);
                return;
            }

            var (Success, Tx, Fees, Error) = WalletService.CreateTransaction(amount, AddressToSendTo, long.Parse(Fee), password);

            Debug.WriteLine($"Creating a tx: success = {Success}, tx = {Tx.ToString()}, fees = {Fees} and error = {Error}");

            if (Success)
            {
                var totalOut = Tx.TotalOut.ToDecimal(MoneyUnit.BTC);
                transactionToBroadcast = Tx;

                MessagingCenter.Send(this, "AskToBroadcastTransaction", (amount, Fees));
            }
            else
            {
                string errorMsg = string.Format("Error trying to create a transaction.\nAmount to send: {0}, address: {1}, fee: {2}, password: {3}.\nFull Error: {4}",
                    amount,
                    AddressToSendTo,
                    Fee,
                    password,
                    Error);

                DisplayProcessAddressErrorAlert(errorMsg, Constants.DISPLAY_ALERT_ERROR_TITLE);

                // TODO show error screen for now just log it.
                Debug.WriteLine($"Error trying to create a transaction.\nAmount to send: {amount}, address: {AddressToSendTo}, fee: {Fee}, password: {password}.\nFull Error: {Error}");
            }
        }
    }
}
