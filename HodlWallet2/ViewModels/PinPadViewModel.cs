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
    public enum ViewType { Setup, Re_enter, Recover, Re_enter_recover, Update, Update_setup, Re_enter_update }

    public class PinPadViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        List<int> Pin = new List<int>();

        Color pinOne, pinTwo, pinThree, pinFour, pinFive, pinSix;

        string title, header, warning;

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

        public string PinTitle
        {
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged("PinTitle");
                }
            }
            get
            {
                return title;
            }
        }

        public string Header
        {
            set
            {
                if (header != value)
                {
                    header = value;
                    OnPropertyChanged("Header");
                }
            }
            get
            {
                return header;
            }
        }

        public string Warning
        {
            set
            {
                if (warning != value)
                {
                    warning = value;
                    OnPropertyChanged("Warning");
                }
            }
            get
            {
                return warning;
            }
        }

        public PinPadViewModel()
        {

        }

        public PinPadViewModel(ViewType viewType)
        {
            PinOne = PinTwo = PinThree = PinFour = PinFive = PinSix = (Color)App.Current.Resources["White"];

            switch (viewType)
            {
                case ViewType.Setup:
                case ViewType.Update_setup:
                    PinTitle = LocaleResources.Pin_setTitle;
                    Header = LocaleResources.Pin_header;
                    Warning = LocaleResources.Pin_warning;
                    break;

                case ViewType.Re_enter:
                case ViewType.Re_enter_recover:
                    PinTitle = LocaleResources.Pin_redoTitle;
                    Header = LocaleResources.Pin_header;
                    Warning = "";
                    break;

                default:
                    PinTitle = LocaleResources.Pin_setTitle;
                    Header = LocaleResources.Pin_header;
                    Warning = LocaleResources.Pin_warning;
                    break;
            }

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
                            if (viewType == ViewType.Setup || viewType == ViewType.Recover || viewType == ViewType.Update_setup)
                            {
                                SecureStorageProvider.SetPassword(string.Join("", Pin.ToArray()));
                                Pin.Clear();
                                //_Navigation.PushAsync(new PinPadView(new PinPadViewModel(ViewType.Re_enter)));
                                PinOne = PinTwo = PinThree = PinFour = PinFive = PinSix = (Color)App.Current.Resources["White"];
                                RefreshCanExecutes();

                                switch(viewType)
                                {
                                    case ViewType.Setup:
                                        //_Navigation.PushAsync(new PinPadView(new PinPadViewModel(ViewType.Re_enter)));
                                        break;
                                    case ViewType.Recover:
                                        //_Navigation.PushAsync(new PinPadView(new PinPadViewModel(ViewType.Re_enter_recover)));
                                        break;
                                    case ViewType.Update_setup:
                                        //_Navigation.PushAsync(new PinPadView(new PinPadViewModel(ViewType.Re_enter_update)));
                                        break;
                                }
                            }
                            else if (viewType == ViewType.Update || viewType == ViewType.Re_enter || 
                                     viewType == ViewType.Re_enter_recover || viewType == ViewType.Re_enter_update)
                            {
                                if (SecureStorageProvider.GetPassword() == string.Join("", Pin.ToArray()))
                                {
                                    Pin.Clear();
                                    RefreshCanExecutes();

                                    switch (viewType)
                                    {
                                        case ViewType.Re_enter:
                                            //Application.Current.MainPage = new CustomNavigationPage(new BackupView());
                                            break;
                                        case ViewType.Re_enter_recover:
                                            //Application.Current.MainPage = new CustomNavigationPage(new DashboardView(new DashboardViewModel()));
                                            break;
                                        case ViewType.Re_enter_update:
                                            //_Navigation.PopToRootAsync();
                                            break;
                                        case ViewType.Update:
                                            //_Navigation.PushAsync(new PinPadView(new PinPadViewModel(ViewType.Update_setup)));
                                            break;
                                    }
                                }
                                else
                                {
                                    Pin.Clear();
                                    RefreshCanExecutes();
                                    _Navigation.PopAsync();
                                }
                            }
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
