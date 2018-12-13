using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ZXing.Mobile;

namespace HodlWallet2.ViewModels
{
    public class SendViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Wallet _Wallet;

        public SendViewModel()
        {
            _Wallet = Wallet.Instance;
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
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("AddressToSendTo"));
                }
            }
        }

        private decimal _Fee;
        public decimal Fee
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
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Fee"));
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
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("AmountToSend"));
                }
            }
        }

        public async void Send()
        {

        }
    }
}
