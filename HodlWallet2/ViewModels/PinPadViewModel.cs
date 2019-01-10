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

        private Color pinOne, pinTwo, pinThree, pinFour, pinFive, pinSix;

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

        public Color PinTwo
        {
            set
            {
                if (pinTwo != value)
                {
                    pinTwo = value;
                    OnPropertyChanged("PinTwo");
                }
            }
            get
            {
                return pinTwo;
            }
        }

        public Color PinThree
        {
            set
            {
                if (pinThree != value)
                {
                    pinThree = value;
                    OnPropertyChanged("PinThree");
                }
            }
            get
            {
                return pinThree;
            }
        }

        public Color PinFour
        {
            set
            {
                if (pinFour != value)
                {
                    pinFour = value;
                    OnPropertyChanged("PinFour");
                }
            }
            get
            {
                return pinFour;
            }
        }

        public Color PinFive
        {
            set
            {
                if (pinFive != value)
                {
                    pinFive = value;
                    OnPropertyChanged("PinFive");
                }
            }
            get
            {
                return pinFive;
            }
        }

        public Color PinSix
        {
            set
            {
                if (pinSix != value)
                {
                    pinSix = value;
                    OnPropertyChanged("PinSix");
                }
            }
            get
            {
                return pinSix;
            }
        }

        public PinPadViewModel()
        {
            PinOne = PinTwo = PinThree = PinFour = PinFive = PinSix = (Color)App.Current.Resources["White"];

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
                            case 2:
                                PinTwo = (Color)App.Current.Resources["SyncGradientStart"];
                                break;
                            case 3:
                                PinThree = (Color)App.Current.Resources["SyncGradientStart"];
                                break;
                            case 4:
                                PinFour = (Color)App.Current.Resources["SyncGradientStart"];
                                break;
                            case 5:
                                PinFive = (Color)App.Current.Resources["SyncGradientStart"];
                                break;
                            case 6:
                                PinSix = (Color)App.Current.Resources["SyncGradientStart"];
                                break;
                        }
                    }
                    else if (SecureStorageProvider.HasPassword() == false)
                    {
                        // SecureStorageProvider.SetPassword(Pin.ToString());
                        Pin.Clear();
                        RefreshCanExecutes();
                        PinOne = PinTwo = PinThree = PinFour = PinFive = PinSix = (Color)App.Current.Resources["White"];
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
                            PinOne = PinTwo = PinThree = PinFour = PinFive = PinSix = (Color)App.Current.Resources["White"];
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
