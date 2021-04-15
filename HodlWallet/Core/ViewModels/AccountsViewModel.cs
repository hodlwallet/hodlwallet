using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet.Core.Models;
using HodlWallet.UI.Views;

namespace HodlWallet.Core.ViewModels
{
    public class AccountsViewModel : BaseViewModel
    {
        string _PriceText;
        AccountModel _CurrentAccount;

        public string PriceText
        {
            get => _PriceText;
            set => SetProperty(ref _PriceText, value);
        }

        object _Lock = new object();
        public ObservableCollection<AccountModel> Accounts { get; } = new ObservableCollection<AccountModel>();

        public AccountModel CurrentAccount
        {
            get => _CurrentAccount;
            set => SetProperty(ref _CurrentAccount, value);
        }

        public ICommand NavigateToHomeCommand { get; }

        public AccountsViewModel()
        {
            NavigateToHomeCommand = new Command(NavigateToHome);

            InitializeWalletServiceAccounts();
        }

        void InitializeWalletServiceAccounts()
        {
            if (WalletService.IsStarted)
            {
                LoadAccounts();
            }
            else
            {
                WalletService.OnStarted += _WalletService_OnStarted;
            }
        }

        void _WalletService_OnStarted(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (_Lock)
                {
                    LoadAccounts();
                }
            });
        }

        void LoadAccounts()
        {
            if (Accounts != null)
                Accounts.Clear();

            var accounts = WalletService.Wallet.Accounts;

            foreach (var account in accounts)
            {
                Accounts.Add(AccountModel.FromAccountData(account));
            }
        }

        void NavigateToHome()
        {
            if (CurrentAccount is null) return;

            WalletService.Wallet.CurrentAccount = CurrentAccount.AccountData;

            CurrentAccount = null;

            // FIXME this is probably wrong
            //_WalletService.Start();

            Shell.Current.GoToAsync("home");
        }
    }
}
