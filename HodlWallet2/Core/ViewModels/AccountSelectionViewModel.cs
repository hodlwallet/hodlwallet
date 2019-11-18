using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class AccountSelectionViewModel : BaseViewModel
    {
        public string AccountSelectionTitle => "Select an Account";

        public string HeaderText => "Please enter a name for this account.";

        public string SubheaderText => "Choose from the options below which type of account you want to create.";

        string _Name;
        public string Name
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
        }

        public ICommand AccountCommand;

        public string AccountType;

        public AccountSelectionViewModel()
        {
            AccountCommand = new Command<string>(
                (string arg) =>
                {
                    switch (arg)
                    {
                        case "bip44":
                        case "bip49":
                        case "bip84":
                        case "bip141":
                        case "wasabi":
                        case "paper":
                            _WalletService.Wallet.AddAccount(arg, Name);
                            break;
                        default:
                            _WalletService.Wallet.AddAccount("bip141", Name);
                            break;
                    }
                });
        }
    }
}
