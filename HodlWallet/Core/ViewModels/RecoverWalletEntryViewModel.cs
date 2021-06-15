//
// RecoverWalletEntryViewModel.cs
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
using System.Diagnostics;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet.Core.Services;
using System;
using System.Runtime.CompilerServices;

namespace HodlWallet.Core.ViewModels
{
    public class RecoverWalletEntryViewModel : BaseViewModel
    {
        public ICommand OnRecoverEntryCompleted { get; }

        string _WordOne;
        public string WordOne
        {
            get => _WordOne;
            set => SetProperty(ref _WordOne, value);
        }

        string _WordTwo;
        public string WordTwo
        {
            get => _WordTwo;
            set => SetProperty(ref _WordTwo, value);
        }

        string _WordThree;
        public string WordThree
        {
            get => _WordThree;
            set => SetProperty(ref _WordThree, value);
        }

        string _WordFour;
        public string WordFour
        {
            get => _WordFour;
            set => SetProperty(ref _WordFour, value);
        }

        string _WordFive;
        public string WordFive
        {
            get => _WordFive;
            set => SetProperty(ref _WordFive, value);
        }

        string _WordSix;
        public string WordSix
        {
            get => _WordSix;
            set => SetProperty(ref _WordSix, value);
        }

        string _WordSeven;
        public string WordSeven
        {
            get => _WordSeven;
            set => SetProperty(ref _WordSeven, value);
        }

        string _WordEight;
        public string WordEight
        {
            get => _WordEight;
            set => SetProperty(ref _WordEight, value);
        }

        string _WordNine;
        public string WordNine
        {
            get => _WordNine;
            set => SetProperty(ref _WordNine, value);
        }

        string _WordTen;
        public string WordTen
        {
            get => _WordTen;
            set => SetProperty(ref _WordTen, value);
        }

        string _WordEleven;
        public string WordEleven
        {
            get => _WordEleven;
            set => SetProperty(ref _WordEleven, value);
        }

        string _WordTwelve;
        public string WordTwelve
        {
            get => _WordTwelve;
            set => SetProperty(ref _WordTwelve, value);
        }

        public RecoverWalletEntryViewModel()
        {
            OnRecoverEntryCompleted = new Command(TrySaveMnemonic);
        }

        void TrySaveMnemonic()
        {
            if (!MnemonicInWordList()) return;

            string mnemonic = GetMnemonic();

            if (!CheckMnemonicHasValidChecksum(mnemonic)) return;

            SecureStorageService.SetMnemonic(mnemonic);
            SecureStorageService.SetSeedBirthday(new DateTimeOffset(DateTime.UtcNow));

            WalletService.StartWalletWithWalletId();

            MessagingCenter.Send(this, "NavigateToRootView");
        }

        bool CheckWordInWordlist(string word, string wordlist = "english")
        {
            if (!string.IsNullOrEmpty(word) && Services.WalletService.IsWordInWordlist(word.ToLower(), wordlist) == true)
            {
                return true;
            }

            Debug.WriteLine($"User input not found in wordlist: {word}");

            DisplayRecoverAlert();

            return false;
        }

        bool MnemonicInWordList()
        {
            if (!CheckWordInWordlist(WordOne)) return false;
            if (!CheckWordInWordlist(WordTwo)) return false;
            if (!CheckWordInWordlist(WordThree)) return false;
            if (!CheckWordInWordlist(WordFour)) return false;
            if (!CheckWordInWordlist(WordFive)) return false;
            if (!CheckWordInWordlist(WordSix)) return false;
            if (!CheckWordInWordlist(WordSeven)) return false;
            if (!CheckWordInWordlist(WordEight)) return false;
            if (!CheckWordInWordlist(WordNine)) return false;
            if (!CheckWordInWordlist(WordTen)) return false;
            if (!CheckWordInWordlist(WordEleven)) return false;
            if (!CheckWordInWordlist(WordTwelve)) return false;

            return true;
        }

        bool CheckMnemonicHasValidChecksum(string mnemonic, string wordlist = "english")
        {
            if (Services.WalletService.IsVerifyChecksum(mnemonic, wordlist) == true) return true;

            Debug.WriteLine($"Mnemonic returned invalid checksum: {mnemonic}");

            DisplayRecoverAlert();

            return false;
        }

        string GetMnemonic()
        {
            return string.Join(" ", new string[]
            {
                WordOne, WordTwo, WordThree,
                WordFour, WordFive, WordSix,
                WordSeven, WordEight, WordNine,
                WordTen, WordEleven, WordTwelve
            }).ToLower();
        }

        void DisplayRecoverAlert()
        {
            MessagingCenter.Send(this, "RecoverySeedError");
        }
    }
}