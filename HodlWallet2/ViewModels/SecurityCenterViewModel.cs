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

        public string FingerprintCheck
        {
            set
            {
                if (fingerprintCheck != value)
                {
                    fingerprintCheck = value;
                    OnPropertyChanged("FingerprintCheck");
                }
            }
            get
            {
                return fingerprintCheck;
            }
        }

        public string MnemonicCheck
        {
            set
            {
                if (mnemonicCheck != value)
                {
                    mnemonicCheck = value;
                    OnPropertyChanged("MnemonicCheck");
                }
            }
            get
            {
                return mnemonicCheck;
            }
        }

        public SecurityCenterViewModel()
        {
            InitializeChecks();
        }

        public void InitializeChecks()
        {
            if (SecureStorageProvider.HasPassword())
            {
                PinCheck = yellowCheck;
            }
            else
            {
                throw new ArgumentException("Pin cannot be null after setup!");
            }

            if (SecureStorageProvider.HasFingerprintStatus())
            {
                FingerprintCheck = SecureStorageProvider.GetFingerprintStatus() == "1" ? yellowCheck : grayCheck;
            }
            else
            {
                throw new ArgumentException("Fingerprint status cannot be null after setup!");
            }

            if (SecureStorageProvider.HasMnemonicStatus())
            {
                MnemonicCheck = SecureStorageProvider.GetMnemonicStatus() == "1" ? yellowCheck : grayCheck;
            }
            else
            {
                throw new ArgumentException("Mnemonic status cannot be null after setup!");
            }
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

