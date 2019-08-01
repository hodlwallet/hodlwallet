using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NBitcoin.RPC;
using Xamarin.Forms;

using HodlWallet2.Core.Services;
using HodlWallet2.UI.Views;

namespace HodlWallet2.Core.ViewModels
{
    public class PinPadViewModel : BaseViewModel
    {
        public string PinPadTitle { get; } = "Enter PIN";
        public string PinPadHeader { get; } = "Your PIN will be used to unlock your wallet and send money.";
        public string PinPadWarning { get; } = "Remember this PIN. If you forget it, you won't be able to access your bitcoin.";

        public ICommand SuccessCommand { get; }

        public PinPadViewModel()
        {
            SuccessCommand = new Command<string>(async (s) => await Success_Callback(s));
        }

        async Task Success_Callback(string pin)
        {
            SavePin(pin);

            MessagingCenter.Send(this, "NavigateToBackupViewModel");
        }

        void SavePin(string pin)
        {
            SecureStorageService.SetPin(pin);
        }
    }
}
