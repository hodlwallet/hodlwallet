﻿using System;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet2.Core.Services;

namespace HodlWallet2.Core.ViewModels
{
    public class AccountSelectionViewModel : BaseViewModel
    {
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
                            _WalletService.Wallet.AddAccount(arg, Name ?? "Bitcoin Account");
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    _WalletService.Start();
                });
        }
    }
}
