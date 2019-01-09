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

        public PinPadViewModel(bool didSet = false)
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
                    else if (didSet == false)
                    {
                        if (SecureStorageProvider.HasPassword())
                        {
                            throw new System.ArgumentException("Pin should be null.");
                        }
                        SecureStorageProvider.SetPassword(Pin.ToString());
                        Pin.Clear();
                        RefreshCanExecutes();
                    }
                    else if (didSet == true)
                    {
                        if (!SecureStorageProvider.HasPassword())
                        {
                            throw new System.ArgumentNullException();
                        }
                        if (SecureStorageProvider.GetPassword() == Pin.ToString())
                        {
                            // Set Binding and Navigation
                            Pin.Clear();
                            RefreshCanExecutes();
                        }
                        else
                        {
                            Pin.Clear();
                            RefreshCanExecutes();
                        }
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
