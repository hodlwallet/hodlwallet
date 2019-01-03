using Serilog;
using System.ComponentModel;

namespace HodlWallet2.ViewModels
{
    public class ReceiveViewModel : INotifyPropertyChanged
    {
        private ILogger _Logger;
        private Wallet _Wallet;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _Address;
        public string Address
        {
            get
            {
                return _Address;
            }

            set
            {
                _Address = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Address)));
            }
        }

        public ReceiveViewModel()
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;
           
            Address = _Wallet.GetReceiveAddress().Address;

            _Logger.Information("New Receive Address: {address}", Address);
        }

    }
}
