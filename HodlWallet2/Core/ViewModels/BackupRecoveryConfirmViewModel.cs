//
// BackupRecoveryConfirmViewModel.cs
//
// Copyright (c) 2019 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;

using HodlWallet.Core.Services;
using HodlWallet.UI.Locale;

namespace HodlWallet.Core.ViewModels
{
    public class BackupRecoveryConfirmViewModel : BaseViewModel
    {
        private const int AMOUNT_AROUND = 7;
        private int _Confirm = 0;
        private string _WordToGuess;
        private string _Exercise;
        private string[] _Mnemonic;
        int _PrevIndex;
        bool _WarningVisible;

        private string[] confirmWords = new string[8], place = { "first", "second", "third", "fourth",
            "fifth", "sixth", "seventh", "eighth", "ninth", "tenth", "eleventh", "twelveth" }; // Localize

        public ICommand WordCommand { get; }

        public string[] Mnemonic
        {
            get => _Mnemonic;
            set
            {
                SetProperty(ref _Mnemonic, value);

                _PrevIndex = _Mnemonic.Length;
                _ = RefreshWords(_Mnemonic);
            }
        }

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

        public BackupRecoveryConfirmViewModel()
        {
            WordCommand = new Command<string>(RefreshConfirmWords);
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

            _ = RefreshWords(_Mnemonic);
        }

        public async Task RefreshWords(string[] mnemonic)
        {
            Random rng = new Random();

            if (_Confirm < 2)
            {
                var rangeArray = Enumerable.Range(0, mnemonic.Length - 1).Where(a => a != _PrevIndex).ToArray();
                int wordIndex = rangeArray[rng.Next(rangeArray.Length)];
                _WordToGuess = mnemonic[wordIndex];

                Exercise = string.Format(LocaleResources.BackupConfirm_exercise, place[wordIndex]);
                string[] guessWords = WalletService.GenerateGuessWords(_WordToGuess, _WalletService.GetWordListLanguage(), AMOUNT_AROUND);

                UpdateWords(guessWords);
            }
            else
            {
                Preferences.Set("MnemonicStatus", true);

                IsLoading = true;

                await Task.Delay(1);

                MessagingCenter.Send(this, "NavigateToRootView");
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