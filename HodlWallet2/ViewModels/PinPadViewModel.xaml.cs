using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet2.Locale;

namespace HodlWallet2.ViewModels
{
    public partial class PinPadViewModel : ContentPage
    {
        public PinPadViewModel()
        {
            InitializeComponent();
        }

    }

    public class PinPad : INotifyPropertyChanged
    {
        private List<int> Pin = new List<int>();

        public event PropertyChangedEventHandler PropertyChanged;

        public PinPad()
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
