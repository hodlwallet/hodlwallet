using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet2.Utils;
using HodlWallet2.Locale;
using HodlWallet2.Views;

namespace HodlWallet2.ViewModels
{
    [Obsolete]
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        List<int> Pin = new List<int>();

        Color pinOne, pinTwo, pinThree, pinFour, pinFive, pinSix;

        public INavigation _Navigation;

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

        public LoginViewModel()
        {
            PinOne = PinTwo = PinThree = PinFour = PinFive = PinSix = (Color)App.Current.Resources["White"];

            BackspaceCommand = new Command(
                execute: () =>
                {
                    if (Pin.Count - 1 >= 0)
                    {
                        switch (Pin.Count)
                        {
                            case 1:
                                PinOne = (Color)App.Current.Resources["White"];
                                break;
                            case 2:
                                PinTwo = (Color)App.Current.Resources["White"];
                                break;
                            case 3:
                                PinThree = (Color)App.Current.Resources["White"];
                                break;
                            case 4:
                                PinFour = (Color)App.Current.Resources["White"];
                                break;
                            case 5:
                                PinFive = (Color)App.Current.Resources["White"];
                                break;
                            case 6:
                                PinSix = (Color)App.Current.Resources["White"];
                                break;
                        }
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

                        RefreshCanExecutes();

                        if (Pin.Count == 6)
                        {
                            PinOne = PinTwo = PinThree = PinFour = PinFive = PinSix = (Color)App.Current.Resources["White"];

                            if (SecureStorageProvider.HasPassword() == false)
                            {
                                // TODO: Throw exception
                                Pin.Clear();
                                return;
                            }
                            if (SecureStorageProvider.GetPassword() == string.Join("", Pin.ToArray()))
                            {
                                Pin.Clear();
                                RefreshCanExecutes();
                                //Application.Current.MainPage = new CustomNavigationPage(new DashboardView(new DashboardViewModel()));
                                return;
                            }
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

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

