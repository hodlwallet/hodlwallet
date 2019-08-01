using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HodlWallet2.Core.Services;
using HodlWallet2.UI.Views;
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

            if (SecureStorageService.HasMnemonic())
            {
                rawMnemonic = SecureStorageService.GetMnemonic();

                _WalletService.Logger.Information($"Wallet got the mnemonic from secure storage, mnemonic: {rawMnemonic}");
            }
            else
            {
                rawMnemonic = _WalletService.NewMnemonic(GetWordListLanguage(), GetWordCount());

                _WalletService.Logger.Information($"Wallet generated a new mnemonic, mnemonic: {rawMnemonic}");

                SecureStorageService.SetMnemonic(rawMnemonic);
                SecureStorageService.SetSeedBirthday(new DateTimeOffset(DateTime.UtcNow));

                _WalletService.Logger.Information("Saved mnemonic to secure storage.");
            }

            // After this we should be able to start the wallet since we have a mnemonic
            if (!_WalletService.IsStarted)
                Task.Run(_WalletService.StartWalletWithWalletId);

            return rawMnemonic;
        }
    }
}