using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.Utils;
using Liviano.Models;
using NBitcoin.Protocol;
using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class RecoverWalletEntryViewModel : BaseViewModel
    {
        Serilog.ILogger _Logger;

        public ICommand OnRecoverEntryCompleted { get; }

        string _WordOne;
        public string WordOne
        {
            get => _WordOne;
            set => SetProperty(ref _WordOne, value);
        }

        string _WordTwo;
        public string WordTwo
        {
            get => _WordTwo;
            set => SetProperty(ref _WordTwo, value);
        }

        string _WordThree;
        public string WordThree
        {
            get => _WordThree;
            set => SetProperty(ref _WordThree, value);
        }

        string _WordFour;
        public string WordFour
        {
            get => _WordFour;
            set => SetProperty(ref _WordFour, value);
        }

        string _WordFive;
        public string WordFive
        {
            get => _WordFive;
            set => SetProperty(ref _WordFive, value);
        }

        string _WordSix;
        public string WordSix
        {
            get => _WordSix;
            set => SetProperty(ref _WordSix, value);
        }

        string _WordSeven;
        public string WordSeven
        {
            get => _WordSeven;
            set => SetProperty(ref _WordSeven, value);
        }

        string _WordEight;
        public string WordEight
        {
            get => _WordEight;
            set => SetProperty(ref _WordEight, value);
        }

        string _WordNine;
        public string WordNine
        {
            get => _WordNine;
            set => SetProperty(ref _WordNine, value);
        }

        string _WordTen;
        public string WordTen
        {
            get => _WordTen;
            set => SetProperty(ref _WordTen, value);
        }

        string _WordEleven;
        public string WordEleven
        {
            get => _WordEleven;
            set => SetProperty(ref _WordEleven, value);
        }

        string _WordTwelve;
        public string WordTwelve
        {
            get => _WordTwelve;
            set => SetProperty(ref _WordTwelve, value);
        }

        public RecoverWalletEntryViewModel()
        {
            _Logger = _WalletService.Logger;

            OnRecoverEntryCompleted = new Command(async () => await CheckMnemonic());
        }

        public string RecoverTitle
        {
            //TODO: Localize string
            get => "Enter the backup recovery key for the wallet you want to recover.";
        }

        public string RecoverHeader
        {
            //TODO: Localize string
            get => "Enter Backup Recovery Key";
        }

        async Task CheckMnemonic()
        {
            if (CheckWordInWordlist(WordOne) == false) return;
            if (CheckWordInWordlist(WordTwo) == false) return;
            if (CheckWordInWordlist(WordThree) == false) return;
            if (CheckWordInWordlist(WordFour) == false) return;
            if (CheckWordInWordlist(WordFive) == false) return;
            if (CheckWordInWordlist(WordSix) == false) return;
            if (CheckWordInWordlist(WordSeven) == false) return;
            if (CheckWordInWordlist(WordEight) == false) return;
            if (CheckWordInWordlist(WordNine) == false) return;
            if (CheckWordInWordlist(WordTen) == false) return;
            if (CheckWordInWordlist(WordEleven) == false) return;
            if (CheckWordInWordlist(WordTwelve) == false) return;

            string mnemonic = string.Join(" ", new string[]
                                          {
                                              WordOne, WordTwo, WordThree, WordFour,
                                              WordFive, WordSix, WordSeven, WordEight,
                                              WordNine, WordTen, WordEleven, WordTwelve
                                          });

            mnemonic = mnemonic.ToLower();

            if (CheckMnemonicHasValidChecksum(mnemonic) == false) return;

            SecureStorageService.SetMnemonic(mnemonic);

            _WalletService.StartWalletWithWalletId();

            MessagingCenter.Send(this, "NavigateToRootView");
        }

        bool CheckWordInWordlist(string word, string wordlist = "english")
        {
            if (_WalletService.IsWordInWordlist(word.ToLower(), wordlist) == true) return true;

            _Logger.Information("User input not found in wordlist.");

            DisplayRecoverAlert();

            return false;
        }

        bool CheckMnemonicHasValidChecksum(string mnemonic, string wordlist = "english")
        {
            if (_WalletService.IsVerifyChecksum(mnemonic, wordlist) == true) return true;

            _Logger.Information("Mnemonic returned invalid checksum.");

            DisplayRecoverAlert();

            return false;
        }

        void DisplayRecoverAlert()
        {
            MessagingCenter.Send(this, "RecoverySeedError");
        }
    }
}