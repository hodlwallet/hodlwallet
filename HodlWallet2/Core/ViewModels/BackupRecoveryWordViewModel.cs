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
using System.Threading.Tasks;
using System.Windows.Input;
using HodlWallet2.Core.Services;
using HodlWallet2.UI.Views;
using Liviano.Exceptions;
using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class BackupRecoveryWordViewModel : BaseViewModel
    {
        string[] _Mnemonic;
        int _Position;
        string _Word;

        public string Word
        {
            get => (_Position == 0) ? _Mnemonic[PositionText] : _Word;
            private set => SetProperty(ref _Word, value);
        }

        public int PositionText
        {
            get => _Position;
            private set => SetProperty(ref _Position, value);
        }
        public string NextText => "Next Word";
        public string PreviousText => "Previous";
        public string HeaderText => "Write down each word in order and store it in a safe place.";
        public string TitleText => "Backup Recovery Key";
        public ICommand NextWordCommand { get; }
        public ICommand PreviousWordCommand { get; }

        public BackupRecoveryWordViewModel()
        {
            NextWordCommand = new Command(NextWord);
            PreviousWordCommand = new Command(PreviousWord);

            if (DesignMode.IsDesignModeEnabled) return;

            InitMnemonic();
        }

        private void PreviousWord()
        {
            if (PositionText > 0)
            {
                PositionText--;
                Word = _Mnemonic[PositionText];
            }
        }

        private void NextWord()
        {
            if (PositionText < 11)
            {
                PositionText++;
                Word = _Mnemonic[PositionText];

                return;
            }

            MessagingCenter.Send(this, "NavigateToBackupRecoveryConfirmView", _Mnemonic);
        }

        void InitMnemonic()
        {
            string rawMnemonic = GetMnemonic();

            _WalletService.Logger.Information($"Mnemonic is: {rawMnemonic}");

            _Mnemonic = rawMnemonic.Split(' ');
        }

        private string GetMnemonic()
        {
            if (!SecureStorageService.HasMnemonic())
                throw new WalletException("This wallet doesn't have a mnemonic, we cannot do anything without that one");

            return SecureStorageService.GetMnemonic();
        }
    }
}