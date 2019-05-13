using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;

using Xamarin.Forms;
using Serilog;
using HodlWallet2.Core.Interfaces;
using MvvmCross;
using HodlWallet2.Core.Services;

namespace HodlWallet2.ViewModels
{
    [Obsolete]
    public class RecoverWalletEntryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Grid _EntryGrid;

        WalletService _Wallet;
        Serilog.ILogger _Logger;

        public RecoverWalletEntryViewModel()
        {
            _Wallet = (WalletService) WalletService.Instance;
            _Logger = _Wallet.Logger;

        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        void RefreshCanExecutes()
        {
            ((Command)ReturnCommand).ChangeCanExecute();
        }

        public ICommand ReturnCommand { private set; get; }
    }
}

