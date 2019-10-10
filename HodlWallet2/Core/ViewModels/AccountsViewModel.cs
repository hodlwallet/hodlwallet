using System.Collections.ObjectModel;
using System.Windows.Input;

using HodlWallet2.Core.Models;

namespace HodlWallet2.Core.ViewModels
{
    public class AccountsViewModel : BaseViewModel
    {
        public string AccountTitle => "Accounts";

        string _PriceText;
        object _CurrentAccount;

        public string PriceText
        {
            get => _PriceText;
            set => SetProperty(ref _PriceText, value);
        }

        public ObservableCollection<AccountModel> Accounts { get; } = new ObservableCollection<AccountModel>();

        public object CurrentAccount
        {
            get => _CurrentAccount;
            set => SetProperty(ref _CurrentAccount, value);
        }

        public ICommand NavigateToHomeCommand { get; }
        public ICommand AddAccountCommand { get; }

        public AccountsViewModel()
        {
            //Commands
        }

        // Methods
    }
}
