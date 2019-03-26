using System;
using Serilog;
using System.Threading.Tasks;
using System.ComponentModel;

using Xamarin.Essentials;

namespace HodlWallet2.ViewModels
{
    [Obsolete]
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

        public Task ToClipboard()
        {
            // TODO: Clipboard Animation
            return Clipboard.SetTextAsync(Address);
        }
    }
}
