//
// BackupRecoveryWordViewModel.cs
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
using System.Collections.Generic;
using System.Windows.Input;

using Xamarin.Forms;

using Liviano.Exceptions;

using HodlWallet.Core.Models;
using HodlWallet.Core.Services;
using HodlWallet.UI.Converters;
using HodlWallet.Core.Utils;

namespace HodlWallet.Core.ViewModels
{
    public class BackupRecoveryWordViewModel : BaseViewModel
    {
        string[] _Mnemonic;

        public ICommand NextCommand { get; }

        public List<BackupWordModel> Words { get; set; }

        public List<BackupWordModel> ShuffledWordsList { get; set; } = new();

        public BackupRecoveryWordViewModel()
        {
            NextCommand = new Command(NextWord);

            if (DesignMode.IsDesignModeEnabled) return;

            InitMnemonic();
        }

        private void NextWord()
        {
            //MessagingCenter.Send(this, "NavigateToBackupRecoveryConfirmView", _Mnemonic);
            MessagingCenter.Send(this, "MnemonicListMessage", Words);
        }

        void InitMnemonic()
        {
            string rawMnemonic = GetMnemonic();

            WalletService.Logger.Information($"Mnemonic is: {rawMnemonic}");

            _Mnemonic = rawMnemonic.Split(' ');
            Words = MnemonicArrayToList.GenerateWordsList(_Mnemonic);
            GenerateShuffledMnemonics();
        }

        private string GetMnemonic()
        {
            if (!SecureStorageService.HasMnemonic())
                throw new WalletException("This wallet doesn't have a mnemonic, we cannot do anything without that one");

            return SecureStorageService.GetMnemonic();
        }

        public void GenerateShuffledMnemonics()
        {
            ShuffledWordsList.AddRange(Words);
            ShuffledWordsList.Shuffle();


            Console.WriteLine("Lista A");
            foreach (var item in Words)
            {
                Console.WriteLine($"{item.WordIndex} --> {item.Word}");
            }
            Console.WriteLine("Lista B");
            foreach (var item in ShuffledWordsList)
            {
                Console.WriteLine($"{item.WordIndex} --> {item.Word}");
            }
        }
    }
}