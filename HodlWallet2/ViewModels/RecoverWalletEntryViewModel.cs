using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;

namespace HodlWallet2.ViewModels
{
    public class RecoverWalletEntryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RecoverWalletEntryViewModel()
        {

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
            ((Command)BackspaceCommand).ChangeCanExecute();
            ((Command)DigitCommand).ChangeCanExecute();
        }

        public ICommand BackspaceCommand { private set; get; }

        public ICommand DigitCommand { private set; get; }
    }
}

