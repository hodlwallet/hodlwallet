﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet2.Core.Models;
using HodlWallet2.UI.Views;

namespace HodlWallet2.Core.ViewModels
{
    public class AccountsViewModel : BaseViewModel
    {
        public string AccountTitle => "Accounts";

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
            if (_WalletService.IsStarted)
            {
                LoadAccounts();
            }
            else
            {
                _WalletService.OnStarted += _WalletService_OnStarted;
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

            var accounts = _WalletService.Wallet.Accounts;

            foreach(var account in accounts)
            {
                Accounts.Add(AccountModel.FromAccountData(account));
            }
        }

        void NavigateToHome()
        {
            if (CurrentAccount == null) return;

            _WalletService.Wallet.CurrentAccount = CurrentAccount.AccountData;

            CurrentAccount = null;

            _WalletService.Start();

            MessagingCenter.Send(this, "ChangeCurrentPageTo", RootView.Tabs.Home);
        }
    }
}
