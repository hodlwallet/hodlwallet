using System;
using System.Collections.Generic;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.Utils;
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
            }
            else
            {
                //TODO: Log activity here.
                NavigationService.Navigate<BackupRecoveryConfirmViewModel, string[]>(_Mnemonic);
            }
        }

        public override void Prepare()
        {
            string rawMnemonic = GetMnemonicFromWalletOrSecureStorage();

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

        private string GetMnemonicFromWalletOrSecureStorage()
        {
            string rawMnemonic;

            if (SecureStorageProvider.HasMnemonic())
            {
                rawMnemonic = SecureStorageProvider.GetMnemonic();

                _WalletService.Logger.Information($"Wallet got the mnemonic from secure storage, mnemonic: {rawMnemonic}");
            }
            else
            {
                rawMnemonic = _WalletService.NewMnemonic(GetWordListLanguage(), GetWordCount());

                _WalletService.Logger.Information($"Wallet generated a new mnemonic, mnemonic: {rawMnemonic}");

                SecureStorageProvider.SetMnemonic(rawMnemonic);
                SecureStorageProvider.SetSeedBirthday(new DateTimeOffset(DateTime.UtcNow));

                _WalletService.Logger.Information("Saved mnemonic to secure storage.");
            }

            // After this we should be able to start the wallet since we have a mnemonic
            if (!_WalletService.IsStarted)
            {
                _WalletService.StartWalletWithWalletId();
            }

            return rawMnemonic;
        }
    }
}