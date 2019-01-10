using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet2.Utils;

namespace HodlWallet2.ViewModels
{
    public class PinPadViewModel : INotifyPropertyChanged
    {
        private List<int> Pin = new List<int>();

        public event PropertyChangedEventHandler PropertyChanged;

        private Color pinOne;

        public Color PinOne
        {
            set
            {
                if (pinOne != value)
                {
                    pinOne = value;
                    OnPropertyChanged("PinOne");
                }
            }
            get
            {
                return pinOne;
            }
        }

        public PinPadViewModel()
        {
            BackspaceCommand = new Command(
                execute: () =>
                {
                    if (Pin.Count - 1 >= 0)
                    {
                        Pin.RemoveAt(Pin.Count - 1);
                        RefreshCanExecutes();
                    }
                });

            DigitCommand = new Command<string>(
                execute: (string arg) =>
                {
                    if (Pin.Count < 6)
                    {
                        Pin.Add(int.Parse(arg));
                        RefreshCanExecutes();
                        switch (Pin.Count)
                        {
                            case 1:
                                PinOne = (Color)App.Current.Resources["SyncGradientStart"];
                                break;
                        }
                    }
                    else if (SecureStorageProvider.HasPassword() == false)
                    {
                        SecureStorageProvider.SetPassword(Pin.ToString());
                        Pin.Clear();
                        RefreshCanExecutes();
                        // Next Page
                    }
                    else if (SecureStorageProvider.HasPassword() == true)
                    {
                        if (SecureStorageProvider.GetPassword() == Pin.ToString())
                        {
                            Pin.Clear();
                            RefreshCanExecutes();
                            // Next Page
                        }
                        else
                        {
                            Pin.Clear();
                            RefreshCanExecutes();
                            PinOne = (Color)App.Current.Resources["White"];
                        }
                    }
                });
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
