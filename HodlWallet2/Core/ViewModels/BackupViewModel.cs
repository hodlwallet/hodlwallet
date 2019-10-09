﻿//
// BackupViewModel.cs
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
using System.Threading.Tasks;
using System.Windows.Input;

using HodlWallet2.Core.Services;

namespace HodlWallet2.Core.ViewModels
{
    public class BackupViewModel : BaseViewModel
    {
        //TODO: Add localization strings
        public string BackupTitle => "Backup Recovery Key";

        public string HeaderText =>
            "Your backup recovery key is the only way to restore your wallet if your phone is lost, stolen, broken or upgraded.";

        public string SubheaderText =>
            "We will show you a list of words to write down on a piece of paper and keep safe.";
        public string ButtonText => "Write Down Backup Recovery Key";

        public ICommand WriteDownWordsCommand { get; }

        public BackupViewModel()
        {
            CreateSeedIfNeeded();
        }

        void CreateSeedIfNeeded()
        {
            if (SecureStorageService.HasMnemonic())
            {
                if (!_WalletService.IsStarted)
                    Task.Run(_WalletService.StartWalletWithWalletId);

                return;
            }

            string rawMnemonic = WalletService.GetNewMnemonic(GetWordListLanguage(), GetWordCount());

            _WalletService.Logger.Information($"Wallet generated a new mnemonic, mnemonic: {rawMnemonic}");

            SecureStorageService.SetMnemonic(rawMnemonic);
            SecureStorageService.SetSeedBirthday(new DateTimeOffset(DateTime.UtcNow));

            _WalletService.Logger.Information("Saved mnemonic to secure storage.");

            // After this we should be able to start the wallet if it's not started since we have a mnemonic
            if (!_WalletService.IsStarted)
                Task.Run(_WalletService.StartWalletWithWalletId);
        }

        private string GetWordListLanguage()
        {
            // TODO This should read from the user's language.
            string language = "english";

            _WalletService.Logger.Information($"Wordlist is on {language}");

            return language;
        }

        private int GetWordCount()
        {
            // TODO This should read from the user's preference eventually.
            int wordCount = 12;

            _WalletService.Logger.Information($"Word count is {wordCount}");

            return wordCount;
        }
    }
}
