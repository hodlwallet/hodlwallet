using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;

using Xamarin.Forms;
using Serilog;

namespace HodlWallet2.ViewModels
{
    public class RecoverWalletEntryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Grid _EntryGrid;

        Wallet _Wallet;
        ILogger _Logger;

        public RecoverWalletEntryViewModel()
        {
            _Wallet = Wallet.Instance;
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

