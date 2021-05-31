using System;
using System.Diagnostics;
using System.Windows.Input;

using Xamarin.Forms;


namespace HodlWallet.Core.ViewModels
{
    class CreateAccountViewModel : BaseViewModel
    {
        public ICommand CreateAccCommand { get; }

        string _AccountName;

        public string AccountName
        {
            get => _AccountName;
            set => SetProperty(ref _AccountName, value);
        }

        string _AccountType;

        public string AccountType
        {
            get => _AccountType;
            set => SetProperty(ref _AccountType, value);
        }
        public CreateAccountViewModel()
        {
            CreateAccCommand = new Command(CreateAccount);
        }
        private async void CreateAccount()
        {
            Debug.WriteLine($"[CreateAcount Before] Trying to create a new account => Account Type: {AccountType} Account Name: {AccountName} ");
            var (Success, Error) = await WalletService.AddAccount(AccountType ?? "bip84", AccountName ?? "Karen's Bitcoin Account");
            Debug.WriteLine($"[CreateAcount] Trying to create a new account => Account Type: {AccountType} Account Name: {AccountName} | Result => Success: {Success} Error: {Error} ");
            await Shell.Current.GoToAsync("..");
        }
    }
}
