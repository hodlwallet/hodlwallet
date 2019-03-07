using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet2.Utils;

namespace HodlWallet2.ViewModels
{
    public class SecurityCenterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public INavigation _Navigation;

        string pinCheck, fingerprintCheck, mnemonicCheck;

        const string grayCheck = "Assets.circle_check_gray.svg", yellowCheck = "Assets.circle_check_yellow.svg";

        public string PinCheck
        {
            set
            {
                if (pinCheck != value)
                {
                    pinCheck = value;
                    OnPropertyChanged("PinCheck");
                }
            }
            get
            {
                return pinCheck;
            }
        }

        public SecurityCenterViewModel()
        {
            InitializeChecks();
        }

        private void InitializeChecks()
        {
            if (SecureStorageProvider.HasPassword())
            {
                PinCheck = yellowCheck;
            }
            else
            {
                PinCheck = grayCheck;
            }
            // TODO: Add next two checks.
        }

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

