using Serilog;
using Xamarin.Forms;

namespace HodlWallet2.Views
{
    public partial class BaseView : ContentPage
    {
        protected Wallet _Wallet;
        protected ILogger _Logger;

        public BaseView()
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;
        }
    }
}
