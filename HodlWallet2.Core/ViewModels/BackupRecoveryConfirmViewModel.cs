using System;
using System.Linq;
using System.Threading.Tasks;
using HodlWallet2.Core.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using Xamarin.Essentials;

namespace HodlWallet2.Core.ViewModels
{
    public class BackupRecoveryConfirmViewModel : BaseViewModel<string[]>
    {
        private const int AMOUNT_AROUND = 7;
        private int _Confirm = 0;
        private string _WordToGuess;
        private string _Exercise;
        private string[] _Mnemonic;
        private readonly IWalletService _WalletService;
        int _PrevIndex;
        bool _WarningVisible;

        public string HeaderText =>
            "To make sure everything was written down correctly, please enter the following words from your backup recovery key.";

        public string WarningText => "That word is not in your mnemonic.";
        
        private string[] confirmWords = new string[8], place = { "first", "second", "third", "fourth", 
            "fifth", "sixth", "seventh", "eighth", "ninth", "tenth", "eleventh", "twelveth" }; // Localize
        
        public MvxCommand<string> WordCommand { get; }
        
        public string Exercise
        {
            get => _Exercise;
            set => SetProperty(ref _Exercise, value);
        }

        public bool WarningVisible
        {
            get => _WarningVisible;
            set => SetProperty(ref _WarningVisible, value);
        }
        
        public string WordOne
        {
            set
            {
                if (confirmWords[0] != value)
                {
                    SetProperty(ref confirmWords[0], value);
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
                    SetProperty(ref confirmWords[1], value);
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
                    SetProperty(ref confirmWords[2], value);
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
                    SetProperty(ref confirmWords[3], value);
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
                    SetProperty(ref confirmWords[4], value);
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
                    SetProperty(ref confirmWords[5], value);
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
                    SetProperty(ref confirmWords[6], value);
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
                    SetProperty(ref confirmWords[7], value);
                }
            }
            get
            {
                return confirmWords[7];
            }
        }

        bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }
        
        public BackupRecoveryConfirmViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) : base(logProvider, navigationService)
        {
            _WalletService = walletService;
            WordCommand = new MvxCommand<string>(RefreshConfirmWords);
        }

        public override void Prepare(string[] parameter)
        {
            _Mnemonic = parameter;
            _PrevIndex = _Mnemonic.Length;
            RefreshWords(_Mnemonic);
        }

        private void RefreshConfirmWords(string arg)
        {
            int input = Convert.ToInt32(arg);

            if (confirmWords[input] == _WordToGuess)
            {
                if (WarningVisible)
                    WarningVisible = false;
                _Confirm++;
                _PrevIndex = input;
            }
            else
            {
                _Confirm = 0;
                WarningVisible = true;
                _PrevIndex = _Mnemonic.Length;
            }

            RefreshWords(_Mnemonic);
        }

        public async void RefreshWords(string[] mnemonic)
        {
            Random rng = new Random();

            if (_Confirm < 2)
            {
                var rangeArray = Enumerable.Range(0, mnemonic.Length - 1).Where(a => a != _PrevIndex).ToArray();
                int wordIndex = rangeArray[rng.Next(rangeArray.Length)];
                _WordToGuess = mnemonic[wordIndex];
                Exercise = "Choose the " + place[wordIndex] + " word from your mnemonic:"; // Format and localize label.
                string[] guessWords = _WalletService.GenerateGuessWords(_WordToGuess, "english", AMOUNT_AROUND);
                UpdateWords(guessWords);
            }
            else
            {
                Preferences.Set("MnemonicStatus", true);

                IsLoading = true;

                await Task.Delay(1);

                await NavigationService.Navigate<RootViewModel, int>((int)RootViewModel.Tabs.Home);
            }
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();

            IsLoading = false;
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
    }
}