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

            string rawMnemonic = _WalletService.NewMnemonic(GetWordListLanguage(), GetWordCount());

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
