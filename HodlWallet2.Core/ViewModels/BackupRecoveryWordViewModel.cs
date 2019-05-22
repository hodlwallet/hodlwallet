using System;
using System.Collections.Generic;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace HodlWallet2.Core.ViewModels
{
    public class BackupRecoveryWordViewModel : BaseViewModel<string[]>
    {
        string[] _Mnemonic;
        int _Position;
        string _Word;
        IWalletService _WalletService;

        public string Word
        {
            get => (_Position == 0) ? _Mnemonic[PositionText] : _Word;
            private set => SetProperty(ref _Word, value);
        }

        //TODO: Localize strings
        public int PositionText
        {
            get => _Position;
            private set => SetProperty(ref _Position, value);
        }
        public string NextText => "Next Word";
        public string PreviousText => "Previous";
        public string HeaderText => "Write down each word in order and store it in a safe place.";
        public string TitleText => "Backup Recovery Key";
        public IMvxCommand NextWordCommand { get; private set; }
        public IMvxCommand PreviousWordCommand { get; private set; }

        public BackupRecoveryWordViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IWalletService walletService) : base(logProvider, navigationService)
        {
            _WalletService = walletService;

            NextWordCommand = new MvxCommand(NextWord_Action);
            PreviousWordCommand = new MvxCommand(PreviousWord_Action);
        }

        private void PreviousWord_Action()
        {
            if (PositionText > 0)
            {
                PositionText--;
                Word = _Mnemonic[PositionText];
                //TODO: Log activity here.
            }
        }

        private void NextWord_Action()
        {

            if (PositionText < 11)
            {
                PositionText++;
                Word = _Mnemonic[PositionText];
                //TODO: Log activity here.
            }
            else
            {
                //TODO: Log activity here.
                NavigationService.Navigate<BackupRecoveryConfirmViewModel, string[]>(_Mnemonic);
            }
        }

        public override void Prepare()
        {
            string rawMnemonic = _WalletService.NewMnemonic(GetWordListLanguage(), GetWordCount());

            _WalletService.Logger.Information($"Newly generated mnemonic is: {rawMnemonic}");

            _Mnemonic = rawMnemonic.Split(' ');
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