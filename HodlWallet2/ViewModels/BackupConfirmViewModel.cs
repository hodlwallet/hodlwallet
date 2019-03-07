using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;
using Serilog;

using HodlWallet2.Locale;
using HodlWallet2.Utils;
using HodlWallet2.Views;

namespace HodlWallet2.ViewModels
{
    public class BackupConfirmViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        int amountAround = 7, confirm = 0;
        string wordToGuess, exercise;
        string[] confirmWords = new string[8], place = { "first", "second", "third", "fourth", 
        "fifth", "sixth", "seventh", "eighth", "ninth", "tenth", "eleventh", "twelveth" }; // Localize

        Wallet _Wallet;

        ILogger _Logger;

        public string WordOne
        {
            set
            {
                if (confirmWords[0] != value)
                {
                    confirmWords[0] = value;
                    OnPropertyChanged("WordOne");
                }
            }
            get
            {
                return confirmWords[0];
            }
        }

        public string WordTwo
        {
            set
            {
                if (confirmWords[1] != value)
                {
                    confirmWords[1] = value;
                    OnPropertyChanged("WordTwo");
                }
            }
            get
            {
                return confirmWords[1];
            }
        }

        public string WordThree
        {
            set
            {
                if (confirmWords[2] != value)
                {
                    confirmWords[2] = value;
                    OnPropertyChanged("WordThree");
                }
            }
            get
            {
                return confirmWords[2];
            }
        }

        public string WordFour
        {
            set
            {
                if (confirmWords[3] != value)
                {
                    confirmWords[3] = value;
                    OnPropertyChanged("WordFour");
                }
            }
            get
            {
                return confirmWords[3];
            }
        }

        public string WordFive
        {
            set
            {
                if (confirmWords[4] != value)
                {
                    confirmWords[4] = value;
                    OnPropertyChanged("WordFive");
                }
            }
            get
            {
                return confirmWords[4];
            }
        }

        public string WordSix
        {
            set
            {
                if (confirmWords[5] != value)
                {
                    confirmWords[5] = value;
                    OnPropertyChanged("WordSix");
                }
            }
            get
            {
                return confirmWords[5];
            }
        }

        public string WordSeven
        {
            set
            {
                if (confirmWords[6] != value)
                {
                    confirmWords[6] = value;
                    OnPropertyChanged("WordSeven");
                }
            }
            get
            {
                return confirmWords[6];
            }
        }

        public string WordEight
        {
            set
            {
                if (confirmWords[7] != value)
                {
                    confirmWords[7] = value;
                    OnPropertyChanged("WordEight");
                }
            }
            get
            {
                return confirmWords[7];
            }
        }

        public string Exercise
        {
            set
            {
                if (exercise != value)
                {
                    exercise = value;
                    OnPropertyChanged("Exercise");
                }
            }
            get
            {
                return exercise;
            }
        }

        public BackupConfirmViewModel()
        {

        }

        public BackupConfirmViewModel(string[] mnemonic)
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;
            RefreshWords(mnemonic);

            WordCommand = new Command<string>(
                execute: (string arg) =>
                {
                    int input = Convert.ToInt32(arg);

                    if (confirmWords[input] == wordToGuess)
                        confirm++;

                    RefreshWords(mnemonic);
                });
        }

        public void RefreshWords(string[] mnemonic)
        {
            Random rng = new Random();

            if (confirm < 2)
            {
                int wordIndex = rng.Next(0, mnemonic.Length - 1);
                wordToGuess = mnemonic[wordIndex];
                Exercise = "Choose the " + place[wordIndex] + " word from your mnemonic:"; // Format and localize label.
                string language = "english"; //Implement MVVMCross
                string[] guessWords = _Wallet.GenerateGuessWords(wordToGuess, language, amountAround);
                UpdateWords(guessWords);
            }
            else
            {
                SecureStorageProvider.SetMnemonicStatus("TRUE");
                Application.Current.MainPage = new CustomNavigationPage(new DashboardView(new DashboardViewModel()));
            }
        }

        private void UpdateWords(string[] guessWords)
        {
            WordOne = guessWords[0];
            WordTwo = guessWords[1];
            WordThree = guessWords[2];
            WordFour = guessWords[3];
            WordFive = guessWords[4];
            WordSix = guessWords[5];
            WordSeven = guessWords[6];
            WordEight = guessWords[7];
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand WordCommand { private set; get; }
    }
}

