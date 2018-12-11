using Serilog;

namespace HodlWallet2.ViewModels
{
    public class ReceiveViewModel
    {
        private ILogger _Logger;
        private Wallet _Wallet;
        public string Address { get; }

        public ReceiveViewModel()
        {
            _Wallet = Wallet.Instance;

            _Logger = _Wallet.Logger;
           
            Address = _Wallet.GetReceiveAddress().Address;

            _Logger.Information("New Receive Address: {address}", Address);
        }

    }
}
