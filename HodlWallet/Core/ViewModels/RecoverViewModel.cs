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
using System;
using System.Diagnostics;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet.Core.Services;

namespace HodlWallet.Core.ViewModels
{
    public class RecoverViewModel : BaseViewModel
    {
        public ICommand OnRecoverEntryCompleted { get; }

        string wordOne;
        public string WordOne
        {
            get => wordOne;
            set => SetProperty(ref wordOne, value);
        }

        string wordTwo;
        public string WordTwo
        {
            get => wordTwo;
            set => SetProperty(ref wordTwo, value);
        }

        string wordThree;
        public string WordThree
        {
            get => wordThree;
            set => SetProperty(ref wordThree, value);
        }

        string wordFour;
        public string WordFour
        {
            get => wordFour;
            set => SetProperty(ref wordFour, value);
        }

        string wordFive;
        public string WordFive
        {
            get => wordFive;
            set => SetProperty(ref wordFive, value);
        }

        string wordSix;
        public string WordSix
        {
            get => wordSix;
            set => SetProperty(ref wordSix, value);
        }

        string wordSeven;
        public string WordSeven
        {
            get => wordSeven;
            set => SetProperty(ref wordSeven, value);
        }

        string wordEight;
        public string WordEight
        {
            get => wordEight;
            set => SetProperty(ref wordEight, value);
        }

        string wordNine;
        public string WordNine
        {
            get => wordNine;
            set => SetProperty(ref wordNine, value);
        }

        string wordTen;
        public string WordTen
        {
            get => wordTen;
            set => SetProperty(ref wordTen, value);
        }

        string wordEleven;
        public string WordEleven
        {
            get => wordEleven;
            set => SetProperty(ref wordEleven, value);
        }

        string wordTwelve;
        public string WordTwelve
        {
            get => wordTwelve;
            set => SetProperty(ref wordTwelve, value);
        }

        public RecoverViewModel()
        {
            OnRecoverEntryCompleted = new Command(TrySaveMnemonic);
        }

        void TrySaveMnemonic()
        {
            if (!MnemonicInWordList()) return;

            var mnemonic = GetMnemonic();

            if (!CheckMnemonicHasValidChecksum(mnemonic)) return;

            SecureStorageService.SetMnemonic(mnemonic);

            MessagingCenter.Send(this, "InitiateAppShell");
        }

        bool CheckWordInWordlist(string word, string wordlist = "english")
        {
            if (!string.IsNullOrEmpty(word) && Services.WalletService.IsWordInWordlist(word.ToLower(), wordlist))
                return true;

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
            if (Services.WalletService.IsVerifyChecksum(mnemonic, wordlist)) return true;

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