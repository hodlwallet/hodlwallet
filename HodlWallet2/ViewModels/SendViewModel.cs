using System;
using System.ComponentModel;
using System.Threading.Tasks;
using HodlWallet2.Core.Services;
using Serilog;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace HodlWallet2.ViewModels
{
    [Obsolete]
    public class SendViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        WalletService _Wallet;

        ILogger _Logger;

        public SendViewModel()
        {
            _Wallet = (WalletService) WalletService.Instance;
            _Logger = _Wallet.Logger;
        }

        public async Task<string> Scan()
        {
            var scanner = new MobileBarcodeScanner();

            var result = await scanner.Scan();

            AddressToSendTo = result.Text;

            return result.Text;
        }

        private string _AddressToSendTo;
        public string AddressToSendTo
        {
            get
            {
                return _AddressToSendTo;
            }
            set
            {
                if (value != _AddressToSendTo)
                {
                    _AddressToSendTo = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddressToSendTo)));
                }
            }
        }

        private int _Fee;
        public int Fee
        {
            get
            {
                return _Fee;
            }
            set
            {
                if (value != _Fee)
                {
                    _Fee = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Fee)));
                }
            }
        }

        private decimal _AmountToSend;
        public decimal AmountToSend
        {
            get
            {
                return _AmountToSend;
            }
            set
            {
                if (value != _AmountToSend)
                {
                    _AmountToSend = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AmountToSend)));
                }
            }
        }

        public async Task Send()
        {
            string password = "123456";
            var txCreateResult = _Wallet.CreateTransaction(AmountToSend, AddressToSendTo, Fee, password);

            if (txCreateResult.Success)
            {
                await _Wallet.BroadcastManager.BroadcastTransactionAsync(txCreateResult.Tx);
            }
            else
            {
                // TODO show error screen for now just log it.
                _Logger.Error(
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
