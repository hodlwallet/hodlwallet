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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet.Core.Models;
using HodlWallet.UI.Converters;
using HodlWallet.Core.Utils;

namespace HodlWallet.Core.ViewModels
{
    public class BackupRecoveryConfirmViewModel : BaseViewModel
    {
        public List<BackupWordModel> ShuffledWordsList { get; set; } = new();

        public List<BackupWordModel> Words { get; set; } = new();

        public ICommand NextCommand { get; }

        public BackupRecoveryConfirmViewModel()
        {
            NextCommand = new Command(NextWord);
            GenerateShuffledMnemonics();
        }

        void GenerateShuffledMnemonics()
        {
            //Bring the Mnemonic list from SecureStorage as a List and copy it to a new list
            Words = MnemonicStringToList.GenerateWordsList();
            ShuffledWordsList.AddRange(Words);

            //Suffle the copy of the original list.
            ShuffledWordsList.Shuffle();
        }

        private void NextWord()
        {
            Debug.WriteLine("============> ALL DONE!!!");
            MessagingCenter.Send(this, "NavigateToExtraBackupView", ShuffledWordsList);
        }
    }
}