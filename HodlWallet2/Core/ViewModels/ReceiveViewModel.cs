using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class ReceiveViewModel : BaseViewModel
    {
        public string ShareButtonText => "Share";
        public ICommand ShowShareIntentCommand { get; }

        string _Address;
        public string Address
        {
            get => _Address;
            set => SetProperty(ref _Address, value);
        }

        public ReceiveViewModel()
        {
            ShowShareIntentCommand = new Command(ShowShareIntent);

            if (_WalletService.IsStarted)
            {
                GetAddressFromWallet();
            }
            else
            {
                _WalletService.OnStarted += _WalletService_OnStarted;
            }
        }

        private void _WalletService_OnStarted(object sender, EventArgs e)
        {
            GetAddressFromWallet();
        }

        void GetAddressFromWallet()
        {
            Address = _WalletService.GetReceiveAddress().Address;

            Debug.WriteLine($"[GetAddressFromWallet] New address: {Address}");
        }

        void ShowShareIntent()
        {
            Debug.WriteLine($"[ShowShareIntent] Sharing address: {Address}");

            _ShareIntent.QRTextShareIntent(Address);
        }
    }
}
