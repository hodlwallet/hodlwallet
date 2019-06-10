using System;
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
        private int _confirm = 0;
        private string _wordToGuess;
        private string _exercise;
        private string[] _mnemonic;
        private readonly IWalletService _walletService;
        string _Prev_Word = "";

        public string HeaderText =>
            "To make sure everything was written down correctly, please enter the following words from your backup recovery key.";
        
        private string[] confirmWords = new string[8], place = { "first", "second", "third", "fourth", 
            "fifth", "sixth", "seventh", "eighth", "ninth", "tenth", "eleventh", "twelveth" }; // Localize
        
        public MvxCommand<string> WordCommand { get; private set; }
        
        public string Exercise
        {
            get => _exercise;
            set => SetProperty(ref _exercise, value);
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
        
        public BackupRecoveryConfirmViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) : base(logProvider, navigationService)
        {
            _walletService = walletService;
            WordCommand = new MvxCommand<string>(RefreshConfirmWords);
        }

        public override void Prepare(string[] parameter)
        {
            _mnemonic = parameter;
            RefreshWords(_mnemonic);
        }

        private void RefreshConfirmWords(string arg)
        {
            int input = Convert.ToInt32(arg);

            if (confirmWords[input] == _wordToGuess && confirmWords[input] != _Prev_Word)
            {
                _confirm++;
                _Prev_Word = _wordToGuess;
            }

            RefreshWords(_mnemonic);
        }

        public async void RefreshWords(string[] mnemonic)
        {
            Random rng = new Random();

            if (_confirm < 2)
            {
                int wordIndex = rng.Next(0, mnemonic.Length - 1);
                _wordToGuess = mnemonic[wordIndex];
                Exercise = "Choose the " + place[wordIndex] + " word from your mnemonic:"; // Format and localize label.
                string language = "english"; //Implement MVVMCross
                string[] guessWords = _walletService.GenerateGuessWords(_wordToGuess, language, AMOUNT_AROUND);
                UpdateWords(guessWords);
            }
            else
            {
                Preferences.Set("MnemonicStatus", true);
                await NavigationService.Navigate<DashboardViewModel>();
                //Application.Current.MainPage = new CustomNavigationPage(new DashboardView(new DashboardViewModel()));
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
    }
}