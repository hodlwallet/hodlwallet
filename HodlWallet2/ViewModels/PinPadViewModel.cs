using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;

namespace HodlWallet2.ViewModels
{
    public class PinPadViewModel : INotifyPropertyChanged
    {
        private List<int> Pin = new List<int>();

        public event PropertyChangedEventHandler PropertyChanged;

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
                    }
                    else
                    {
                        // Store Pin
                        Pin.Clear();
                        RefreshCanExecutes();
                    }
                });
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