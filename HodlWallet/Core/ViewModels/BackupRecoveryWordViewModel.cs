﻿//
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
using System.Windows.Input;

using Xamarin.Forms;

using Liviano.Exceptions;

using HodlWallet.Core.Services;
using HodlWallet.Core.Models;
using System.Collections.Generic;


namespace HodlWallet.Core.ViewModels
{
    public class BackupRecoveryWordViewModel : BaseViewModel
    {
        string[] mnemonic;

        public ICommand NextCommand { get; }

        public IList<BackupWordModel> Words { get; set; }

        public BackupRecoveryWordViewModel()
        {
            NextCommand = new Command(NextWord);

            if (DesignMode.IsDesignModeEnabled) return;

            InitMnemonic();
        }

        private void NextWord()
        {
            MessagingCenter.Send(this, "NavigateToBackupRecoveryConfirmView", mnemonic);
        }

        void InitMnemonic()
        {
            string rawMnemonic = GetMnemonic();

            WalletService.Logger.Information($"Mnemonic is: {rawMnemonic}");

            mnemonic = rawMnemonic.Split(' ');
            GenerateWordsList();
        }

        private string GetMnemonic()
        {
            if (!SecureStorageService.HasMnemonic())
                throw new WalletException("This wallet doesn't have a mnemonic, we cannot do anything without that one");

            return SecureStorageService.GetMnemonic();
        }

        private void GenerateWordsList()
        {
            int index = 0;
            Words = new List<BackupWordModel>();
            foreach (var word in mnemonic)
            {
                index++;
                Words.Add(new BackupWordModel() { Word = word, WordIndex=index.ToString() });
            }
            
            //Temp code to print 12 extra-mnemonics
            //for (int i = 0; i < _Mnemonic.Length; i++)
            //{
            //    index++;
            //    Words.Add(new BackupWordModel() { Word = $"{_Mnemonic[i]}{index}" , WordIndex = index.ToString() });
            //}
        }
    }
}