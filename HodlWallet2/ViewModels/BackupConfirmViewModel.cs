using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;

namespace HodlWallet2.ViewModels
{
    public class BackupConfirmViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string wordOne, wordTwo, wordThree, wordFour, wordFive, wordSix, wordSeven, wordEight;

        public string WordOne
        {
            set
            {
                if (wordOne != value)
                {
                    wordOne = value;
                    OnPropertyChanged("WordOne");
                }
            }
            get
            {
                return wordOne;
            }
        }

        public string WordTwo
        {
            set
            {
                if (wordTwo != value)
                {
                    wordTwo = value;
                    OnPropertyChanged("WordTwo");
                }
            }
            get
            {
                return wordTwo;
            }
        }

        public string WordThree
        {
            set
            {
                if (wordThree != value)
                {
                    wordThree = value;
                    OnPropertyChanged("WordThree");
                }
            }
            get
            {
                return wordThree;
            }
        }

        public string WordFour
        {
            set
            {
                if (wordFour != value)
                {
                    wordFour = value;
                    OnPropertyChanged("WordFour");
                }
            }
            get
            {
                return wordFour;
            }
        }

        public string WordFive
        {
            set
            {
                if (wordFive != value)
                {
                    wordFive = value;
                    OnPropertyChanged("WordFive");
                }
            }
            get
            {
                return wordFive;
            }
        }

        public string WordSix
        {
            set
            {
                if (wordSix != value)
                {
                    wordSix = value;
                    OnPropertyChanged("WordSix");
                }
            }
            get
            {
                return wordSix;
            }
        }

        public string WordSeven
        {
            set
            {
                if (wordSeven != value)
                {
                    wordSeven = value;
                    OnPropertyChanged("WordSeven");
                }
            }
            get
            {
                return wordSeven;
            }
        }

        public string WordEight
        {
            set
            {
                if (wordEight != value)
                {
                    wordEight = value;
                    OnPropertyChanged("WordEight");
                }
            }
            get
            {
                return wordEight;
            }
        }

        public BackupConfirmViewModel()
        {

        }

        public BackupConfirmViewModel(string[] mnemonic)
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
    }
}

